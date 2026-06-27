namespace AiAgents.AzureFoundry.Abstractions;

public static class AiAgentFoundryClientFactory
{
    public static AIProjectClient CreateProjectClient(FoundryClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Endpoint);
        ArgumentNullException.ThrowIfNull(options.Credential);

        if (!options.Endpoint.IsAbsoluteUri)
        {
            throw new ArgumentException("Endpoint must be an absolute URI.", nameof(options.Endpoint));
        }

        return new AIProjectClient(options.Endpoint, options.Credential);
    }
}
