namespace AiAgents.Abstractions;

public static class AiAgentClientFactory
{
    public static IChatClient CreateChatClient(OpenAIChatClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ModelName, nameof(options.ModelName));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey, nameof(options.ApiKey));

        return new ChatClient(options.ModelName, options.ApiKey).AsIChatClient();
    }

    public static ResponsesClient CreateResponsesClient(OpenAIResponsesClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey, nameof(options.ApiKey));

        return new ResponsesClient(options.ApiKey);
    }
}
