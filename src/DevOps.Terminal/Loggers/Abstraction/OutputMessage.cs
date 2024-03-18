namespace DevOps.Loggers.Abstraction;

/// <summary>An output message.</summary>
public class OutputMessage
{
    /// <summary>Gets the log level.</summary>
    public required LogLevel Level { get; init; }

    /// <summary>Gets the message.</summary>
    public required string Message { get; init; }

    /// <inheritdoc/>
    public override string ToString() => $"[{Level}]: {Message}";
}
