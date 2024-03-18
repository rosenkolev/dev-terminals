using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevOps.Commands;
using DevOps.Loggers.Abstraction;

namespace DevOps.Terminals;

/// <summary>A generic terminal control.</summary>
public class Terminal
{
    private readonly Command _command;
    private readonly LogLevel _consoleLogLevel;
    private readonly TerminalMonitor _monitor;
    private readonly TerminalCommandSyntax _syntax;

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, LogLevel consoleLogLevel, string? workingDirectory)
        : this(syntax, new CommandLogger(consoleLogLevel), workingDirectory)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, CommandLogger commandLogger, string? workingDirectory)
        : this(
              syntax,
              new TerminalMonitor(commandLogger),
              CreateCommand(syntax, workingDirectory, commandLogger))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, TerminalMonitor monitor, Command command)
    {
        _syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
        _monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        _command = command ?? throw new ArgumentNullException(nameof(command));

        _consoleLogLevel = monitor.LogLevel;

        _command.Process.StartInfo.RedirectStandardInput = true;
        _command.Start();
    }

    /// <summary>Gets the command.</summary>
    public Command Command => _command;

    /// <summary>Gets the monitor.</summary>
    public TerminalMonitor Monitor => _monitor;

    /// <summary>Creates a command.</summary>
    public static Command CreateCommand(
        TerminalCommandSyntax syntax,
        string? workingDirectory,
        CommandLogger logger)
    {
        if (syntax == null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        return new Command(
            commandPath: syntax.CommandName,
            arguments: string.Empty,
            workingDirectory: workingDirectory ?? Directory.GetCurrentDirectory(),
            logger);
    }

    /// <summary>Executes the specified command.</summary>
    public CommandResult Exec(TerminalCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return command.HasNext
            ? ExecPipe(command)
            : ExecuteCommand(command.Info);
    }

    /// <summary>Executes the specified command in an async task.</summary>
    public Task<CommandResult> ExecAsync(TerminalCommand command) =>
        Task.Run(() => Exec(command));

    /// <summary>Executes the specified command.</summary>
    public CommandResult ExecPipe(TerminalCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var builder = new StringBuilder();
        CommandResult? result = null;
        var currentCommand = command;
        do
        {
            if (result != null)
            {
                builder.AppendLine();
            }

            result = ExecuteCommand(currentCommand!.Info);
            builder.Append(result.Output);
            currentCommand = command.GetNext();
        }
        while (result.ExitCode == 0 && currentCommand != null);

        return new CommandResult(builder.ToString(), result.ExitCode);
    }

    /// <summary>Closes this terminal.</summary>
    public void Close()
    {
        _command.Dispose();
    }

    /// <summary>Executes the command.</summary>
    public CommandResult ExecuteCommand(
        string[] arguments,
        LogLevel? logLevel,
        bool raw = false) =>
        ExecuteCommand(new TerminalCommandExecuteInfo { CommandArguments = arguments, LogLevel = logLevel }, raw);

    /// <summary>Executes the command.</summary>
    public CommandResult ExecuteCommand(
        TerminalCommandExecuteInfo info,
        bool raw = false)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        _monitor.SetLogLevel(info.LogLevel ?? _command.Logger.LogLevel);

        var result = raw ?
            Execute(new TerminalExecution(string.Join(" ", info.CommandArguments))) :
            Execute(info.CommandArguments);

        _monitor.Reset();
        _monitor.SetLogLevel(_consoleLogLevel);

        info.OnComplete?.Invoke(result);

        return result;
    }

    /// <summary>Executes the command.</summary>
    internal CommandResult Execute(params string[] commandArguments)
    {
        var commandText = _syntax.BuildCommand(commandArguments);
        return Execute(new TerminalExecution(commandText));
    }

    /// <summary>Executes the command.</summary>
    internal CommandResult Execute(TerminalExecution execution)
    {
        _monitor.WriteHostLine(execution.Command, LogLevel.Debug);

        var prefix = execution.Prefix;
        var statusCodeCommand = "echo " + prefix + _syntax.ReturnCodeCommand;

        _command.Process.StandardInput.WriteLine(execution.Command);
        _command.Process.StandardInput.WriteLine(statusCodeCommand);

        var skipLines = _syntax.BuildInputClearWildCards(execution.Command, statusCodeCommand).ToArray();
        var outputResult = _monitor.WaitForResult(prefix + '*', skipLines);
        var code = outputResult[prefix.Length..];
        var statusCode = Convert.ToInt32(code, CultureInfo.InvariantCulture);
        var output = _monitor.Output.Trim(' ', '\r', '\n');
        var result = new CommandResult(output, statusCode);

        _monitor.WriteHostLine("Exit code " + code, LogLevel.Debug);

        return result;
    }
}
