namespace Harness;

internal static class HarnessApp
{
    public static async Task<int> RunAsync(string[] args)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        var cancellationToken = cancellationTokenSource.Token;
        var mode = args.Length > 0 ? args[0].Trim().ToLowerInvariant() : HarnessModes.AllMode;
        var prompt = args.Length > 1
            ? string.Join(' ', args.Skip(1))
            : Environment.GetEnvironmentVariable("AGENT_HARNESS_PROMPT") ?? "What is the weather in Seattle today?";

        if (!HarnessModes.All.Contains(mode))
        {
            UsagePrinter.Print();
            return 1;
        }

        var configuration = HarnessConfiguration.FromEnvironment(prompt);
        var executed = 0;
        var failed = 0;

        foreach (var selectedMode in HarnessModes.Resolve(mode))
        {
            var result = await ExecuteModeAsync(selectedMode, configuration, cancellationToken);

            Console.WriteLine($"[{selectedMode}] {result.Status}: {result.Message}");

            if (result.Executed)
            {
                executed++;
            }

            if (!result.Success)
            {
                failed++;
            }

            Console.WriteLine();
        }

        if (executed == 0)
        {
            Console.WriteLine("No harnesses ran. Configure the required environment variables and run again.");
            UsagePrinter.Print();
            return 1;
        }

        return failed == 0 ? 0 : 1;
    }

    private static async Task<HarnessRunResult> ExecuteModeAsync(
        string mode,
        HarnessConfiguration configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            return mode switch
            {
                HarnessModes.Chat => await HarnessRunner.RunChatAsync(configuration, cancellationToken),
                HarnessModes.Responses => await HarnessRunner.RunResponsesAsync(configuration, cancellationToken),
                HarnessModes.FoundryProject => await HarnessRunner.RunFoundryProjectAsync(configuration, cancellationToken),
                HarnessModes.FoundryVersioned => await HarnessRunner.RunFoundryVersionedAsync(configuration, cancellationToken),
                _ => HarnessRunResult.Skipped($"Unknown mode '{mode}'."),
            };
        }
        catch (Exception exception)
        {
            return HarnessRunResult.Failed(exception.Message);
        }
    }
}
