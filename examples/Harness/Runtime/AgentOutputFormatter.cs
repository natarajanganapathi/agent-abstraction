namespace Harness.Runtime;

internal static class AgentOutputFormatter
{
    public static string Format(string title, object? response)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"=== {title} ===");
        builder.AppendLine(ExtractResponseText(response));
        return builder.ToString();
    }

    private static string ExtractResponseText(object? response)
    {
        if (response is null)
        {
            return "<no response object returned>";
        }

        if (TryExtractMessages(response, out var messages))
        {
            var lines = messages
                .Select(ExtractMessageText)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToArray();

            if (lines.Length > 0)
            {
                return string.Join(Environment.NewLine, lines);
            }
        }

        return response.ToString() ?? response.GetType().FullName ?? "<unknown response>";
    }

    private static bool TryExtractMessages(object response, out IReadOnlyList<object> messages)
    {
        messages = [];

        var property = response.GetType().GetProperty("Messages", BindingFlags.Instance | BindingFlags.Public);
        if (property?.GetValue(response) is not IEnumerable enumerable)
        {
            return false;
        }

        messages = enumerable.Cast<object>().ToArray();
        return messages.Count > 0;
    }

    private static string ExtractMessageText(object message)
    {
        if (message is ChatMessage chatMessage)
        {
            return ExtractChatMessageText(chatMessage);
        }

        return message.ToString() ?? string.Empty;
    }

    private static string ExtractChatMessageText(ChatMessage message)
    {
        var textProperty = typeof(ChatMessage).GetProperty("Text", BindingFlags.Instance | BindingFlags.Public);
        if (textProperty?.GetValue(message) is string text && !string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        var contentsProperty = typeof(ChatMessage).GetProperty("Contents", BindingFlags.Instance | BindingFlags.Public);
        if (contentsProperty?.GetValue(message) is IEnumerable contents)
        {
            var contentTexts = contents
                .Cast<object>()
                .Select(content => content.GetType().GetProperty("Text", BindingFlags.Instance | BindingFlags.Public)?.GetValue(content) as string)
                .Where(value => !string.IsNullOrWhiteSpace(value));

            var combined = string.Join(" ", contentTexts);
            if (!string.IsNullOrWhiteSpace(combined))
            {
                return combined;
            }
        }

        return message.ToString() ?? string.Empty;
    }
}
