# Harness

Console harness for exercising the four supported agent wiring paths in this repository:

- `chat`: `AiAgentBase.GetChatAgent(IChatClient, ...)`
- `responses`: `AiAgentBase.GetResponsesAgent(ResponsesClient, ...)`
- `foundry-project`: `AiAgentFoundryBase.GetProjectAgent(AIProjectClient, ...)`
- `foundry-versioned`: `AiAgentFoundryBase.GetVersionedAgent(AIProjectClient, ...)`

## Run

```powershell
dotnet run --project examples/Harness -- all
dotnet run --project examples/Harness -- chat "What is the weather in Seattle?"
dotnet run --project examples/Harness -- foundry-project
```

## Configuration Template

Use `examples/Harness/.env.example` as the starter template for the environment variables the harness expects.

## Environment Variables

```text
OPENAI_API_KEY
OPENAI_CHAT_MODEL
OPENAI_RESPONSES_MODEL
AZURE_AI_FOUNDRY_PROJECT_ENDPOINT
AZURE_AI_FOUNDRY_MODEL
AZURE_AI_FOUNDRY_AGENT_NAME
AZURE_AI_FOUNDRY_AGENT_VERSION
AGENT_HARNESS_PROMPT
```

`OPENAI_CHAT_MODEL`, `OPENAI_RESPONSES_MODEL`, and `AZURE_AI_FOUNDRY_AGENT_VERSION` have defaults in the harness. The other variables are required for the corresponding mode.
