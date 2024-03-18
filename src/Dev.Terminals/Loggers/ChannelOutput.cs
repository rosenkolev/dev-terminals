using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers;

/// <summary>A channel/pipe output.</summary>
public class ChannelOutput : IOutput, IInput
{
    /// <summary>Gets the logger.</summary>
    [MemberNotNull]
    internal ScopeLogger Logger { get; private set; } = new ScopeLogger();

    /// <inheritdoc/>
    public void WriteLine() =>
        Write(Environment.NewLine, LogLevel.Message);

    /// <inheritdoc/>
    public void WriteLine(string message, LogLevel logLevel) =>
        Write(message + Environment.NewLine, logLevel);

    /// <inheritdoc/>
    public void Write(string message, LogLevel logLevel) =>
        Logger.Write(
            new OutputMessage
            {
                Message = message,
                Level = logLevel,
            });

    /// <inheritdoc/>
    public OutputMessage WaitAndRead() =>
        Logger.WaitAndRead();

    /// <summary>Resets this output.</summary>
    public void Reset() =>
        Logger = new ScopeLogger();

    /// <summary>Channel scope logger.</summary>
    internal class ScopeLogger
    {
        private readonly ConcurrentQueue<OutputMessage> _queue = new ConcurrentQueue<OutputMessage>();

        /// <summary>Gets or sets the wait interval in milliseconds.</summary>
        public static int WaitIntervalInMs { get; set; } = 50;

        /// <summary>Writes the specified message.</summary>
        public void Write(OutputMessage message) =>
            _queue.Enqueue(message);

        /// <summary>Wait and read asynchronous.</summary>
        public OutputMessage WaitAndRead()
        {
            while (_queue.IsEmpty)
            {
                Thread.Sleep(WaitIntervalInMs);
            }

            return _queue.TryDequeue(out var output)
                ? output
                : throw new InvalidOperationException("Failed to dequeue output message");
        }
    }
}
