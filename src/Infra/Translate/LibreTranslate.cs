namespace AJE.Infra.Translate;

public class LibreTranslate : ITranslate
{
    private readonly ILogger<LibreTranslate> _logger;
    private readonly TranslateConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Uri _serverUri;

    public LibreTranslate(
        ILogger<LibreTranslate> logger,
        TranslateConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };
        _serverUri = new Uri(_config.Host);
    }

    public async Task<TranslateResponse> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken)
    {
        // create request
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "translate"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, "application/json");

        // send request
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_config.TimeoutInSeconds);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(json))
        {
            _logger.LogError("Failed to translate text. Response empty/null");
            throw new TranslateException("Failed to translate text.");
        }
        var response = JsonSerializer.Deserialize<TranslateResponse>(json, _jsonSerializerOptions);
        if (response == null)
        {
            _logger.LogError("Failed to translate text, deserialization failed. Response:{json}", json);
            throw new TranslateException("Failed to translate text.");
        }
        return response;
    }
}
