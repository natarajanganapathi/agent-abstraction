namespace AiAgents.Abstractions;

public abstract class AiAgentBase<TClient> where TClient : class
{
    protected abstract string AgentName { get; }
    protected abstract string Description { get; }
    protected abstract StringBuilder Instructions { get; }

    protected virtual IEnumerable<Delegate> GetTools() => [];

    public AIAgent GetAgent(AiAgentClientOptions options)
    {
        var client = CreateClient(options);
        return CreateAgent(client, options);
    }

    protected virtual TClient CreateClient(AiAgentClientOptions options)
    {
        object client = typeof(TClient) switch
        {
            Type t when t == typeof(ChatClient) => new ChatClient(options.ModelName, options.ApiKey!),
            Type t when t == typeof(ResponsesClient) => new ResponsesClient(options.ApiKey!),
            _ => throw new NotSupportedException($"'{typeof(TClient).Name}' cannot be constructed from {nameof(AiAgentClientOptions)}"),
        };
        return (TClient)client;
    }
    protected virtual AIAgent CreateAgent(TClient client, AiAgentClientOptions options)
    {
        var tools = BuildToolsList();
        var instructions = Instructions.ToString();

        return client switch
        {
            IChatClient cc => new ChatClientAgent(
                options.ClientFactory?.Invoke(cc) ?? cc,
                instructions,
                AgentName,
                Description,
                tools,
                options.LoggerFactory,
                options.Services),
            ChatClient raw => raw.AsAIAgent(
                instructions,
                AgentName,
                Description,
                tools,
                options.ClientFactory,
                options.LoggerFactory,
                options.Services),
            ResponsesClient rc => rc.AsAIAgent(
                options.ModelName,
                instructions,
                AgentName,
                Description,
                tools,
                options.ClientFactory,
                options.LoggerFactory,
                options.Services),
            _ => throw new NotSupportedException($"No wrapping strategy for '{typeof(TClient).Name}'."),
        };
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
