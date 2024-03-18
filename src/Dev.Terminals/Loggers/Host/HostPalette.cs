using System;

namespace Dev.Terminals.Loggers.Host;

/// <summary>A color palette.</summary>
public class HostPalette
{
    private const string ResetColor = "\x1b[0m";
    private const string BlackColor = "\x1b[30m";
    private const string RedColor = "\x1b[31m";
    private const string GreenColor = "\x1b[32m";
    private const string YellowColor = "\x1b[33m";
    private const string BlueColor = "\x1b[34m";
    private const string MagentaColor = "\x1b[35m";
    private const string CyanColor = "\x1b[36m";
    private const string WhiteColor = "\x1b[37m";
    private const string BrightBlackColor = "\x1b[90m";
    private const string BrightRedColor = "\x1b[91m";
    private const string BrightGreenColor = "\x1b[92m";
    private const string BrightYellowColor = "\x1b[93m";
    private const string BrightBlueColor = "\x1b[94m";
    private const string BrightMagentaColor = "\x1b[95m";
    private const string BrightCyanColor = "\x1b[96m";
    private const string BrightWhiteColor = "\x1b[97m";

    /// <summary>Initializes a new instance of the <see cref="HostPalette"/> class.</summary>
    public HostPalette()
    {
        var bg = Console.BackgroundColor;
        var black = bg == ConsoleColor.Black ? BrightBlackColor : BlackColor;
        var magenta = bg == ConsoleColor.DarkMagenta ? BrightMagentaColor : MagentaColor;
        var cyan = bg == ConsoleColor.DarkCyan ? BrightCyanColor : CyanColor;
        var white = bg == ConsoleColor.Gray ? BrightWhiteColor : WhiteColor;
        var yellow = bg == ConsoleColor.DarkYellow ? BrightYellowColor : YellowColor;
        var red = bg == ConsoleColor.DarkRed ? BrightRedColor : RedColor;
        var green = bg == ConsoleColor.DarkGreen ? BrightGreenColor : GreenColor;
        var blue = bg == ConsoleColor.DarkBlue ? BrightBlueColor : BlueColor;
        var brightBlack = bg == ConsoleColor.DarkGray ? black : BrightBlackColor;
        var brightMagenta = bg == ConsoleColor.Magenta ? magenta : BrightMagentaColor;
        var brightCyan = bg == ConsoleColor.Cyan ? cyan : BrightCyanColor;
        var brightRed = bg == ConsoleColor.Red ? red : BrightRedColor;
        var brightYellow = bg == ConsoleColor.Yellow ? yellow : BrightYellowColor;
        var brightBlue = bg == ConsoleColor.Blue ? blue : BrightBlueColor;

        Reset = ResetColor;
        Prefix = brightBlack;
        Trace = brightMagenta;
        Debug = brightCyan;
        Information = white;
        Message = brightYellow;
        Error = brightRed;
        None = brightBlue;
        Success = green;
    }

    /// <summary>Gets the reset.</summary>
    public string Reset { get; init; }

    /// <summary>Gets or sets the prefix.</summary>
    public string Prefix { get; set; }

    /// <summary>Gets the trace.</summary>
    public string Trace { get; init; }

    /// <summary>Gets the debug.</summary>
    public string Debug { get; init; }

    /// <summary>Gets the information.</summary>
    public string Information { get; init; }

    /// <summary>Gets the warning.</summary>
    public string Message { get; init; }

    /// <summary>Gets the error.</summary>
    public string Error { get; init; }

    /// <summary>Gets the none.</summary>
    public string None { get; init; }

    /// <summary>Gets the success.</summary>
    public string Success { get; init; }
}
