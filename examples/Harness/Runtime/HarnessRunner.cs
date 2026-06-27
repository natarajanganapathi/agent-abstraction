namespace Harness.Runtime;

using Harness.Agents;
using Harness.Models;

internal static class HarnessRunner
{
    public static async Task<HarnessRunResult> RunChatAsync(HarnessConfiguration configuration, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.OpenAIApiKey))
        {
            return HarnessRunResult.Skipped("Set OPENAI_API_KEY to run the chat agent harness.");
        }

        IChatClient chatClient = new OpenAIChatClient(configuration.OpenAIChatModel, configuration.OpenAIApiKey).AsIChatClient();
        var agent = new HarnessChatAgent().GetChatAgent(chatClient);
        var response = await AgentInvoker.RunAsync(agent, configuration.Prompt, cancellationToken);

        Console.WriteLine(AgentOutputFormatter.Format("OpenAI chat / IChatClient", response));
        return HarnessRunResult.ExecutedSuccessfully("Completed OpenAI chat harness.");
    }

    public static async Task<HarnessRunResult> RunResponsesAsync(HarnessConfiguration configuration, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.OpenAIApiKey))
        {
            return HarnessRunResult.Skipped("Set OPENAI_API_KEY to run the responses agent harness.");
        }

        var client = new ResponsesClient(configuration.OpenAIApiKey);
        var agent = new HarnessChatAgent().GetResponsesAgent(
            client,
            new ResponsesAgentOptions { ModelName = configuration.OpenAIResponsesModel });
        var response = await AgentInvoker.RunAsync(agent, configuration.Prompt, cancellationToken);

        Console.WriteLine(AgentOutputFormatter.Format("OpenAI responses", response));
        return HarnessRunResult.ExecutedSuccessfully("Completed OpenAI responses harness.");
    }

    public static async Task<HarnessRunResult> RunFoundryProjectAsync(HarnessConfiguration configuration, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.FoundryProjectEndpoint))
        {
            return HarnessRunResult.Skipped("Set AZURE_AI_FOUNDRY_PROJECT_ENDPOINT to run the Foundry project harness.");
        }

        if (string.IsNullOrWhiteSpace(configuration.FoundryModel))
        {
            return HarnessRunResult.Skipped("Set AZURE_AI_FOUNDRY_MODEL to run the Foundry project harness.");
        }

        var client = new AIProjectClient(new Uri(configuration.FoundryProjectEndpoint), new DefaultAzureCredential());
        var agent = new HarnessFoundryProjectAgent().GetProjectAgent(
            client,
            new FoundryProjectAgentOptions { ModelName = configuration.FoundryModel });
        var response = await AgentInvoker.RunAsync(agent, configuration.Prompt, cancellationToken);

        Console.WriteLine(AgentOutputFormatter.Format("Foundry project agent", response));
        return HarnessRunResult.ExecutedSuccessfully("Completed Foundry project harness.");
    }

    public static async Task<HarnessRunResult> RunFoundryVersionedAsync(HarnessConfiguration configuration, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.FoundryProjectEndpoint))
        {
            return HarnessRunResult.Skipped("Set AZURE_AI_FOUNDRY_PROJECT_ENDPOINT to run the Foundry versioned harness.");
        }

        if (string.IsNullOrWhiteSpace(configuration.FoundryAgentName))
        {
            return HarnessRunResult.Skipped("Set AZURE_AI_FOUNDRY_AGENT_NAME to run the Foundry versioned harness.");
        }

        var client = new AIProjectClient(new Uri(configuration.FoundryProjectEndpoint), new DefaultAzureCredential());
        var agent = new HarnessFoundryVersionedAgent().GetVersionedAgent(
            client,
            new FoundryVersionedAgentOptions
            {
                AgentRef = new AgentRef(configuration.FoundryAgentName, configuration.FoundryAgentVersion),
            });
        var response = await AgentInvoker.RunAsync(agent, configuration.Prompt, cancellationToken);

        Console.WriteLine(AgentOutputFormatter.Format("Foundry versioned agent", response));
        return HarnessRunResult.ExecutedSuccessfully("Completed Foundry versioned harness.");
    }
}
