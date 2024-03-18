using System;

using DevOps.Terminals;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static DevOps.TerminalFacade;

namespace DevOps.Tests
{
    [TestClass]
    public class ShellTests
    {
        [TestMethod]
        public void ShellShouldExec()
        {
            //TODO: LoggerTests.InitNullLogger();
            var result = Shell("echo shell test");
            Assert.AreEqual("shell test", result.Output);
        }

        [TestMethod]
        public void TerminalShouldExec()
        {
            //TODO: LoggerTests.InitNullLogger();
            var result = Shell("echo shell test");
            Assert.AreEqual("shell test", result.Output);
        }

        [TestMethod]
        public void TerminalShouldPipeCommands()
        {
            //TODO: LoggerTests.InitNullLogger();
            var command =
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2");

            var result = Shell(command);
            Assert.AreEqual($"Test1{Environment.NewLine}Test2", result.Output);
        }

        [TestMethod]
        public void TerminalShouldPipeTreeCommands()
        {
            // TODO: LoggerTests.InitNullLogger();
            var command =
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2") &
                TerminalCommand.Create("echo", "Test3");

            var result = Shell(command);
            Assert.AreEqual($"Test1{Environment.NewLine}Test2{Environment.NewLine}Test3", result.Output);
        }
    }
}
