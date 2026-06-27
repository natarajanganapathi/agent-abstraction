namespace Harness.Agents;

internal sealed class HarnessFoundryProjectAgent : AiAgentFoundryBase
{
    protected override string AgentName => "HarnessFoundryProjectAgent";

    protected override string Description => "Example Foundry project agent used by the harness.";

    protected override StringBuilder Instructions => new("You are the Foundry project harness agent. Use tools when useful and answer clearly.");

    protected override IEnumerable<Delegate> GetTools() => [GetWeather];

    private static string GetWeather(string city) => $"Foundry project harness weather for {city}: 24C and clear skies.";
}
