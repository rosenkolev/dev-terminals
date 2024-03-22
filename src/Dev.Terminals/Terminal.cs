using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>A generic terminal control.</summary>
public class Terminal : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Command _command;
    private readonly LogLevel _consoleLogLevel;
    private readonly TerminalMonitor _monitor;
    private readonly int _maxTimeoutInMilliseconds = 3600000; // 1h
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, LogLevel consoleLogLevel, string? workingDirectory)
        : this(syntax, new CommandLogger(consoleLogLevel), workingDirectory)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, CommandLogger commandLogger, string? workingDirectory)
        : this(
              syntax,
              new TerminalMonitor(commandLogger!.LogLevel),
              CreateCommand(syntax, workingDirectory, commandLogger))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
    public Terminal(TerminalCommandSyntax syntax, TerminalMonitor monitor, Command command)
    {
        _monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        _command = command ?? throw new ArgumentNullException(nameof(command));

        Syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));

        _consoleLogLevel = command.Logger.LogLevel;

        _monitor.SetCommandLogger(command.Logger);

        _command.Process.StartInfo.RedirectStandardInput = true;
        _command.Start();
    }

    /// <summary>Gets the command.</summary>
    public Command Command => _command;

    /// <summary>Gets the monitor.</summary>
    public TerminalMonitor Monitor => _monitor;

    /// <summary>Gets or sets the log level.</summary>
    public LogLevel LogLevel
    {
        get => Command.Logger.LogLevel;
        set => Command.Logger.LogLevel = value;
    }

    /// <summary>Gets or sets a value indicating whether host output is enabled.</summary>
    public bool IsHostOutputEnabled
    {
        get => _monitor.HostOutput.Enabled;
        set => _monitor.HostOutput.Enabled = value;
    }

    /// <summary>Gets the terminal syntax.</summary>
    public TerminalCommandSyntax Syntax { get; init; }

    /// <summary>Gets the current directory.</summary>
    public string CurrentDirectory =>
        ExecuteCommand(
            TerminalCommandFactory.Parse("echo " + Syntax.CurrentDirectoryCodeCommand, LogLevel.Debug, null))
        .Output
        .Trim();

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

    /// <summary>Executes the specified command in an async task.</summary>
    public Task<CommandResult> ExecuteAsync(TerminalCommand command) =>
        Task.Run(() => Execute(command));

    /// <summary>Executes the specified command.</summary>
    public CommandResult Execute(TerminalCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        _semaphore.Wait(_maxTimeoutInMilliseconds);

        CommandResult? result = null;
        try
        {
            if (!command.HasNext)
            {
                result = ExecuteCommand(command);
            }
            else
            {
                var builder = new StringBuilder();
                var currentCommand = command;
                do
                {
                    if (result != null)
                    {
                        builder.AppendLine();
                    }

                    result = ExecuteCommand(currentCommand);
                    builder.Append(result.Output);
                    currentCommand = currentCommand.MoveNextAndReleasePointers();
                }
                while (result.ExitCode == 0 && currentCommand != null);

                result = new CommandResult(builder.ToString(), result.ExitCode);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return result;
    }

    /// <summary>Closes this terminal.</summary>
    public void Close() =>
        Dispose();

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Executes the command.</summary>
    internal CommandResult ExecuteCommand(
        TerminalCommand context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        LogLevel = context.LogLevel ?? LogLevel;

        var rawCommandText = context.RawInput ?
            string.Join(" ", context.CommandArguments) :
            Syntax.BuildCommand(context.CommandArguments);

        var result = ExecuteRunUnit(new TerminalCommandRunUnit(rawCommandText));

        LogLevel = _consoleLogLevel;

        context.OnComplete?.Invoke(result);

        return result;
    }

    /// <summary>Runs a single command in the terminal.</summary>
    internal CommandResult ExecuteRunUnit(TerminalCommandRunUnit execution)
    {
        _monitor.WriteHostLine(execution.Command, LogLevel.Debug);

        var prefix = execution.Prefix;
        var statusCodeCommand = "echo " + prefix + Syntax.ReturnCodeCommand;

        _command.Process.StandardInput.WriteLine(execution.Command);
        _command.Process.StandardInput.WriteLine(statusCodeCommand);

        var skipLines = Syntax.BuildInputClearWildCards(execution.Command, statusCodeCommand).ToArray();
        var outputResult = _monitor.WaitForResult(prefix + '*', skipLines);
        var code = outputResult[prefix.Length..];
        var statusCode = Convert.ToInt32(code, CultureInfo.InvariantCulture);
        var output = _monitor.Output.Trim(' ', '\r', '\n');
        var result = new CommandResult(output, statusCode);

        _monitor.WriteHostLine("Exit code " + code, LogLevel.Debug);
        _monitor.Reset();

        return result;
    }

    /// <summary>Dispose the resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _command.Dispose();
                _semaphore.Dispose();
            }

            _disposed = true;
        }
    }
}
