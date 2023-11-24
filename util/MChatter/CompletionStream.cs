namespace AJE.Util.MChatter;

public class CompletionStream : IDisposable
{
    private readonly Uri _serverUri = new("http://localhost:5999/completion");
    private readonly HttpClient _client;

    public CompletionStream()
    {
        _client = new HttpClient();
    }

    public async Task Execute(string input, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _serverUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = CreateContent(input);
        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);
        File.Delete("/tmp/MChatter-response.json");
        using var writer = new StreamWriter("/tmp/MChatter-response.json");
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(ct);
            await writer.WriteLineAsync(line);
            if (!string.IsNullOrWhiteSpace(line))
            {
                var data = line.Replace("data: ", string.Empty);
                try
                {

                    var completion = JsonSerializer.Deserialize<CompletionResponse>(data);
                    if (completion != null && !completion.Stop)
                    {
                        Console.Write(completion.Content);
                    }
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"Error parsing json: {e.Message}");
                }
            }
        }
        Console.Write(Environment.NewLine);
    }

    #region Prompt

    private static readonly string[] _instructions =
    {
        "You are Antti",
        "You were born september 12th in 1980",
        "You were born in Rovaniemi Finland",
        "Your current location is Kempele Finland",
        "You think you are very funny",
        "You joke about everything and anything",
        "You make people laugh",
        "You try to be a good person",
        "You try to cheer everyone up"
    };

    private static string CreatePrompt(string input)
    {
        var sb = new StringBuilder();
        sb.Append("<|im_start|>system\n");
        foreach (var instruction in _instructions)
        {
            sb.Append(instruction);
            sb.Append('\n');
        }
        sb.Append($"Current time is {DateTime.UtcNow.ToString("s")}\n<|im_end|>");
        sb.Append("\n<|im_start|>context\n");
        sb.Append(input);
        sb.Append("\n<|im_end|>\n<|im_start|>Antti\n");
        return sb.ToString();
    }

    private static StringContent CreateContent(string input)
    {
        var request = new CompletionRequest
        {
            Prompt = CreatePrompt(input),
            Stop = new string[] { "<|im_start|>", "<|im_end|>" },
            Stream = true,
            Temperature = 1.2,
            NumberOfTokensToPredict = 256,
            NumberOfTokensToKeep = -1,
        };

        // we gotta allow the < and > characters as llama.cpp won't decode
        // even if we define the characters as allowed it still does not work
        // only way is to use dangerous UnsafeRelaxedJsonEscaping
        // Global block list characters can't be allowed like: AllowCharacters('\u003C', '\u003E')
        // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/character-encoding
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //Encoder = JavaScriptEncoder.Create(encoderSettings),
        };
        var json = JsonSerializer.Serialize(request, options);
        File.WriteAllText("/tmp/MChatter-request.json", json);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #endregion Prompt

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable
}
