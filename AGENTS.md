# aiagents-abstractions — Agent session guide

## Identity

- **Microsoft Agent Framework (MAF) abstractions library.**
- Provides reusable building blocks for creating agents: **tools**, **MCP**, **skills**, etc.
- This repo is **owned by pronative-ai** and consumed by other repos in the organization.

## Project structure

```
aiagents-abstractions/
├── .github/workflows/
│   ├── ci.yml                     # format → build → test on PR/push
│   └── publish.yml                # pack → NuGet publish on version tag
├── src/AiAgents.Abstractions/
│   ├── AiAgents.Abstractions.csproj
│   └── AiAgentBase.cs
├── src/AiAgents.AzureFoundry.Abstractions/
│   ├── AiAgents.AzureFoundry.Abstractions.csproj
│   └── AiAgentFoundryBase.cs
├── tests/AiAgents.Abstractions.Tests/
│   ├── AiAgents.Abstractions.Tests.csproj
│   └── AiAgentBaseTests.cs
├── tests/AiAgents.AzureFoundry.Abstractions.Tests/
│   ├── AiAgents.AzureFoundry.Abstractions.Tests.csproj
│   └── AiAgentFoundryBaseTests.cs
├── .editorconfig
├── .gitignore
├── Directory.Build.props           # Shared: net10.0, nullable, implicit usings
├── Directory.Packages.props        # Central NuGet version management
├── Makefile                        # format → build → test → pack shortcuts
├── README.md
└── aiagents-abstractions.slnx      # .NET solution file (.slnx format)
```

## Library packages

| Library | Purpose | MAF dependency |
|---------|---------|---------------|
| `AiAgents.Abstractions` | General-purpose agent base (Chat/Responses APIs) | `Microsoft.Agents.AI` |
| `AiAgents.AzureFoundry.Abstractions` | Foundry-specific agent base | `Microsoft.Agents.AI.Foundry` + `Azure.AI.Projects` |

## Current state

Scaffolding complete — `dotnet format`, `dotnet build`, and `dotnet test` all pass.

## MAF references

This library's consumers will reference MAF NuGet packages. The core package is `Microsoft.Agents.AI` (MAF 1.0+ GA, MIT license). Depending on which abstractions are exposed, consumers may also pull:

| Concern | Package |
|---------|---------|
| Core agent, tool, MCP abstractions | `Microsoft.Agents.AI` |
| OpenAI / Azure OpenAI provider | `Microsoft.Agents.AI.OpenAI` |
| Azure AI Foundry integration | `Microsoft.Agents.AI.Foundry` |
| Multi-agent graph workflows | `Microsoft.Agents.AI.Workflows` |
| Agent-to-Agent protocol | `Microsoft.Agents.AI.A2A` |
| Anthropic Claude provider | `Microsoft.Agents.AI.Anthropic` |

Key API surface: `AsAIAgent()`, `RunAsync()`, `AIFunctionFactory.Create()`.

## Design decisions

### Base class design (Option A)

The library provides `AiAgentBase` — subclasses override providers, the base class handles all MAF wiring internally.

**Subclass contract:**

| Subclass provides | Base class handles |
|---|---|
| `StringBuilder Instructions` | Converted via `.ToString()` and passed to `ChatClientAgent(instructions:)` |
| `IEnumerable<Delegate> GetTools()` | Wraps with `AIFunctionFactory.Create()` and registers on agent |

**Public methods on base:**
- `AIAgent GetChatAgent(IChatClient client)` — builds a fully wired agent via Chat API (works with any `IChatClient`)
- `AIAgent GetResponsesAgent(ResponsesClient client, string model)` — builds a fully wired agent via OpenAI Responses API

**MCP and memory** not yet wired — `McpServerOptions` and `IChatMemory` types are not present in the current MAF packages. Can be added when these types become available or via `ChatClientAgentOptions`.

**Note:** `ResponsesClient` is an evaluation-only API (`OPENAI001` diagnostic suppressed via `#pragma warning disable`).

**Foundry versioned agents** (loaded from portal via `AIProjectClient.AsAIAgent(record)`) own their tools server-side and **do not support adding local tools, MCP, or memory at runtime**. This restriction is Foundry-specific — all other providers (OpenAI, Azure OpenAI, Anthropic, etc.) support full local wiring.

**Consumer usage:**
```csharp
public class MySupportAgent : AiAgentBase
{
    protected override StringBuilder Instructions => new("You are a support agent.");
    protected override IEnumerable<Delegate> GetTools() => [GetWeather];

    private string GetWeather(string city) => "25°C";
}

// Usage — one method call
var agent = new MySupportAgent();
var aiAgent = agent.GetChatAgent(chatClient);
await aiAgent.RunAsync("What's the weather in Tokyo?");
```

### Foundry base class design

`AiAgents.AzureFoundry.Abstractions` provides `AiAgentFoundryBase` for Foundry-specific agents:

| Subclass provides | Base class handles |
|---|---|
| `StringBuilder Instructions` | Converted via `.ToString()` and passed to `AIProjectClient.AsAIAgent(...)` |
| `string ModelName` | The Foundry model deployment name |

```csharp
public class MyFoundryAgent : AiAgentFoundryBase
{
    public MyFoundryAgent(Uri endpoint, TokenCredential credential)
        : base(endpoint, credential) { }

    protected override StringBuilder Instructions => new("You are a Foundry agent.");
    protected override string ModelName => "gpt-4o";
}

var agent = new MyFoundryAgent(endpoint, credential).GetAgent();
await agent.RunAsync("Hello");
```

**Note:** Foundry versioned agents (loaded from portal via `AsAIAgent(record)`) own their tools server-side and do not support adding local tools, MCP, or memory at runtime. This restriction is Foundry-specific.

## Key constraints for future agents

1. **No assumptions** — verify the actual build/test tooling before running any command.
2. **Stick to MAF idioms** — abstractions here should mirror how MAF models tools, MCP servers, and skills. Use the `Microsoft.Agents.AI` namespace conventions.
3. **Consumer-first design** — other repos will reference this library; keep public API surface minimal and typed. Do not re-export MAF types that consumers can reference directly.
4. **.NET stack** — target `net10.0` with `<PackageReference>` for dependencies. Use `dotnet format` (lint) → `dotnet build` (typecheck) → `dotnet test` before writing feature code.
