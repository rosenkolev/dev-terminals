using Dev.Terminals;
using Dev.Terminals.Commands;
using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Loggers.Host;
using Dev.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev.Tests.TerminalIntegration
{
    [TestClass]
    public class TerminalIntegrationTests
    {
        [TestMethod]
        public void TestTerminalLoggerShouldLogTheSame()
        {
            // TODO: LoggerTests.InitNullLogger();
            var logger = new CommandLogger(LogLevel.Message);
            var formatter = new HostOutputFormatter(
                new HostPalette(),
                prefix: string.Empty,
                offsetRation: 0,
                noColor: true);

            using var writer = MemoryTextStream.Create();
            var hostOutput = new HostOutput(writer.Writer, LogLevel.Message, formatter);
            var textOutput = new TextOutput();
            var monitor = new TerminalMonitor(logger, textOutput, hostOutput);
            var syntax = TerminalFacade.DefaultTerminalSyntax;
            var terminal = new Terminal(syntax, monitor, Terminal.CreateCommand(syntax, null, logger));

            var result = terminal.Exec(
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2") &
                TerminalCommand.Create("echo", "Test3"));

            var value = writer.GetText().TrimEnd('\r', '\n');

            Assert.AreEqual(value, result.Output);
        }
    }
}
