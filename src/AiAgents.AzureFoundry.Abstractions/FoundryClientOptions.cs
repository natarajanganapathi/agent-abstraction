namespace AiAgents.AzureFoundry.Abstractions;

public record FoundryClientOptions
{
    public required Uri Endpoint { get; init; }
    public TokenCredential Credential { get; init; } = new DefaultAzureCredential();
}
