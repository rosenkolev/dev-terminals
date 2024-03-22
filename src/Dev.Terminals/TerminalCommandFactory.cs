using System;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>The terminal command factory.</summary>
public static class TerminalCommandFactory
{
    /// <summary>Creates the specified command arguments.</summary>
    public static TerminalCommand CreateCommand(
        params string[] arguments) =>
        new TerminalCommand
        {
            CommandArguments = arguments,
            LogLevel = null,
        };

    /// <summary>Parses the specified command.</summary>
    public static TerminalCommand Parse(string command, LogLevel? logLevel, Action<CommandResult>? onComplete) =>
        new TerminalCommand
        {
            CommandArguments = command?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [],
            LogLevel = logLevel,
            OnComplete = onComplete,
        };

    /// <summary>Creates the change directory command.</summary>
    public static TerminalCommand Cd(string workingFolder, LogLevel logLevel = LogLevel.Debug) =>
        new TerminalCommand
        {
            CommandArguments = ["cd", workingFolder],
            LogLevel = logLevel,
        };
}
