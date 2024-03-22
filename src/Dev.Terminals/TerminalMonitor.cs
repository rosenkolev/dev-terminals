using System;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Loggers.Host;

namespace Dev.Terminals;

/// <summary>Command logger monitor.</summary>
/// <remarks>The terminal monitor implements the Decorator Design Pattern.</remarks>
public sealed class TerminalMonitor
{
    private readonly ChannelOutput _channelOutput = new();
    private readonly HostOutput _hostOutput;
    private readonly TextOutput _textOutput;
    private readonly bool _trimLines;

    /// <summary>Initializes a new instance of the <see cref="TerminalMonitor"/> class.</summary>
    public TerminalMonitor(LogLevel hostLogLevel)
        : this(
              new TextOutput(),
              Command.CreateConsoleOutput(hostLogLevel))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TerminalMonitor"/> class.</summary>
    public TerminalMonitor(
        TextOutput textOutput,
        HostOutput hostOutput,
        bool trimLines = true)
    {
        _textOutput = textOutput;
        _hostOutput = hostOutput;
        _trimLines = trimLines;
    }

    /// <summary>Gets the output.</summary>
    public string Output => _textOutput.Logger.Output;

    /// <summary>Gets the host output.</summary>
    public HostOutput HostOutput => _hostOutput;

    /// <summary>Sets the command logger.</summary>
    public void SetCommandLogger(ICommandLogger commandLogger)
    {
        if (commandLogger == null)
        {
            throw new ArgumentNullException(nameof(commandLogger));
        }

        commandLogger.Add(_channelOutput);
    }

    /// <summary>Waits for exit result.</summary>
    public string WaitForResult(string endMonitorWildcard, string[] skipLinesWildcards) =>
        _channelOutput.WaitForWildcard(endMonitorWildcard, skipLinesWildcards, WriteLine);

    /// <summary>Writes the host line.</summary>
    public void WriteHostLine(string message, LogLevel logLevel) =>
        _hostOutput?.WriteLine(message, logLevel);

    /// <summary>Resets the output.</summary>
    public void Reset() =>
        _textOutput.Reset();

    private void WriteLine(OutputMessage output)
    {
        var message = _trimLines ? output.Message.TrimEnd(Environment.NewLine) : output.Message;
        _textOutput.WriteLine(message, output.Level);
        _hostOutput?.WriteLine(message, output.Level);
    }
}
