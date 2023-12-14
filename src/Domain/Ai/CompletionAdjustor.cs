namespace AJE.Domain.Ai;

public record AiSettings
{
    public required double Temperature { get; init; }
    public required int TopK { get; init; }
}

public static class CompletionAdjustor
{
    public static AiSettings GetSettings(int tryNumber)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(tryNumber, 1, nameof(tryNumber));
        // temperature 0.1 ... 1.0
        // topK 40
        if (tryNumber <= 10)
        {
            return new AiSettings
            {
                Temperature = Math.Round(0.1 * tryNumber, 1),
                TopK = 40,
            };

        }
        // temperature 0.1
        // topK 10 ... 100
        else if (tryNumber <= 20)
        {
            return new AiSettings
            {
                Temperature = 0.1,
                TopK = 10 + (tryNumber - 11) * 10,
            };
        }
        // all combinations of temperature 0.1...1.0 and topK 10...100
        else if (tryNumber <= 99)
        {
            var firstDigit = int.Parse(tryNumber.ToString()[..1]);
            var lastDigit = int.Parse(tryNumber.ToString()[1..]);
            // temp is 0.1 * first digit except when last digit is 0
            var temperature = 0.1 * firstDigit;
            temperature = lastDigit == 0 ? temperature - 0.1 : temperature;
            return new AiSettings
            {
                Temperature = Math.Round(temperature, 1),
                TopK = 10 * (lastDigit == 0 ? 10 : lastDigit),
            };
        }
        else if (tryNumber == 100)
        {
            return new AiSettings
            {
                Temperature = 0.9,
                TopK = 100,
            };
        }
        else if (tryNumber <= 110)
        {
            return new AiSettings
            {
                Temperature = 1.0,
                TopK = 10 + (tryNumber - 101) * 10,
            };
        }
        // just random until for ever
        else
        {
            var r = new Random();
            return new AiSettings
            {
                Temperature = Math.Round(0.1 * r.Next(1, 10), 1),
                TopK = r.Next(10, 100)
            };
        }

    }
}
