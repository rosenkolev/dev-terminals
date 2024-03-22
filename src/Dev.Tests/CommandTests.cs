using System;
using System.Runtime.InteropServices;

using Dev.Terminals;
using Dev.Terminals.Commands;
using Dev.Terminals.Loggers.Abstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev.Tests;

[TestClass]
public class CommandTests
{
    private static bool NotWindows() =>
        !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [TestMethod]
    public void CommandShouldWork()
    {
        if (NotWindows())
        {
            return;
        }

        using var command = Command.CreateAndStart(
              commandPath: "cmd.exe",
              arguments: "/c \"ping localhost>NUL && echo MyTestString\"",
              workingDirectory: "c:\\",
              outputLogLevel: LogLevel.Debug
        );

        Assert.IsFalse(command.HasExited);

        // Wait for the process to complete!
        command.WaitForResult();

        // Throws exception when Exit Code is not 0
        command.EnsureExitCodeIs(0);

        Console.WriteLine(command.ExitCode); // 1
        Console.WriteLine(command.TextOutput); // MyTestString
        Assert.IsTrue(command.HasExited);
        Assert.AreEqual(0, command.ExitCode);
        Assert.AreEqual("MyTestString", command.TextOutput);
        Assert.AreEqual("cmd.exe", command.Process.StartInfo.FileName);
    }

    [TestMethod]
    public void Command_ShouldDisposeWhenRunning()
    {
        var syntax = Shells.DefaultTerminalSyntax;
        var command = Command.CreateAndStart(
              commandPath: syntax.CommandName,
              arguments: string.Format(syntax.CommandArguments, "ping localhost"),
              workingDirectory: null,
              outputLogLevel: LogLevel.Debug
        );

        Assert.IsTrue(command.IsRunning);
        command.Dispose();
        Assert.IsFalse(command.IsRunning);
    }

    [TestMethod]
    public void CommandShouldWorkWithTimeout()
    {
        if (NotWindows())
        {
            return;
        }

        using var command = new Command("abc", "de fg", null, new CommandLogger(LogLevel.Debug));

        Assert.AreEqual("abc", command.Process.StartInfo.FileName);
        Assert.AreEqual("de fg", command.Process.StartInfo.Arguments);
    }
}
