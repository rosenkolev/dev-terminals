namespace Dev.Terminals.Loggers.Abstraction;

/// <summary>Async input stream.</summary>
public interface IInput
{
    /// <summary>Waits for message and read it.</summary>
    OutputMessage WaitAndRead();
}
