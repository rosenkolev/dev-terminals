using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals;

/// <summary>The shell options.</summary>
public sealed record ShellOptions(
    string? WorkingDirectory = null,
    bool AutoHostOutput = false,
    LogLevel? LogLevel = null);
