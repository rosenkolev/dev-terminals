namespace Dev.Terminals.Loggers.Abstraction;

/// <summary>A logger output.</summary>
public interface IOutput
{
    bool Enabled { get; }

    /// <summary>Writes an empty new line.</summary>
    void WriteLine();

    /// <summary>Writes an message to a new line.</summary>
    void WriteLine(string message, LogLevel logLevel);

    /// <summary>Writes the specified message.</summary>
    void Write(string message, LogLevel logLevel);
}
