using System.Diagnostics;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Commands;

/// <summary>A command logger.</summary>
public interface ICommandLogger
{
    /// <summary>Gets or sets the log level.</summary>
    LogLevel LogLevel { get; set; }

    /// <summary>Logs the output.</summary>
    void LogOutput(object sender, DataReceivedEventArgs e);

    /// <summary>Logs the error.</summary>
    void LogError(object sender, DataReceivedEventArgs e);

    /// <summary>Adds the specified output.</summary>
    void Add(IOutput output);
}
