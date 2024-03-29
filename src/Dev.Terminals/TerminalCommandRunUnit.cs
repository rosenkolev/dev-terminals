﻿namespace Dev.Terminals;

/// <summary>A terminal execution.</summary>
internal class TerminalCommandRunUnit
{
    private static uint globalIndex;

    /// <summary>Initializes a new instance of the <see cref="TerminalCommandRunUnit"/> class.</summary>
    public TerminalCommandRunUnit(string command)
    {
#pragma warning disable S3010
        Id = globalIndex++;
#pragma warning restore S3010
        Command = command;
    }

    /// <summary>Gets the identifier.</summary>
    public uint Id { get; init; }

    /// <summary>Gets the command.</summary>
    public string Command { get; init; }

    /// <summary>Gets the prefix.</summary>
    public string Prefix => $"@@{Id}@";
}
