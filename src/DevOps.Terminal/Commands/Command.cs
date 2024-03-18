using System;
using System.Diagnostics;

using DevOps.Loggers;
using DevOps.Loggers.Abstraction;
using DevOps.Loggers.Host;

namespace DevOps.Commands;

/// <summary>An external command.</summary>
public class Command : IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
    public Command(string commandPath, string arguments, string? workingDirectory, ICommandLogger logger)
        : this(ProcessStartInfoFactory.Create(commandPath, arguments, workingDirectory), logger)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
    public Command(ProcessStartInfo startInfo, ICommandLogger logger)
        : this(ProcessFactory.Create(startInfo, logger), logger)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
    public Command(Process process, ICommandLogger logger)
    {
        Process = process;
        Logger = logger;
    }

    /// <summary>Gets the logger.</summary>
    public ICommandLogger Logger { get; init; }

    /// <summary>Gets the exit code.</summary>
    public int ExitCode => Process.ExitCode;

    /// <summary>Gets a value indicating whether this command has exited.</summary>
    public bool HasExited => Process.HasExited;

    public bool IsRunning { get; private set; }

    /// <summary>Gets the text output.</summary>
    public string? TextOutput => Logger.FindOutput<TextOutput>()?.Logger.Output;

    /// <summary>Gets the process.</summary>
    internal Process Process { get; init; }

    /// <summary>Execute command and start it.</summary>
    public static Command CreateAndStart(
        string commandPath,
        string arguments,
        string? workingDirectory = null,
        LogLevel outputLogLevel = LogLevel.Debug)
    {
        var logger = new CommandLogger(
            outputLogLevel,
            CreateConsoleOutput(outputLogLevel),
            new TextOutput());

        var command = new Command(commandPath, arguments, workingDirectory, logger);

        command.Start();

        return command;
    }

    /// <summary>Creates the console output.</summary>
    public static HostOutput CreateConsoleOutput(LogLevel level) =>
        CreateConsoleOutput(level, string.Empty, 1, false);

    /// <summary>Creates the console output.</summary>
    public static HostOutput CreateConsoleOutput(
        LogLevel logLevel,
        string prefix,
        int offsetRation,
        bool noColor) =>
        new HostOutput(
            Console.Out,
            logLevel,
            new HostOutputFormatter(new HostPalette(), prefix, offsetRation, noColor));

    /// <summary>Execute command and wait for exit.</summary>
    public static Command CreateAndWait(
        string commandPath,
        string arguments,
        string? workingDirectory = null,
        LogLevel outputLogLevel = LogLevel.Debug)
    {
        var command = CreateAndStart(commandPath, arguments, workingDirectory, outputLogLevel);

        command.WaitForResult();

        return command;
    }

    /// <summary>Starts the specified command.</summary>
    public void Start()
    {
        var info = Process.StartInfo;
        Log($"{info.FileName} {info.Arguments}", LogLevel.Debug);

        if (Process.Start())
        {
            if (Process.StartInfo.RedirectStandardOutput)
            {
                Process.BeginOutputReadLine();
            }

            if (Process.StartInfo.RedirectStandardError)
            {
                Process.BeginErrorReadLine();
            }

            IsRunning = true;
        }

        Log("----------------------", Logger.LogLevel);
    }

    /// <summary>Closes the specified command.</summary>
    public void Close()
    {
        if (IsRunning && !Process.HasExited)
        {
            Process.Close();
            IsRunning = Process.IsRunning();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Wait for the process to end end return the output as string.</summary>
    public void WaitForResult()
    {
        Process.WaitForExit();
        Log($"Process[{Process.Id}]: Exit code '{ExitCode}'", LogLevel.Debug);
    }

    /// <summary>Logs information to the host.</summary>
    internal void Log(string message, LogLevel logLevel) =>
        Logger.FindOutput<HostOutput>()?.WriteLine(message, logLevel);

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Close();
            if (IsRunning)
            {
                Process.ErrorDataReceived -= Logger.LogError;
                Process.OutputDataReceived -= Logger.LogOutput;
            }

            Process.Dispose();
        }
    }
}
