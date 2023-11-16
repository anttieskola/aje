namespace AJE.Test.Unit;
public class Playground
{
#pragma warning disable S2699
    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task FerrisWheel()
    {
        await Task.Delay(TimeSpan.FromMicroseconds(1));
    }
#pragma warning restore S2699
}
