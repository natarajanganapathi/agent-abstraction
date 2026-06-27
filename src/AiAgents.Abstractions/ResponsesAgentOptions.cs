namespace AiAgents.Abstractions;

public record ResponsesAgentOptions
{
    public required string ModelName { get; init; }
    public Func<IChatClient, IChatClient>? ClientFactory { get; init; }
    public Microsoft.Extensions.Logging.ILoggerFactory? LoggerFactory { get; init; }
    public IServiceProvider? Services { get; init; }
}
