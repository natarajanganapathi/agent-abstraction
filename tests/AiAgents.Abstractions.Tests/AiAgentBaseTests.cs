namespace AiAgents.Abstractions.Tests;

internal sealed class FakeChatClient : IChatClient
{
    public void Dispose() { }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ChatResponse(
            new ChatMessage(ChatRole.Assistant, "ok")));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return AsyncEnumerable.Empty<ChatResponseUpdate>();
    }

    public object? GetService(Type serviceType, object? serviceKey) => null;
}

internal sealed class WrappedChatClient(IChatClient innerClient) : IChatClient
{
    public IChatClient InnerClient { get; } = innerClient;

    public void Dispose() => InnerClient.Dispose();

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => InnerClient.GetResponseAsync(messages, options, cancellationToken);

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => InnerClient.GetStreamingResponseAsync(messages, options, cancellationToken);

    public object? GetService(Type serviceType, object? serviceKey)
        => InnerClient.GetService(serviceType, serviceKey);
}

internal sealed class TestService;

internal sealed class SingleServiceProvider(object service) : IServiceProvider
{
    public object Service { get; } = service;

    public object? GetService(Type serviceType)
        => serviceType.IsInstanceOfType(Service) ? Service : null;
}

internal sealed class TestWeatherAgent : AiAgentBase
{
    protected override string AgentName => "TestWeatherAgent";
    protected override string Description => string.Empty;
    protected override StringBuilder Instructions => new("You are a weather assistant.");
    protected override IEnumerable<Delegate> GetTools() => [() => "sunny"];
}

internal sealed class TestNoToolsAgent : AiAgentBase
{
    protected override string AgentName => "TestNoToolsAgent";
    protected override string Description => string.Empty;
    protected override StringBuilder Instructions => new("You are a simple assistant.");
}

internal sealed class TestDescribedAgent : AiAgentBase
{
    protected override string AgentName => "TestDescribedAgent";
    protected override string Description => "A helpful weather agent that provides forecasts.";
    protected override StringBuilder Instructions => new("You are a described assistant.");
}

public class AiAgentBaseTests
{
    private static int GetToolCount(object agent)
    {
        var chatOptionsProperty = agent.GetType().GetProperty(
            "ChatOptions",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        var chatOptions = chatOptionsProperty?.GetValue(agent);
        var toolsProperty = chatOptions?.GetType().GetProperty("Tools");
        var tools = toolsProperty?.GetValue(chatOptions) as System.Collections.ICollection;

        return tools?.Count ?? 0;
    }

    private static bool ContainsChatClientType(IChatClient chatClient, Type targetType)
        => ContainsChatClientType(chatClient, targetType, []);

    private static bool ContainsChatClientType(IChatClient chatClient, Type targetType, HashSet<object> visited)
    {
        if (!visited.Add(chatClient))
            return false;

        if (targetType.IsInstanceOfType(chatClient))
            return true;

        var members = chatClient.GetType()
            .GetMembers(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        foreach (var member in members)
        {
            object? value = member switch
            {
                System.Reflection.FieldInfo field when typeof(IChatClient).IsAssignableFrom(field.FieldType) => field.GetValue(chatClient),
                System.Reflection.PropertyInfo property when typeof(IChatClient).IsAssignableFrom(property.PropertyType) && property.GetIndexParameters().Length == 0 => property.GetValue(chatClient),
                _ => null,
            };

            if (value is IChatClient innerClient && ContainsChatClientType(innerClient, targetType, visited))
                return true;
        }

        return false;
    }

    [Fact]
    public void GetChatAgent_ReturnsChatClientAgent()
    {
        var agent = new TestWeatherAgent();

        var aiAgent = agent.GetChatAgent(new FakeChatClient());

        Assert.IsType<ChatClientAgent>(aiAgent);
    }

    [Fact]
    public void GetChatAgent_SetsInstructions()
    {
        var agent = new TestWeatherAgent();

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient()));

        Assert.Equal("You are a weather assistant.", chatAgent.Instructions);
    }

    [Fact]
    public void GetChatAgent_SetsNameFromClassName()
    {
        var agent = new TestWeatherAgent();

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient()));

        Assert.Equal("TestWeatherAgent", chatAgent.Name);
    }

    [Fact]
    public void GetChatAgent_WithNoTools_DoesNotThrow()
    {
        var agent = new TestNoToolsAgent();

        var exception = Record.Exception(() => agent.GetChatAgent(new FakeChatClient()));

        Assert.Null(exception);
    }

    [Fact]
    public void GetChatAgent_DescriptionDefaultsToEmptyString()
    {
        var agent = new TestWeatherAgent();

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient()));

        Assert.Equal(string.Empty, chatAgent.Description);
    }

    [Fact]
    public void GetChatAgent_SetsDescriptionWhenOverridden()
    {
        var agent = new TestDescribedAgent();

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient()));

        Assert.Equal("A helpful weather agent that provides forecasts.", chatAgent.Description);
    }

    [Fact]
    public void GetChatAgent_AcceptsClientFactoryAndServices()
    {
        var agent = new TestNoToolsAgent();
        var service = new TestService();
        var serviceProvider = new SingleServiceProvider(service);
        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient(), new ChatAgentOptions
        {
            ClientFactory = client => new WrappedChatClient(client),
            LoggerFactory = Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance,
            Services = serviceProvider,
        }));

        Assert.True(ContainsChatClientType(chatAgent.ChatClient, typeof(WrappedChatClient)));
    }

    [Fact]
    public void GetChatAgent_AddsToolsToChatOptions()
    {
        var agent = new TestWeatherAgent();

        var chatAgent = Assert.IsType<ChatClientAgent>(agent.GetChatAgent(new FakeChatClient()));

        Assert.Equal(1, GetToolCount(chatAgent));
    }

    [Fact]
    public void GetChatAgent_WithNullClient_ThrowsArgumentNullException()
    {
        var agent = new TestNoToolsAgent();

        var exception = Assert.Throws<ArgumentNullException>(() => agent.GetChatAgent((IChatClient)null!));

        Assert.Equal("client", exception.ParamName);
    }

    [Fact]
    public void GetResponsesAgent_ReturnsNonNullAgent()
    {
        var agent = new TestWeatherAgent();

        var aiAgent = agent.GetResponsesAgent(
            new ResponsesClient("test-api-key"),
            new ResponsesAgentOptions { ModelName = "gpt-4o" });

        Assert.NotNull(aiAgent);
    }

    [Fact]
    public void GetResponsesAgent_WithNullClient_ThrowsArgumentNullException()
    {
        var agent = new TestNoToolsAgent();

        var exception = Assert.Throws<ArgumentNullException>(() => agent.GetResponsesAgent((ResponsesClient)null!, new ResponsesAgentOptions { ModelName = "gpt-4o" }));

        Assert.Equal("client", exception.ParamName);
    }

    [Fact]
    public void GetResponsesAgent_WithNullOptions_ThrowsArgumentNullException()
    {
        var agent = new TestNoToolsAgent();

        var exception = Assert.Throws<ArgumentNullException>(() => agent.GetResponsesAgent(new ResponsesClient("test-api-key"), null!));

        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void GetResponsesAgent_WithEmptyModelName_ThrowsArgumentException()
    {
        var agent = new TestNoToolsAgent();

        var exception = Assert.Throws<ArgumentException>(() => agent.GetResponsesAgent(
            new ResponsesClient("test-api-key"),
            new ResponsesAgentOptions { ModelName = " " }));

        Assert.Equal("ModelName", exception.ParamName);
    }
}
