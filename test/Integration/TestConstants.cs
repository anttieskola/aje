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
}
