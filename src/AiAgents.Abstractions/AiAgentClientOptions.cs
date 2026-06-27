namespace AiAgents.Abstractions;

public record AiAgentClientOptions
{
    public required string ModelName { get; init; }
    public required Uri Endpoint { get; init; }

    public string? ApiKey { get; init; }
    public Func<IChatClient, IChatClient>? ClientFactory { get; init; }
    public Microsoft.Extensions.Logging.ILoggerFactory? LoggerFactory { get; init; }
    public IServiceProvider? Services { get; init; }
}
