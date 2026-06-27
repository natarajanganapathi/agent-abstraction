namespace AiAgents.AzureFoundry.Abstractions;

public record FoundryVersionedAgentOptions
{
    public required AgentRef AgentRef { get; init; }
    public Func<IChatClient, IChatClient>? ClientFactory { get; init; }
    public IServiceProvider? Services { get; init; }
}
