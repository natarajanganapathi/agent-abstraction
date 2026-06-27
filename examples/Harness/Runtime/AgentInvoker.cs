namespace Harness.Runtime;

internal static class AgentInvoker
{
    public static async Task<object?> RunAsync(AIAgent agent, string prompt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var invocation = TryCreateInvocation(agent, prompt, cancellationToken);
        if (invocation is null)
        {
            throw new InvalidOperationException($"Could not find a supported RunAsync overload on '{agent.GetType().FullName}'.");
        }

        return await InvokeAsync(invocation.Value.Method, agent, invocation.Value.Arguments);
    }

    private static (MethodInfo Method, object?[] Arguments)? TryCreateInvocation(
        AIAgent agent,
        string prompt,
        CancellationToken cancellationToken)
    {
        foreach (var method in agent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!string.Equals(method.Name, nameof(AIAgent.RunAsync), StringComparison.Ordinal) || method.GetParameters().Length != 4)
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters[0].ParameterType == typeof(string))
            {
                return (method, [prompt, null, null, cancellationToken]);
            }

            if (typeof(IEnumerable<ChatMessage>).IsAssignableFrom(parameters[0].ParameterType))
            {
                return (method, [new[] { new ChatMessage(ChatRole.User, prompt) }, null, null, cancellationToken]);
            }
        }

        return null;
    }

    private static async Task<object?> InvokeAsync(MethodInfo method, object target, object?[] arguments)
    {
        var result = method.Invoke(target, arguments);
        if (result is not Task task)
        {
            return result;
        }

        await task.ConfigureAwait(false);
        return task.GetType().GetProperty("Result")?.GetValue(task);
    }
}
