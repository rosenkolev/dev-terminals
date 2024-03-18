using System;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Loggers.Host;

namespace Dev.Terminals;

/// <summary>Command logger monitor.</summary>
public sealed class TerminalMonitor : CommandMonitor
{
    private readonly CommandLogger _commandLogger;
    private readonly HostOutput _hostOutput;
    private readonly TextOutput _textOutput;
    private readonly bool _trimLines;

    /// <summary>Initializes a new instance of the <see cref="TerminalMonitor"/> class.</summary>
    public TerminalMonitor(CommandLogger commandLogger)
        : this(
              commandLogger,
              new TextOutput(),
              Command.CreateConsoleOutput(commandLogger?.LogLevel ?? LogLevel.Debug))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TerminalMonitor"/> class.</summary>
    public TerminalMonitor(
        CommandLogger commandLogger,
        TextOutput textOutput,
        HostOutput hostOutput,
        bool trimLines = true)
        : base(commandLogger)
    {
        _commandLogger = commandLogger;
        _textOutput = textOutput;
        _hostOutput = hostOutput;
        _trimLines = trimLines;
    }

    /// <summary>Gets the output.</summary>
    public string Output => _textOutput.Logger.Output;

    /// <summary>Gets or sets the log level.</summary>
    public LogLevel LogLevel
    {
        get => _commandLogger.LogLevel;
        set => _commandLogger.LogLevel = value;
    }

    /// <summary>Sets the log level.</summary>
    public void SetLogLevel(LogLevel logLevel) =>
        LogLevel = logLevel;

    /// <summary>Waits for exit result.</summary>
    public string WaitForResult(string endMonitorWildcard, string[] skipLinesWildcards) =>
        WaitForResult(endMonitorWildcard, skipLinesWildcards, WriteLine);

    /// <summary>Writes the host line.</summary>
    public void WriteHostLine(string message, LogLevel logLevel) =>
        _hostOutput?.WriteLine(message, logLevel);

    /// <summary>Resets the output.</summary>
    public void Reset() =>
        _textOutput.Reset();

    private void WriteLine(OutputMessage output)
    {
        var message = _trimLines ? TrimEnd(output.Message, Environment.NewLine) : output.Message;
        _textOutput.WriteLine(message, output.Level);
        _hostOutput?.WriteLine(message, output.Level);
    }
}
