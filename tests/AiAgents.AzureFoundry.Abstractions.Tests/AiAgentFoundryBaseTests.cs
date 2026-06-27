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
    private sealed class TestFoundryAgent : AiAgentFoundryBase
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
    public void GetProjectAgent_ReturnsNonNullAgent()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());
        var aiAgent = agent.GetProjectAgent(projectClient, new FoundryProjectAgentOptions { ModelName = "gpt-4o" });


        Assert.NotNull(aiAgent);
    }

    [Fact]
    public void GetProjectAgent_WithClientOptions_ReturnsNonNullAgent()
    {
        var agent = new TestFoundryAgent();
        var aiAgent = agent.GetProjectAgent(
            new FoundryClientOptions
            {
                Endpoint = new Uri("https://fake-endpoint.example.com"),
                Credential = new FakeTokenCredential(),
            },
            new FoundryProjectAgentOptions { ModelName = "gpt-4o" });

        Assert.NotNull(aiAgent);
    }

    [Fact]
    public void GetVersionedAgent_ReturnsNonNullAgent()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());
        var aiAgent = agent.GetVersionedAgent(projectClient, new FoundryVersionedAgentOptions
        {
            AgentRef = new AgentRef("test-agent-from-portal", "1"),
        });

        Assert.NotNull(aiAgent);
    }

    [Fact]
    public void GetVersionedAgent_WithClientOptions_ReturnsNonNullAgent()
    {
        var agent = new TestFoundryAgent();
        var aiAgent = agent.GetVersionedAgent(
            new FoundryClientOptions
            {
                Endpoint = new Uri("https://fake-endpoint.example.com"),
                Credential = new FakeTokenCredential(),
            },
            new FoundryVersionedAgentOptions
            {
                AgentRef = new AgentRef("test-agent-from-portal", "1"),
            });

        Assert.NotNull(aiAgent);
    }

    [Fact]
    public void GetProjectAgent_WithEmptyModelName_ThrowsArgumentException()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());

        var exception = Assert.Throws<ArgumentException>(() => agent.GetProjectAgent(
            projectClient,
            new FoundryProjectAgentOptions { ModelName = " " }));

        Assert.Equal("ModelName", exception.ParamName);
    }

    [Fact]
    public void GetVersionedAgent_WithNullAgentRef_ThrowsArgumentNullException()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());

        var exception = Assert.Throws<ArgumentNullException>(() => agent.GetVersionedAgent(
            projectClient,
            new FoundryVersionedAgentOptions { AgentRef = null! }));

        Assert.Equal("AgentRef", exception.ParamName);
    }

    [Fact]
    public void GetVersionedAgent_WithEmptyAgentRefName_ThrowsArgumentException()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());

        var exception = Assert.Throws<ArgumentException>(() => agent.GetVersionedAgent(
            projectClient,
            new FoundryVersionedAgentOptions { AgentRef = new AgentRef(" ", "1") }));

        Assert.Equal("AgentRef.Name", exception.ParamName);
    }
}
