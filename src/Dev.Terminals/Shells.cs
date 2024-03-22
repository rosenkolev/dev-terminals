using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Syntax;

using static Dev.Terminals.TerminalCommandFactory;

namespace Dev.Terminals;

/// <summary>A facade for terminal operations.</summary>
public static class Shells
{
    private static Terminal? _defaultInstance;

    /// <summary>Gets the default terminal syntax.</summary>
    public static TerminalCommandSyntax DefaultTerminalSyntax =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            new WindowsCmdSyntax() :
            new UnixShSyntax();

    /// <summary>Gets or sets the default terminal.</summary>
    public static Terminal DefaultTerminal
    {
        get
        {
            _defaultInstance ??= CreateDefaultTerminal();
            return _defaultInstance;
        }

        set
        {
            _defaultInstance?.Close();
            _defaultInstance = value;
        }
    }

    /// <summary>Execute a shell command.</summary>
    public static CommandResult Shell(string command) =>
        DefaultTerminal.Execute(Parse(command, logLevel: null, onComplete: null));

    /// <summary>Execute a shell command.</summary>
    public static CommandResult Shell(string command, ShellOptions options)
    {
        if (options == null)
        {
            return Shell(command);
        }

        var currentIsHostOutputEnabled = DefaultTerminal.IsHostOutputEnabled;
        string? currentWorkingDirectory = null;

        DefaultTerminal.IsHostOutputEnabled = options.AutoHostOutput;
        if (options.WorkingDirectory != null)
        {
            currentWorkingDirectory = DefaultTerminal.CurrentDirectory;
            Shell(Cd(options.WorkingDirectory));
        }

        try
        {
            return Shell(Parse(command, options.LogLevel, null));
        }
        finally
        {
            DefaultTerminal.IsHostOutputEnabled = currentIsHostOutputEnabled;
            SafeCd(currentWorkingDirectory!);
        }
    }

    /// <summary>Execute a shell command.</summary>
    public static CommandResult Shell(TerminalCommand command) =>
        DefaultTerminal.Execute(command);

    /// <summary>Creates the default terminal.</summary>
    public static Terminal CreateDefaultTerminal(
        LogLevel logLevel = LogLevel.Info,
        string? workingDirectory = null,
        string logPrefix = "",
        bool noColor = false,
        Dictionary<string, string>? environmentVariables = null) =>
        CreateTerminal(DefaultTerminalSyntax, logLevel, workingDirectory, logPrefix, noColor, environmentVariables);

    /// <summary>Creates a terminal.</summary>
    public static Terminal CreateTerminal(
        TerminalCommandSyntax terminalSyntax,
        LogLevel logLevel = LogLevel.Info,
        string? workingDirectory = null,
        string logPrefix = "",
        bool noColor = false,
        Dictionary<string, string>? environmentVariables = null)
    {
        if (terminalSyntax == null)
        {
            throw new ArgumentNullException(nameof(terminalSyntax));
        }

        var logger = new CommandLogger(logLevel);
        var consoleOutput = Command.CreateConsoleOutput(logLevel, logPrefix, 1, noColor);
        var monitor = new TerminalMonitor(new TextOutput(), consoleOutput);
        var processStartInfo = ProcessStartInfoFactory.Create(
            terminalSyntax.CommandName,
            string.Empty,
            workingDirectory);

        if (environmentVariables != null && environmentVariables.Count > 0)
        {
            foreach (var (name, value) in environmentVariables)
            {
                processStartInfo.EnvironmentVariables.Add(name, value);
            }
        }

        var command = new Command(processStartInfo, logger);
        var terminal = new Terminal(terminalSyntax, monitor, command);

        return terminal;
    }

    private static void SafeCd(string? workingDirectory)
    {
        if (workingDirectory != null)
        {
            DefaultTerminal.Execute(Cd(workingDirectory));
        }
    }
}
