using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Dev.Terminals;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev.Tests;

[TestClass]
public class TerminalITests
{
    [TestMethod]
    public void TerminalHostAndTestOutputs_ShouldLogTheSame()
    {
        using var terminal = TestTerminal.Create(LogLevel.Message);

        var result = terminal.Execute(
            TerminalCommandFactory.CreateCommand("echo", "Test1") &
            TerminalCommandFactory.CreateCommand("echo", "Test2") &
            TerminalCommandFactory.CreateCommand("echo", "Test3"));

        var value = terminal.Text.TrimEnd('\r', '\n');

        Assert.AreEqual(value, result.Output);
        terminal.Clear();
    }

    [TestMethod]
    public void TerminalCommand_ShouldBePiped()
    {
        var command =
            TerminalCommandFactory.CreateCommand("echo", "Test1") &
            TerminalCommandFactory.CreateCommand("echo", "Test2") &
            TerminalCommandFactory.CreateCommand("echo", "Test3") &
            TerminalCommandFactory.CreateCommand("echo", "Test4");

        Assert.AreEqual("Test1", command.CommandArguments[1]);
        Assert.AreEqual("Test1", command.Next.Previous.CommandArguments[1]);
        Assert.AreEqual("Test2", command.Next.CommandArguments[1]);
        Assert.AreEqual("Test2", command.Next.Next.Previous.CommandArguments[1]);
        Assert.AreEqual("Test3", command.Next.Next.CommandArguments[1]);
        Assert.AreEqual("Test4", command.Next.Next.Next.CommandArguments[1]);
    }

    [TestMethod]
    public async Task Terminal_ShouldExecuteAsync_InSyncWay()
    {
        using var terminal = TestTerminal.Create(LogLevel.Info);

        var ping = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "ping -n 2 127.0.0.1 > nil "
            : "ping -c 2 127.0.0.1 > nil ";

        var task1 = terminal.ExecuteAsync(
            TerminalCommandFactory.Parse(ping + "&& echo A", null, null));
        Thread.Sleep(10);
        var task2 = terminal.ExecuteAsync(
            TerminalCommandFactory.Parse(ping + "&& echo B", null, null));

        Assert.AreEqual(false, task1.IsCompleted);
        Assert.AreEqual(false, task2.IsCompleted);

        await task1;

        Assert.AreEqual(true, task1.IsCompleted);
        Assert.AreEqual(false, task2.IsCompleted);

        await task2;

        Assert.AreEqual($"A{Environment.NewLine}B{Environment.NewLine}", terminal.Text);
        terminal.Clear();
    }
}
