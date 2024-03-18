using Dev.Terminals.Loggers.Abstraction;

namespace Dev.Terminals.Loggers.Host;

/// <summary>The host message formatter.</summary>
public class HostOutputFormatter
{
    private readonly int _offsetRation;
    private readonly bool _noColor;
    private readonly string _outputPrefix;

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
        _outputPrefix = string.IsNullOrEmpty(prefix)
            ? string.Empty
            : prefix + ':';

        Palette = palette;
    }

    /// <summary>Gets the palette.</summary>
    public HostPalette Palette { get; init; }

    /// <summary>Formats the specified message.</summary>
    public virtual string FormatMessage(string message, LogLevel logLevel)
    {
        var color = GetColor(logLevel);
        return _noColor ? message : string.Concat(color, message, Palette.Reset);
    }

    /// <summary>Formats the specified message.</summary>
    public virtual string GetLinePrefix(LogLevel logLevel)
    {
        var offset = new string(' ', (int)logLevel * _offsetRation);
        return string.Concat(_noColor ? _outputPrefix : GetHostPrefix(), offset);
    }

    /// <summary>Gets the host prefix.</summary>
    protected virtual string GetHostPrefix() =>
        $"{Palette.Prefix}{_outputPrefix}{Palette.Reset}";

    /// <summary>Gets the log level color.</summary>
    protected virtual string GetColor(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Error => Palette.Error,
            LogLevel.Message => Palette.Message,
            LogLevel.Info => Palette.Information,
            LogLevel.Verbose => Palette.Debug,
            LogLevel.Debug => Palette.Trace,
            _ => Palette.None,
        };
}
