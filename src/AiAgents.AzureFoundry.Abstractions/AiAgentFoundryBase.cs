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
            BuildAgentOptions(options.ModelName),
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
        var agentOptions = BuildAgentOptions();
        var chatClient = CreateVersionedChatClient(client, reference, agentOptions.ChatOptions ?? new ChatOptions());

        return new ChatClientAgent(
            options.ClientFactory?.Invoke(chatClient) ?? chatClient,
            agentOptions,
            loggerFactory: null,
            services: options.Services);
    }

    private static IChatClient CreateVersionedChatClient(AIProjectClient client, AgentReference reference, ChatOptions chatOptions)
    {
        var foundryAssembly = typeof(AzureAIProjectChatClientExtensions).Assembly;
        var chatClientType = foundryAssembly.GetType("Microsoft.Agents.AI.Foundry.AzureAIProjectChatClient", throwOnError: true)!;
        var instance = Activator.CreateInstance(
            chatClientType,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
            binder: null,
            args: [client, reference, string.Empty, chatOptions],
            culture: null);

        return (IChatClient)instance!;
    }
}
