namespace AiAgents.AzureFoundry.Abstractions;

public record FoundryProjectAgentOptions
{
    public required string ModelName { get; init; }
    public Func<IChatClient, IChatClient>? ClientFactory { get; init; }
    public ILoggerFactory? LoggerFactory { get; init; }
    public IServiceProvider? Services { get; init; }
}
