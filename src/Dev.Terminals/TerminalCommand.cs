using System;
using System.Diagnostics.CodeAnalysis;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>The terminal command execution information.</summary>
public sealed class TerminalCommand
{
    /// <summary>Gets the command arguments.</summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By Design")]
    public required string[] CommandArguments { get; init; }

    /// <summary>Gets the log level.</summary>
    public required LogLevel? LogLevel { get; init; }

    /// <summary>Gets the on complete action.</summary>
    public Action<CommandResult>? OnComplete { get; init; }

    /// <summary>
    /// Gets a value indicating whether to use input is passed raw,
    /// without transforming it to the shell syntax.
    /// </summary>
    public bool RawInput { get; init; }

    /// <summary>Gets a value indicating whether this instance has next command.</summary>
    [MemberNotNullWhen(true, nameof(Next))]
    public bool HasNext => Next != null;

    /// <summary>Gets the previous command in the pipe.</summary>
    internal TerminalCommand? Previous { get; private set; }

    /// <summary>Gets the next command in the pipe.</summary>
    internal TerminalCommand? Next { get; private set; }

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

        if (right == null)
        {
            throw new ArgumentNullException(nameof(right));
        }

        if (right.Previous != null)
        {
            throw new InvalidOperationException("The command already have a next command");
        }

        var last = left;
        while (last.HasNext)
        {
            last = last.Next!;
        }

        last.Next = right;
        right.Previous = last;
        return left;
    }

    /// <summary>Move to the next command and release the pointer references.</summary>
    internal TerminalCommand? MoveNextAndReleasePointers()
    {
        if (HasNext)
        {
            var nextCommand = Next;
            Next = null;
            nextCommand.Previous = null;
            return nextCommand;
        }

        return null;
    }
}
