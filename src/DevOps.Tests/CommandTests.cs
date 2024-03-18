using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevOps.Commands;
using DevOps.Loggers.Abstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public void CommandShouldWork()
    {
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
}
