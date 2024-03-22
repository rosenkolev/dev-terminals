using System;
using System.IO;
using System.Runtime.InteropServices;

using Dev.Terminals;
using Dev.Terminals.Loggers.Abstraction;
using Dev.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Dev.Terminals.Shells;

namespace Dev.Tests;

[TestClass]
public class ShellTests
{
    private static TestTerminal _terminal;

    [ClassInitialize]
    public static void Init(TestContext context)
    {
        _terminal = TestTerminal.Create(LogLevel.Verbose);
        DefaultTerminal = _terminal;
    }

    [ClassCleanup]
    public static void Cleanup()
    {
        _terminal.Dispose();
    }

    [TestCleanup]
    public void Clean()
    {
        _terminal.Execute(
            TerminalCommandFactory.Cd(Directory.GetCurrentDirectory()));
        _terminal.Clear();
    }

    [TestMethod]
    public void Shell_ShouldExec()
    {
        var result = Shell("echo shell test");
        Assert.AreEqual("shell test", result.Output);
    }

    [TestMethod]
    public void Shell_ShouldGetCd()
    {
        var dir = _terminal.CurrentDirectory;
        Assert.IsFalse(string.IsNullOrEmpty(dir));
    }

    [TestMethod]
    public void Shell_ShouldCd()
    {
        var temp = Path.GetTempPath();
        Shell(TerminalCommandFactory.Cd(temp));
        Assert.AreEqual(_terminal.CurrentDirectory.Trim('/', '\\'), temp.Trim('/', '\\'));
    }

    [TestMethod]
    public void Shell_ShouldPrintInHost()
    {
        Shell("echo Test");
        Assert.AreEqual("Test" + Environment.NewLine, _terminal.Text);
    }

    [TestMethod]
    public void Shell_ShouldNotPrintInHost()
    {
        _terminal.Clear();

        Shell("echo Test", new ShellOptions { AutoHostOutput = false });
        Assert.AreEqual(string.Empty, _terminal.Text);
    }

    [TestMethod]
    public void Shell_ShouldChangeWorkingDirectory()
    {
        var path = Path.GetTempPath();
        var result = Shell(
            "echo " + _terminal.Syntax.CurrentDirectoryCodeCommand,
            new ShellOptions { WorkingDirectory = path });

        Assert.AreEqual(path.Trim('/', '\\'), result.Output);
    }

    [TestMethod]
    public void TerminalShouldPipeCommands()
    {
        var command =
            TerminalCommandFactory.CreateCommand("echo", "Test1") &
            TerminalCommandFactory.CreateCommand("echo", "Test2");

        var result = Shell(command);
        Assert.AreEqual($"Test1{Environment.NewLine}Test2", result.Output);
    }

    [TestMethod]
    public void TerminalShouldPipeTreeCommands()
    {
        var command =
            TerminalCommandFactory.CreateCommand("echo", "Test1") &
            TerminalCommandFactory.CreateCommand("echo", "Test2") &
            TerminalCommandFactory.CreateCommand("echo", "Test3");

        var result = Shell(command);
        Assert.AreEqual($"Test1{Environment.NewLine}Test2{Environment.NewLine}Test3", result.Output);
    }

    [TestMethod]
    public void DefaultShell_ShouldBeSetup()
    {
        using var terminal = CreateDefaultTerminal();
        var shellName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh";
        
        Assert.AreEqual(shellName, terminal.Syntax.CommandName);
        Assert.AreEqual(terminal.LogLevel, LogLevel.Info);
        Assert.AreEqual(terminal.IsHostOutputEnabled, true);
        Assert.AreEqual(terminal.CurrentDirectory, Directory.GetCurrentDirectory());
        Assert.AreSame(terminal.Monitor.HostOutput.Writer, Console.Out);
        Assert.AreEqual(terminal.Monitor.HostOutput.Formatter.Prefix, string.Empty);
        Assert.AreEqual(terminal.Monitor.HostOutput.Formatter.FormatWithColor, true);
    }
}
