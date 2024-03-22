using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers.Host;

/// <summary>The host message formatter.</summary>
public class HostOutputFormatter
{
    private readonly int _offsetRation;
    private readonly bool _noColor;
    private readonly HostPalette _palette;

    /// <summary>Initializes a new instance of the <see cref="HostOutputFormatter"/> class.</summary>
    public HostOutputFormatter(HostPalette palette, string prefix)
        : this(palette, prefix, 2, false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="HostOutputFormatter"/> class.</summary>
    public HostOutputFormatter(
        HostPalette palette,
        string prefix,
        int offsetRation,
        bool noColor)
    {
        _offsetRation = offsetRation;
        _noColor = noColor;
        Prefix = string.IsNullOrEmpty(prefix)
            ? string.Empty
            : prefix + ':';

        _palette = palette;
    }

    /// <summary>Gets a value indicating whether to format with color.</summary>
    internal bool FormatWithColor => !_noColor;

    /// <summary>Gets the terminal prefix.</summary>
    internal string Prefix { get; init; }

    /// <summary>Formats the specified message.</summary>
    public virtual string FormatMessage(string message, LogLevel logLevel)
    {
        var color = GetColor(logLevel);
        return _noColor ? message : string.Concat(color, message, _palette.Reset);
    }

    /// <summary>Formats the specified message.</summary>
    public virtual string GetLinePrefix(LogLevel logLevel)
    {
        var offset = new string(' ', (int)logLevel * _offsetRation);
        return string.Concat(_noColor ? Prefix : GetHostPrefix(), offset);
    }

    /// <summary>Gets the host prefix.</summary>
    protected virtual string GetHostPrefix() =>
        $"{_palette.Prefix}{Prefix}{_palette.Reset}";

    /// <summary>Gets the log level color.</summary>
    protected virtual string GetColor(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Error => _palette.Error,
            LogLevel.Message => _palette.Message,
            LogLevel.Info => _palette.Information,
            LogLevel.Verbose => _palette.Debug,
            LogLevel.Debug => _palette.Trace,
            _ => _palette.None,
        };
}
