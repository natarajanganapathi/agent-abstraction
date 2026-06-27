namespace Harness.Models;

internal sealed record HarnessRunResult(bool Executed, bool Success, string Status, string Message)
{
    public static HarnessRunResult ExecutedSuccessfully(string message) => new(true, true, "Success", message);

    public static HarnessRunResult Skipped(string message) => new(false, true, "Skipped", message);

    public static HarnessRunResult Failed(string message) => new(true, false, "Failed", message);
}
