using Dev.Terminals;
using Dev.Terminals.Commands;
using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Loggers.Host;

namespace Dev.Tests.Helpers;

internal class TestTerminal : Terminal
{
    private readonly MemoryTextStream _stream;

    public TestTerminal(
        MemoryTextStream stream,
        TerminalCommandSyntax syntax,
        TerminalMonitor terminal,
        Command command)
        : base(syntax, terminal, command)
    {
        _stream = stream;
    }

    public static TestTerminal Create(
        LogLevel logLevel)
    {
        var writer = MemoryTextStream.Create();
        var hostOutput = HostOutput.Create(
            writer.Writer, logLevel, new HostPalette(), string.Empty, 0, true);
        var monitor = new TerminalMonitor(new TextOutput(), hostOutput);
        var syntax = Shells.DefaultTerminalSyntax;
        var terminal = new TestTerminal(
            writer,
            syntax,
            monitor,
            Terminal.CreateCommand(syntax, null, new CommandLogger(logLevel)));

        return terminal;
    }

    public string Text => _stream.GetText();

    public void Clear() => _stream.Clear();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _stream.Dispose();
    }
}
