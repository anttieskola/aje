namespace AJE.Domain;

public record WikipediaHttpQuery
{

}

// https://www.mediawiki.org/wiki/API:Main_page
public class WikipediaHttpQueryHandler : IRequestHandler<YleHttpQuery, string>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WikipediaHttpQueryHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Handle(YleHttpQuery query, CancellationToken cancellationToken)
    {
        // https://en.wikipedia.org/w/api.php?action=opensearch&search=
        using var client = _httpClientFactory.CreateClient();
        var request = CreateRequest(query.Uri);
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        {
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            await gzipStream.CopyToAsync(decompressedStream, cancellationToken);
            decompressedStream.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(decompressedStream).ReadToEndAsync(cancellationToken);
        }
        else
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }

    private static HttpRequestMessage CreateRequest(Uri uri)
    {
        // https://www.mediawiki.org/wiki/API:Etiquette
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        request.Headers.Add("User-Agent", "AJE https://www.anttieskola.com anttieskola@users.noreply.github.com");
        return request;
    }
}
