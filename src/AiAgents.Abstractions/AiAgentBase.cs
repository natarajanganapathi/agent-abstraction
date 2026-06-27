namespace AiAgents.Abstractions;

public abstract class AiAgentBase
{
    protected abstract string AgentName { get; }
    protected abstract string Description { get; }
    protected abstract StringBuilder Instructions { get; }

    protected virtual IEnumerable<Delegate> GetTools() => [];

    public AIAgent GetChatAgent(IChatClient client, ChatAgentOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        options ??= new ChatAgentOptions();

        return new ChatClientAgent(
            options.ClientFactory?.Invoke(client) ?? client,
            Instructions.ToString(),
            AgentName,
            Description,
            BuildToolsList(),
            options.LoggerFactory,
            options.Services);
    }

    public AIAgent GetResponsesAgent(ResponsesClient client, ResponsesAgentOptions options)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ModelName, nameof(options.ModelName));

        return client.AsAIAgent(
            options.ModelName,
            Instructions.ToString(),
            AgentName,
            Description,
            BuildToolsList(),
            options.ClientFactory,
            options.LoggerFactory,
            options.Services);
    }

    protected List<AITool>? BuildToolsList()
    {
        var tools = GetTools()
            .Select(d => AIFunctionFactory.Create(d))
            .Cast<AITool>()
            .ToList();

        return tools.Count > 0 ? tools : null;
    }
}
