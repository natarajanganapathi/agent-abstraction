namespace AiAgents.AzureFoundry.Abstractions.Tests;

internal sealed class FakeTokenCredential : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => new("fake-token", DateTimeOffset.MaxValue);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => new(new AccessToken("fake-token", DateTimeOffset.MaxValue));
}

public class AiAgentFoundryBaseTests
{
    private sealed class TestFoundryAgent : AiAgentFoundryBase<AIProjectClient>
    {
        protected override string AgentName => "TestFoundryAgent";
        protected override string Description => string.Empty;
        protected override StringBuilder Instructions => new("Test Foundry instructions");
        protected override IEnumerable<Delegate> GetTools() => [() => "local-tool"];
    }

    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var exception = Record.Exception(() => new TestFoundryAgent());

        Assert.Null(exception);
    }

    [Fact]
    public void GetAgent_WithAgentReference_ReturnsNonNullAgent()
    {
        var agent = new TestFoundryAgent();
        var aiAgent = agent.GetAgent(new AiAgentFoundryClientOptions
        {
            ModelName = "gpt-4o",
            Endpoint = new Uri("https://fake-endpoint.example.com"),
            Credential = new FakeTokenCredential(),
            AgentRef = new AgentRef("test-agent-from-portal", "1"),
        });

        Assert.NotNull(aiAgent);
    }
}
