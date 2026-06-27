namespace Harness.Agents;

internal sealed class HarnessFoundryVersionedAgent : AiAgentFoundryBase
{
    protected override string AgentName => "HarnessFoundryVersionedAgent";

    protected override string Description => "Example Foundry versioned agent loader used by the harness.";

    protected override StringBuilder Instructions => new("You are the Foundry versioned harness agent.");
}
