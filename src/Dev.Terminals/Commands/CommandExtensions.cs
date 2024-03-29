﻿using System;
using System.Diagnostics;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Commands;

/// <summary>Command extensions.</summary>
public static class CommandExtensions
{
    /// <summary>Ensures the exit code is not.</summary>
    public static void EnsureExitCodeIs(this Command command, int exitCode)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (command.ExitCode != exitCode)
        {
            throw new ExitCodeException(command.ExitCode, exitCode);
        }
    }

    /// <summary>Validates the command exit code.</summary>
    public static void ThrowOnExitCode(this CommandResult command, int exitCode)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (command.ExitCode != exitCode)
        {
            throw new ExitCodeException(command.ExitCode, exitCode);
        }
    }

    /// <summary>Finds the logger output.</summary>
    /// <typeparam name="T">The type of the output.</typeparam>
    public static T? FindOutput<T>(this ICommandLogger logger)
        where T : class, IOutput
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return logger is CommandLogger commandLogger
            ? (T)commandLogger.Outputs.Find(o => o is T)
            : null;
    }

    public static bool IsRunning(this Process process)
    {
        if (process == null)
        {
            throw new ArgumentNullException(nameof(process));
        }

        try
        {
            Process.GetProcessById(process.Id);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }
}
