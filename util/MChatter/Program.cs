namespace AJE.Util.MChatter;
class Program
{
    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();

        // Listen for the Ctrl+C interrupt
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            using var cs = new CompletionStream();
            Console.WriteLine("Press Ctrl+C and enter to exit");
            while (!cts.IsCancellationRequested)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                await cs.Execute(input, cts.Token);
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("oh noes {0}", e.Message);
        }
    }
}

