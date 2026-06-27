namespace AiAgents.Abstractions;

public record OpenAIResponsesClientOptions
{
    public required string ApiKey { get; init; }
}
