namespace AiAgents.AzureFoundry.Abstractions;

public record AiAgentFoundryClientOptions : AiAgentClientOptions
{
    public TokenCredential Credential { get; init; } = new DefaultAzureCredential();
    public AgentRef? AgentRef { get; init; }
}
