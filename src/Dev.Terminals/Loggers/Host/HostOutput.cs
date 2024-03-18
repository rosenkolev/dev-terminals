using System;
using System.IO;
using System.Linq;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers.Host;

/// <summary>A host writer class.</summary>
public sealed class HostOutput : IOutput
{
    private readonly TextWriter _writer;
    private readonly LogLevel _maxLogLevel;
    private readonly HostOutputFormatter _formatter;
    private bool _isLineStart = true;

    /// <summary>Initializes a new instance of the <see cref="HostOutput"/> class.</summary>
    public HostOutput(TextWriter writer, HostPalette palette, string prefix, LogLevel maxLogLevel)
        : this(writer, maxLogLevel, new HostOutputFormatter(palette, prefix))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="HostOutput"/> class.</summary>
    public HostOutput(TextWriter writer, LogLevel maxLogLevel, HostOutputFormatter formatter)
    {
        _writer = writer;
        _maxLogLevel = maxLogLevel;
        _formatter = formatter;
    }

    /// <summary>Writes the specified message.</summary>
    public void Write(string message, LogLevel logLevel)
    {
        if (logLevel > _maxLogLevel ||
            string.IsNullOrEmpty(message))
        {
            return;
        }

        var msgs = CleanUp(message)
            .Split(Environment.NewLine)
            .ToArray();

        var lastLineIndex = msgs.Length - 1;
        for (var index = 0; index < msgs.Length; index++)
        {
            WriteLineStart(logLevel);
            AppendMessage(msgs[index], logLevel);
            if (msgs.Length > 1 && index < lastLineIndex)
            {
                WriteLine();
            }
        }
    }

    /// <summary>Writes the specified message with new line.</summary>
    public void WriteLine(string message, LogLevel logLevel)
    {
        if (logLevel <= _maxLogLevel)
        {
            Write(message, logLevel);
            WriteLine();
        }
    }

    /// <summary>Writes a new line.</summary>
    public void WriteLine()
    {
        _writer.WriteLine();
        _isLineStart = true;
    }

    private static string CleanUp(string message) => message
        .Replace("\u001B[0m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[32m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[39m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[94m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[96m", string.Empty, StringComparison.Ordinal);

    private void AppendMessage(string message, LogLevel logLevel) =>
        _writer.Write(_formatter.FormatMessage(message, logLevel));

    private void WriteLineStart(LogLevel logLevel)
    {
        if (_isLineStart)
        {
            _writer.Write(_formatter.GetLinePrefix(logLevel));
            _isLineStart = false;
        }
    }
}
