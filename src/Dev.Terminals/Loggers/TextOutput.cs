using System;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers;

/// <summary>A string logger.</summary>
public sealed class TextOutput : IOutput
{
    /// <summary>Gets the logger.</summary>
    public TextOutputLogger Logger { get; private set; } = new TextOutputLogger();

    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    /// <inheritdoc/>
    public void Write(string message, LogLevel logLevel)
    {
        if (Enabled)
        {
            if (logLevel == LogLevel.Error)
            {
                Logger.LogError(message);
            }
            else
            {
                Logger.LogMessage(message);
            }
        }
    }

    /// <inheritdoc/>
    public void WriteLine() =>
        Write(Environment.NewLine, LogLevel.Message);

    /// <inheritdoc/>
    public void WriteLine(string message, LogLevel logLevel) =>
        Write(message + Environment.NewLine, logLevel);

    /// <summary>Resets this output.</summary>
    public void Reset() =>
        Logger = new TextOutputLogger();
}
