namespace AJE.Domain;

public record YleHttpQuery : IRequest<string>
{
    public required Uri Uri { get; init; }
}

public class YleHttpQueryHandler : IRequestHandler<YleHttpQuery, string>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YleHttpQueryHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Handle(YleHttpQuery query, CancellationToken cancellationToken)
    {
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
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        request.Headers.Add("Cache-Control:", "no-cache");
        request.Headers.Add("Dnt:", "1");
        request.Headers.Add("Pragma:", "no-cache");
        request.Headers.Add("Sec-Ch-Ua", "\"Not A(Brand\";v=\"99\", \"Microsoft Edge\";v=\"121\", \"Chromium\";v=\"121\"");
        request.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        request.Headers.Add("Sec-Ch-Ua-Platform", "\"Linux\"");
        request.Headers.Add("Sec-Fetch-Dest", "document");
        request.Headers.Add("Sec-Fetch-Mode", "navigate");
        request.Headers.Add("Sec-Fetch-Site", "none");
        request.Headers.Add("Sec-Fetch-User", "?1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0");
        return request;
    }
}
