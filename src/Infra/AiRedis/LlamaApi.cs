namespace AJE.Infra.AiRedis;

/// <summary>
/// This class is responsible for the communication with the llama.cpp server
/// NOTE!!! The request content must be very carefully inspected & filtered as there
/// is NO SAFETY in this class or server, all input is directly fed to model.
/// </summary>
public class LlamaApi
{
    private const int MaxTries = 10;
    private static TimeSpan GetDelay() => TimeSpan.FromMilliseconds(new Random().Next(100, 2000));
    private readonly ILogger<LlamaApi> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Uri _serverUri;
    private readonly int _timeoutInSeconds;
    private readonly int _maxTokenCount;

    public LlamaApi(
        ILogger<LlamaApi> logger,
        IHttpClientFactory httpClientFactory,
        LlamaServer server)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _serverUri = new Uri(server.Host);
        _timeoutInSeconds = server.TimeoutInSeconds;
        _maxTokenCount = server.MaxTokenCount;
    }

    public async Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        if (request.Stream)
            throw new ArgumentException($"Use {nameof(CompletionStreamAsync)} when Stream enabled", nameof(request));

        await CheckContentIsNotTooLarge(request.Prompt, cancellationToken);
        int tries = 1;
        while (tries <= MaxTries)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "completion"));
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = Serialize(request);
            try
            {
                return await CompletionAsync(httpRequest, cancellationToken);
            }
            catch (AiBusyException)
            {
                var delay = GetDelay();
                _logger.LogWarning("Server is busy, retrying {tries}/{maxTries} after {delay} ms", tries, MaxTries, delay);
                await Task.Delay(delay, cancellationToken);
                tries++;
            }
        }
        throw new AiBusyException($"Server is busy, tried {MaxTries} times");
    }

    private async Task<CompletionResponse> CompletionAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(json))
            throw new InvalidOperationException("Empty response from server");
        // server can be busy (File Not Found)
        if (json.StartsWith("File Not Found"))
        {
            throw new AiBusyException("Server is busy");
        }
        var response = JsonSerializer.Deserialize<CompletionResponse>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize response from server: {json}");
        _logger.LogTrace("Model response: {}", response.Content);
        return response;
    }

    public async Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken)
    {
        if (!request.Stream)
            throw new ArgumentException($"Use {nameof(CompletionAsync)} when Stream disabled", nameof(request));

        ArgumentNullException.ThrowIfNull(tokenCreatedCallback);
        await CheckContentIsNotTooLarge(request.Prompt, cancellationToken);

        int tries = 1;
        while (tries <= MaxTries)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "completion"));
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = Serialize(request);
            try
            {
                return await CompletionStreamAsync(httpRequest, tokenCreatedCallback, cancellationToken);
            }
            catch (AiBusyException)
            {
                var delay = GetDelay();
                _logger.LogWarning("Server is busy, retrying {tries}/{maxTries} after {delay} ms", tries, MaxTries, delay);
                await Task.Delay(delay, cancellationToken);
                tries++;
            }
        }
        throw new AiBusyException($"Server is busy, tried {MaxTries} times");
    }

    private async Task<CompletionResponse> CompletionStreamAsync(HttpRequestMessage httpRequest, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
        var httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        var sb = new StringBuilder();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(line))
            {
                // server can be busy (error: {"content":"slot unavailable"})
                if (line.StartsWith("error"))
                {
                    throw new AiBusyException("Server is busy");
                }
                var data = line.Replace("data: ", string.Empty);
                try
                {
                    var completion = JsonSerializer.Deserialize<CompletionResponse>(data);
                    if (completion != null)
                    {
                        if (!completion.Stop)
                        {
                            await tokenCreatedCallback.Invoke(completion.Content);
                            sb.Append(completion.Content);
                        }
                        else
                        {
                            completion.Content = sb.ToString();
                            _logger.LogTrace("Model response: {}", completion.Content);
                            return completion;
                        }
                    }
                    else
                        throw new AiException($"invalid response from server: {data}");
                }
                catch (JsonException e)
                {
                    _logger.LogError("Error parsing json: {}", e.Message);
                    throw;
                }
            }
        }
        _logger.LogError("Model response: {}", sb.ToString());
        throw new InvalidOperationException("Stream ended without stop");
    }

    public async Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "tokenize"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Content = Serialize(request);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            // Can't get this to give errors from unavailability like other api's
            var response = JsonSerializer.Deserialize<TokenizeResponse>(json);
            return response ?? throw new AiException($"invalid response from server: {json}");
        }
        catch (JsonException e)
        {
            _logger.LogError("Error parsing json: {}", e.Message);
            throw;
        }
    }

    private async Task CheckContentIsNotTooLarge(string content, CancellationToken cancellationToken)
    {
        var response = await TokenizeAsync(new TokenizeRequest { Content = content }, cancellationToken);
        int tokenCount = response.Tokens.Length;
        if (tokenCount > _maxTokenCount)
            throw new AiException($"Content is too large, token count: {tokenCount}, max token count: {_maxTokenCount}");
    }

    public async Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "detokenize"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Content = Serialize(request);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            var response = JsonSerializer.Deserialize<DeTokenizeResponse>(json);
            return response ?? throw new AiException($"invalid response from server: {json}");
        }
        catch (JsonException e)
        {
            _logger.LogError("Error parsing json: {}", e.Message);
            throw;
        }
    }

    public async Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
    {
        int tries = 1;
        while (tries <= MaxTries)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_serverUri, "embedding"));
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = Serialize(request);
            try
            {
                return await EmbeddingAsync(httpRequest, cancellationToken);
            }
            catch (AiBusyException)
            {
                var delay = GetDelay();
                _logger.LogWarning("Server is busy, retrying {tries}/{maxTries} after {delay} ms", tries, MaxTries, delay);
                await Task.Delay(delay, cancellationToken);
                tries++;
            }
        }
        throw new AiBusyException($"Server is busy, tried {MaxTries} times");
    }

    private async Task<EmbeddingResponse> EmbeddingAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
        var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        // server response in case of busy: {"content":"slot unavailable"}
        if (json.StartsWith("{\"content\":\"slot unavailable\"}"))
            throw new AiBusyException("Server is busy");
        try
        {
            // server can be busy returns nothing
            var response = JsonSerializer.Deserialize<EmbeddingResponse>(json);
            return response ?? throw new AiException($"invalid response from server: {json}");
        }
        catch (JsonException e)
        {
            _logger.LogError("Error parsing json: {}", e.Message);
            throw;
        }
    }

    // We gotta allow the < and > ('\u003C', '\u003E') characters as llama.cpp won't decode anything
    // for the model. It does not work to use AllowCharacters as if character is in the Global block list
    // like these are there is no other way to allow them than to use UnsafeRelaxedJsonEscaping.
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/character-encoding
    // We have to be carefull what input we allow to this class as it will be passed to the model.
    private readonly JsonSerializerOptions _unsafeJsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private StringContent Serialize(object request)
    {
        var json = JsonSerializer.Serialize(request, _unsafeJsonSerializerOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
