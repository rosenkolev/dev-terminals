using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>Terminal execute command.</summary>
public class TerminalCommand
{
    private readonly TerminalCommandExecuteInfo _runInfo;

    /// <summary>Initializes a new instance of the <see cref="TerminalCommand"/> class.</summary>
    public TerminalCommand(string[] commandArguments, LogLevel? logLevel, Action<CommandResult>? onComplete)
        : this(
              new TerminalCommandExecuteInfo
              {
                  CommandArguments = commandArguments,
                  LogLevel = logLevel,
                  OnComplete = onComplete,
              })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TerminalCommand"/> class.</summary>
    public TerminalCommand(TerminalCommandExecuteInfo runInfo)
    {
        if (runInfo == null)
        {
            throw new ArgumentNullException(nameof(runInfo));
        }

        _runInfo = runInfo;
    }

    /// <summary>Gets the execution information.</summary>
    public TerminalCommandExecuteInfo Info => _runInfo;

    /// <summary>Gets the command arguments.</summary>
    public IReadOnlyList<string> CommandArguments =>
        _runInfo.CommandArguments;

    /// <summary>Gets a value indicating whether this instance has next command.</summary>
    [MemberNotNullWhen(true, nameof(Pipe))]
    public bool HasNext => Pipe != null && Pipe.Count > 0;

    private Queue<TerminalCommand>? Pipe { get; set; }

    /// <summary>Adds a command to the pipe.</summary>
    public static TerminalCommand operator &(TerminalCommand first, TerminalCommand second) =>
        BitwiseAnd(first, second);

    /// <summary>Adds a command to the pipe.</summary>
    public static TerminalCommand BitwiseAnd(TerminalCommand left, TerminalCommand right)
    {
        if (left == null)
        {
            throw new ArgumentNullException(nameof(left));
        }

        if (right != null)
        {
            left.Pipe ??= new Queue<TerminalCommand>();
            left.Pipe.Enqueue(right);
        }

        return left;
    }

    /// <summary>Creates the specified command arguments.</summary>
    public static TerminalCommand Create(params string[] commandArguments) =>
        new TerminalCommand(commandArguments, null, null);

    /// <summary>Parses the specified command.</summary>
    public static TerminalCommand CreateParse(string command, LogLevel? logLevel) =>
        CreateParse(command, logLevel, null);

    /// <summary>Parses the specified command.</summary>
    public static TerminalCommand CreateParse(string command, LogLevel? logLevel, Action<CommandResult>? onComplete) =>
        new TerminalCommand(
            command?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [],
            logLevel,
            onComplete);

    /// <summary>Creates the change directory command.</summary>
    public static TerminalCommand Cd(string workingFolder) =>
        new TerminalCommand(
            new[] { "cd", workingFolder },
            LogLevel.Debug,
            null);

    /// <summary>Gets the next command in the pipe.</summary>
    public TerminalCommand? GetNext() => HasNext ? Pipe.Dequeue() : null;
}
