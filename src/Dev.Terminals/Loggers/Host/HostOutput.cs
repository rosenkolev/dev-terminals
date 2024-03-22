using System;
using System.IO;
using System.Linq;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers.Host;

/// <summary>A host writer class.</summary>
public sealed class HostOutput : IOutput
{
    private readonly LogLevel _maxLogLevel;
    private bool _isLineStart = true;

    /// <summary>Initializes a new instance of the <see cref="HostOutput"/> class.</summary>
    public HostOutput(TextWriter writer, LogLevel maxLogLevel, HostOutputFormatter formatter)
    {
        Writer = writer;
        Formatter = formatter;
        _maxLogLevel = maxLogLevel;
    }

    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    /// <summary>Gets the text writer stream.</summary>
    internal TextWriter Writer { get; init; }

    /// <summary>Gets the text formatter.</summary>
    internal HostOutputFormatter Formatter { get; init; }

    /// <summary>Creates a new instance of the <see cref="HostOutput"/> class.</summary>
    public static HostOutput Create(
        TextWriter writer,
        LogLevel logLevel,
        HostPalette palette,
        string hostPrefix,
        int offsetRation,
        bool noColor) =>
        new HostOutput(
            writer,
            logLevel,
            new HostOutputFormatter(palette, hostPrefix, offsetRation, noColor));

    /// <summary>Writes the specified message.</summary>
    public void Write(string message, LogLevel logLevel)
    {
        if (logLevel > _maxLogLevel ||
            string.IsNullOrEmpty(message) ||
            !Enabled)
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
        if (Enabled)
        {
            Writer.WriteLine();
            _isLineStart = true;
        }
    }

    private static string CleanUp(string message) => message
        .Replace("\u001B[0m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[32m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[39m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[94m", string.Empty, StringComparison.Ordinal)
        .Replace("\u001B[96m", string.Empty, StringComparison.Ordinal);

    private void AppendMessage(string message, LogLevel logLevel) =>
        Writer.Write(Formatter.FormatMessage(message, logLevel));

    private void WriteLineStart(LogLevel logLevel)
    {
        if (_isLineStart)
        {
            Writer.Write(Formatter.GetLinePrefix(logLevel));
            _isLineStart = false;
        }
    }
}
