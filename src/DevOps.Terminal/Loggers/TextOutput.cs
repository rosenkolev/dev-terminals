using System;

using DevOps.Loggers.Abstraction;

namespace DevOps.Loggers;

/// <summary>A string logger.</summary>
public sealed class TextOutput : IOutput
{
    /// <summary>Gets the logger.</summary>
    public TextOutputLogger Logger { get; private set; } = new TextOutputLogger();

    /// <inheritdoc/>
    public void Write(string message, LogLevel logLevel)
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
