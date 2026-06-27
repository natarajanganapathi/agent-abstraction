namespace AiAgents.AzureFoundry.Abstractions;

public abstract class AiAgentFoundryBase : AiAgentBase
{
    public AIAgent GetProjectAgent(AIProjectClient client, FoundryProjectAgentOptions options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ModelName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(options.ModelName));
        }

        return client.AsAIAgent(
            model: options.ModelName,
            instructions: Instructions.ToString(),
            name: AgentName,
            description: Description,
            tools: BuildToolsList(),
            clientFactory: options.ClientFactory,
            loggerFactory: options.LoggerFactory,
            services: options.Services);
    }

    public AIAgent GetVersionedAgent(AIProjectClient client, FoundryVersionedAgentOptions options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.AgentRef, nameof(options.AgentRef));

        if (string.IsNullOrWhiteSpace(options.AgentRef.Name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", $"{nameof(options.AgentRef)}.{nameof(options.AgentRef.Name)}");
        }
        var reference = new AgentReference(options.AgentRef.Name, options.AgentRef.Version);
        return client.AsAIAgent(reference, BuildToolsList(), options.ClientFactory, options.Services);
    }
}
