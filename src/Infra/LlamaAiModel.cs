namespace AJE.Infra;

/// <summary>
/// This class is responsible for the communication with the llama.cpp server
/// NOTE!!! The request content must be very carefully inspected & filtered as there
/// is NO SAFETY in this class or server, all input is directly fed to model.
/// </summary>
public class LlamaAiModel : IAiModel
{
    private readonly LlamaConfiguration _configuration;
    private readonly Uri _serverUri;

    public LlamaAiModel(LlamaConfiguration configuration)
    {
        _configuration = configuration;
        _serverUri = new Uri(_configuration.Host);
    }

    public async Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        if (request.Stream)
            throw new ArgumentException($"Use {nameof(CompletionStreamAsync)} when Stream enabled", nameof(request));

        using var client = new HttpClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "completion"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Content = Serialize(request);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(json))
            throw new InvalidOperationException("Empty response from server");

        var response = JsonSerializer.Deserialize<CompletionResponse>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize response from server: {json}");

        return response;
    }

    public Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, Stream outputStream, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }

    private static StringContent Serialize(CompletionRequest request)
    {
        // We gotta allow the < and > ('\u003C', '\u003E') characters as llama.cpp won't decode anything
        // for the model. It does not work to use AllowCharacters as if character is in the Global block list
        // like these are there is no other way to allow them than to use UnsafeRelaxedJsonEscaping.
        // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/character-encoding
        // We have to be carefull what input we allow to this class as it will be passed to the model.
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        var json = JsonSerializer.Serialize(request, options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
