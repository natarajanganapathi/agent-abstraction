namespace Harness.Agents;

internal sealed class HarnessChatAgent : AiAgentBase
{
    protected override string AgentName => "HarnessChatAgent";

    protected override string Description => "Example agent used by the harness for chat and responses clients.";

    protected override StringBuilder Instructions => new("You are the harness validation agent. Use available tools when helpful and answer clearly.");

    protected override IEnumerable<Delegate> GetTools() => [GetWeather, GetLibraryPurpose];

    private static string GetWeather(string city) => $"Harness weather for {city}: 24C and sunny.";

    private static string GetLibraryPurpose() => "This library provides reusable Microsoft Agent Framework abstractions.";
}
