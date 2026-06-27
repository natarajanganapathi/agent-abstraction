namespace Harness.Models;

internal static class HarnessModes
{
    public const string AllMode = "all";
    public const string Chat = "chat";
    public const string Responses = "responses";
    public const string FoundryProject = "foundry-project";
    public const string FoundryVersioned = "foundry-versioned";

    public static IReadOnlySet<string> All { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        AllMode,
        Chat,
        Responses,
        FoundryProject,
        FoundryVersioned,
    };

    public static IEnumerable<string> Resolve(string mode)
    {
        return string.Equals(mode, AllMode, StringComparison.OrdinalIgnoreCase)
            ? [Chat, Responses, FoundryProject, FoundryVersioned]
            : [mode];
    }
}
