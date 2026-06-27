namespace AiAgents.AzureFoundry.Abstractions;

public abstract class AiAgentFoundryBase<TClient> : AiAgentBase<TClient> where TClient : AIProjectClient
{
    protected override TClient CreateClient(AiAgentClientOptions agentOptions)
    {
        if (agentOptions is not AiAgentFoundryClientOptions options) { throw new NotSupportedException($"'{typeof(TClient).Name}' is not supported by {nameof(AiAgentFoundryBase<TClient>)}."); }
        var projectClient = new AIProjectClient(options.Endpoint, options.Credential);
        return (TClient)projectClient;
    }

    protected override AIAgent CreateAgent(TClient projectClient, AiAgentClientOptions agentOptions)
    {
        if (agentOptions is not AiAgentFoundryClientOptions options) { throw new NotSupportedException($"'{typeof(TClient).Name}' is not supported by {nameof(AiAgentFoundryBase<TClient>)}."); }

        var tools = BuildToolsList();

        if (options.AgentRef is { } agentRef)
        {
            var reference = new AgentReference(agentRef.Name, agentRef.Version);
            return projectClient.AsAIAgent(reference, tools, options.ClientFactory, options.Services);
        }

        return projectClient.AsAIAgent(
            model: options.ModelName,
            instructions: Instructions.ToString(),
            name: AgentName,
            description: Description,
            tools: tools,
            clientFactory: options.ClientFactory,
            loggerFactory: options.LoggerFactory,
            services: options.Services);
    }
}
