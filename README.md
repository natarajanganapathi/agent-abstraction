# aiagents-abstractions

Reusable Microsoft Agent Framework abstractions for building and wiring agents with a small, provider-aware API surface.

## Packages

- `AiAgents.Abstractions`: provider-agnostic base class for chat and OpenAI Responses-backed agents
- `AiAgents.AzureFoundry.Abstractions`: Azure AI Foundry-specific base class for project and versioned agents

## Core Usage

```csharp
using AiAgents.Abstractions;
using Microsoft.Extensions.AI;
using OpenAI.Responses;
using System.Text;

public sealed class SupportAgent : AiAgentBase
{
    protected override string AgentName => "SupportAgent";
    protected override string Description => "Handles support questions.";
    protected override StringBuilder Instructions => new("You are a support agent.");
    protected override IEnumerable<Delegate> GetTools() => [GetWeather];

    private static string GetWeather(string city) => $"Sunny in {city}";
}

IChatClient chatClient = GetChatClient();
var agent = new SupportAgent().GetChatAgent(chatClient);
```

## Responses Usage

```csharp
var responsesClient = new ResponsesClient("api-key");
var agent = new SupportAgent().GetResponsesAgent(
    responsesClient,
    new ResponsesAgentOptions { ModelName = "gpt-4o" });
```

## Foundry Usage

```csharp
using AiAgents.Abstractions;
using AiAgents.AzureFoundry.Abstractions;
using Azure.AI.Projects;
using Azure.Identity;
using System.Text;

public sealed class FoundrySupportAgent : AiAgentFoundryBase
{
    protected override string AgentName => "FoundrySupportAgent";
    protected override string Description => "Foundry-hosted support agent.";
    protected override StringBuilder Instructions => new("You are a Foundry support agent.");
}

var projectClient = new AIProjectClient(new Uri("https://my-project.services.ai.azure.com/api/projects/my-project"), new DefaultAzureCredential());

var projectAgent = new FoundrySupportAgent().GetProjectAgent(
    projectClient,
    new FoundryProjectAgentOptions { ModelName = "gpt-4o" });

var versionedAgent = new FoundrySupportAgent().GetVersionedAgent(
    projectClient,
    new FoundryVersionedAgentOptions { AgentRef = new AgentRef("support-agent", "1") });
```

## Notes

- Consumers provide provider clients directly; this library focuses on wiring agents rather than wrapping SDK client creation.
- Chat agents are standardized on `IChatClient` to keep the core abstraction provider-agnostic.
- Foundry project agents and Foundry versioned agents are modeled as separate entry points because their runtime capabilities differ.
