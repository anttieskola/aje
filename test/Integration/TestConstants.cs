using AJE.Infra.Ai;
using AJE.Infra.FileSystem;
using AJE.Infra.Translate;

namespace AJE.Test.Integration;
public static class TestConstants
{
    public static string LlamaAddress { get; set; } = "http://localhost:8080";
    public static string RedisAddress { get; set; } = "localhost:6379";
    private const string RANDOMCHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz .,?!";
    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        return new string(Enumerable.Repeat(RANDOMCHARACTERS, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public static LlamaConfiguration LlamaConfiguration
    {
        get
        {
            return new LlamaConfiguration
            {

                Servers =
                [
                    new LlamaServer
                    {
                        Host = LlamaAddress,
                        ResourceName = "integration-test-llama",
                        MaxTokenCount = 32768,
                        TimeoutInSeconds = 60,
                    }
                ],
                LogFolder = "/tmp",

            };
        }
    }
    public static TranslateConfiguration TranslateConfiguration
    {
        get
        {
            return new TranslateConfiguration
            {
                Host = "http://127.0.0.1:5888",
                TimeoutInSeconds = 60,
            };
        }
    }

    public static FileSystemConfiguration FileSystemConfiguration
    {
        get
        {
            return new FileSystemConfiguration
            {
                RootFolder = "/var/aje",
            };
        }
    }
}
