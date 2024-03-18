using System.Text;

namespace Dev.Terminals.Loggers;

/// <summary>A scope logger.</summary>
public sealed class TextOutputLogger
{
    private readonly StringBuilder _lastStandardOutput = new StringBuilder();
    private readonly StringBuilder _lastStandardError = new StringBuilder();

    /// <summary>Gets the error.</summary>
    public string Error => _lastStandardError.ToString().Trim();

    /// <summary>Gets the output.</summary>
    public string Output => _lastStandardOutput.ToString().Trim();

    /// <summary>Logs the error.</summary>
    public void LogError(string message) => _lastStandardError.Append(message);

    /// <summary>Logs the message.</summary>
    public void LogMessage(string message) => _lastStandardOutput.Append(message);
}
