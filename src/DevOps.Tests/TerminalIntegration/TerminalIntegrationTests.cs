using DevOps.Commands;
using DevOps.Loggers;
using DevOps.Loggers.Abstraction;
using DevOps.Loggers.Host;
using DevOps.Terminals;
using DevOps.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TerminalIn = DevOps.Terminals.Terminal;

namespace DevOps.Tests.TerminalIntegration
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
            var terminal = new TerminalIn(syntax, monitor, TerminalIn.CreateCommand(syntax, null, logger));

            var result = terminal.Exec(
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2") &
                TerminalCommand.Create("echo", "Test3"));

            var value = writer.GetText().TrimEnd('\r', '\n');

            Assert.AreEqual(value, result.Output);
        }
    }
}
