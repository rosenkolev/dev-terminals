using System;
using System.Diagnostics.CodeAnalysis;

using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Commands;

/// <summary>A command output monitor.</summary>
public class CommandMonitor
{
    private readonly ChannelOutput _channelOutput;

    /// <summary>Initializes a new instance of the <see cref="CommandMonitor"/> class.</summary>
    public CommandMonitor(ICommandLogger commandLogger)
    {
        if (commandLogger == null)
        {
            throw new ArgumentNullException(nameof(commandLogger));
        }

        _channelOutput = new ChannelOutput();
        commandLogger.Add(_channelOutput);
    }

    /// <summary>Waits for exit result.</summary>
    public string WaitForResult(
        string endMonitorWildcard,
        string[] skipLinesWildcards,
        Action<OutputMessage> onRead)
    {
        if (string.IsNullOrEmpty(endMonitorWildcard))
        {
            throw new ArgumentException("End monitoring wildcard is required", nameof(endMonitorWildcard));
        }

        OutputMessage output;
        string message;
        do
        {
            output = _channelOutput.WaitAndRead();
            message = TrimEnd(output.Message, Environment.NewLine);
            if (string.IsNullOrEmpty(message))
            {
                continue;
            }

            if (MatchWildCard(message, endMonitorWildcard))
            {
                break;
            }

            if (skipLinesWildcards != null &&
                Array.Exists(skipLinesWildcards, card => MatchWildCard(message, card)))
            {
                continue;
            }

            onRead?.Invoke(output);
        }
        while (true);

        return message;
    }

    /// <summary>Trim a string from the end.</summary>
    protected static string TrimEnd([NotNull] string input, [NotNull] string value) =>
            input.EndsWith(value, StringComparison.InvariantCulture) ?
            input[..^value.Length] :
            input;

    private static bool MatchWildCard(string input, string pattern)
    {
        if (pattern[0] == '*')
        {
            return input.EndsWith(pattern[1..], StringComparison.InvariantCultureIgnoreCase);
        }

        var lastIndex = pattern.Length - 1;
        if (pattern[lastIndex] == '*')
        {
            return input.StartsWith(pattern[..lastIndex], StringComparison.InvariantCultureIgnoreCase);
        }

        return input.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
}
