namespace Harness;

internal static class UsagePrinter
{
    public static void Print()
    {
        Console.WriteLine("Usage: dotnet run --project examples/Harness -- [all|chat|responses|foundry-project|foundry-versioned] [prompt]");
        Console.WriteLine();
        Console.WriteLine("Environment variables:");
        Console.WriteLine("  OPENAI_API_KEY                 Required for chat and responses");
        Console.WriteLine("  OPENAI_CHAT_MODEL              Optional, defaults to gpt-4o");
        Console.WriteLine("  OPENAI_RESPONSES_MODEL         Optional, defaults to OPENAI_CHAT_MODEL or gpt-4o");
        Console.WriteLine("  AZURE_AI_FOUNDRY_PROJECT_ENDPOINT  Required for Foundry project and versioned agents");
        Console.WriteLine("  AZURE_AI_FOUNDRY_MODEL         Required for Foundry project agent");
        Console.WriteLine("  AZURE_AI_FOUNDRY_AGENT_NAME    Required for Foundry versioned agent");
        Console.WriteLine("  AZURE_AI_FOUNDRY_AGENT_VERSION Optional, defaults to latest");
        Console.WriteLine("  AGENT_HARNESS_PROMPT           Optional default prompt");
    }
}
