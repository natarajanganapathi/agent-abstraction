namespace AiAgents.Abstractions;

public record OpenAIChatClientOptions
{
    public required string ModelName { get; init; }
    public required string ApiKey { get; init; }
}
