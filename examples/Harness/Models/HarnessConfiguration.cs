namespace Harness.Models;

internal sealed record HarnessConfiguration(
    string Prompt,
    string? OpenAIApiKey,
    string OpenAIChatModel,
    string OpenAIResponsesModel,
    string? FoundryProjectEndpoint,
    string? FoundryModel,
    string? FoundryAgentName,
    string FoundryAgentVersion)
{
    public static HarnessConfiguration FromEnvironment(string prompt)
    {
        var chatModel = Environment.GetEnvironmentVariable("OPENAI_CHAT_MODEL") ?? "gpt-4o";

        return new HarnessConfiguration(
            prompt,
            Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
            chatModel,
            Environment.GetEnvironmentVariable("OPENAI_RESPONSES_MODEL") ?? chatModel,
            Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_PROJECT_ENDPOINT"),
            Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_MODEL"),
            Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_AGENT_NAME"),
            Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_AGENT_VERSION") ?? "latest");
    }
}
