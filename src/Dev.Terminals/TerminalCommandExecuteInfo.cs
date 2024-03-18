using System;
using System.Diagnostics.CodeAnalysis;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>The terminal command execution information.</summary>
public sealed class TerminalCommandExecuteInfo
{
    /// <summary>Gets the command arguments.</summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By Design")]
    public required string[] CommandArguments { get; init; }

    /// <summary>Gets the log level.</summary>
    public required LogLevel? LogLevel { get; init; }

    /// <summary>Gets the on complete action.</summary>
    public Action<CommandResult>? OnComplete { get; init; }
}
