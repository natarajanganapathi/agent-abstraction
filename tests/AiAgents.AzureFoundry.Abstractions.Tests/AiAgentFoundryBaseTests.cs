namespace AiAgents.AzureFoundry.Abstractions.Tests;

internal sealed class FakeTokenCredential : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => new("fake-token", DateTimeOffset.MaxValue);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => new(new AccessToken("fake-token", DateTimeOffset.MaxValue));
}

internal sealed class TestFoundryContextProvider : AIContextProvider
{
    public override IReadOnlyList<string> StateKeys => [];
}

internal sealed class TestFoundryChatHistoryProvider : ChatHistoryProvider;

public class AiAgentFoundryBaseTests
{
    private sealed class TestFoundryAgent : AiAgentFoundryBase
    {
        private readonly AIContextProvider[] _contextProviders = [new TestFoundryContextProvider()];
        private readonly ChatHistoryProvider _chatHistoryProvider = new TestFoundryChatHistoryProvider();

        protected override string AgentName => "TestFoundryAgent";
        protected override string Description => string.Empty;
        protected override StringBuilder Instructions => new("Test Foundry instructions");
        protected override IEnumerable<Delegate> GetTools() => [() => "local-tool"];

        protected override IEnumerable<AIContextProvider> GetContextProviders() => _contextProviders;

        protected override ChatHistoryProvider? GetChatHistoryProvider() => _chatHistoryProvider;

        public AIContextProvider ContextProvider => _contextProviders[0];

        public ChatHistoryProvider ChatHistoryProvider => _chatHistoryProvider;
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
    public void GetProjectAgent_AddsContextProvidersAndChatHistoryProvider()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetProjectAgent(
            projectClient,
            new FoundryProjectAgentOptions { ModelName = "gpt-4o" }));
        var contextProviders = Assert.IsAssignableFrom<IReadOnlyList<AIContextProvider>>(chatAgent.AIContextProviders);

        Assert.Single(contextProviders);
        Assert.Same(agent.ContextProvider, contextProviders.Single());
        Assert.Same(agent.ChatHistoryProvider, chatAgent.ChatHistoryProvider);
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
    public void GetVersionedAgent_AddsContextProvidersAndChatHistoryProvider()
    {
        var agent = new TestFoundryAgent();
        var projectClient = new AIProjectClient(new Uri("https://fake-endpoint.example.com"), new FakeTokenCredential());

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetVersionedAgent(projectClient, new FoundryVersionedAgentOptions
        {
            AgentRef = new AgentRef("test-agent-from-portal", "1"),
        }));
        var contextProviders = Assert.IsAssignableFrom<IReadOnlyList<AIContextProvider>>(chatAgent.AIContextProviders);

        Assert.Single(contextProviders);
        Assert.Same(agent.ContextProvider, contextProviders.Single());
        Assert.Same(agent.ChatHistoryProvider, chatAgent.ChatHistoryProvider);
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
