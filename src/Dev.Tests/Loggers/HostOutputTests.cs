using System;
using System.IO;

using Dev.Terminals.Loggers.Abstraction;
using Dev.Terminals.Loggers.Host;
using Dev.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev.Tests.Loggers
{
    [TestClass]
    public class HostOutputTests
    {
        public static HostOutput CreateOutput(
            string prefix,
            LogLevel logLevel,
            int offset,
            bool color,
            StreamWriter writer) =>
            new HostOutput(
                writer,
                logLevel,
                new HostOutputFormatter(new HostPalette(), prefix, offset, !color));

        [TestMethod]
        public void HostOutput_ShouldWriteOneLine_WithNoColorNoPrefix()
        {
            using var stream = MemoryTextStream.Create();
            var output = CreateOutput(string.Empty, LogLevel.Info, offset: 1, color: false, stream.Writer);

            output.Write("A", LogLevel.Debug);
            output.Write("B", LogLevel.Verbose);
            output.Write("C", LogLevel.Info);
            output.Write("D", LogLevel.Message);

            Assert.AreEqual("  CD", stream.GetText());
        }


        [TestMethod]
        public void HostOutput_ShouldWriteManyLine_WithNoColorNoPrefix()
        {
            using var stream = MemoryTextStream.Create();
            var output = CreateOutput(string.Empty, LogLevel.Info, offset: 1, color: false, stream.Writer);

            output.Write($"A{Environment.NewLine}B", LogLevel.Message);
            output.Write("C", LogLevel.Verbose);
            output.WriteLine("D", LogLevel.Message);
            output.WriteLine("E", LogLevel.Message);

            Assert.AreEqual(
                $" A{Environment.NewLine} BD{Environment.NewLine} E{Environment.NewLine}",
                stream.GetText());
        }

        [TestMethod]
        public void HostOutput_ShouldWrite_WithPrefixNoColor()
        {
            using var stream = MemoryTextStream.Create();
            var output = CreateOutput("test", LogLevel.Info, offset: 1, color: false, stream.Writer);

            output.Write("A", LogLevel.Message);
            output.Write("B", LogLevel.Message);
            output.WriteLine("C", LogLevel.Info);
            output.WriteLine("D", LogLevel.Info);
            output.Write("E", LogLevel.Message);
            output.Write("F", LogLevel.Message);
            output.WriteLine("G", LogLevel.Message);
            output.Write($"H{Environment.NewLine}I{Environment.NewLine}JK", LogLevel.Info);

            Assert.AreEqual(
                "test: ABC" + Environment.NewLine +
                "test:  D" + Environment.NewLine +
                "test: EFG" + Environment.NewLine +
                "test:  H" + Environment.NewLine +
                "test:  I" + Environment.NewLine +
                "test:  JK",
                stream.GetText());
        }

        [TestMethod]
        public void HostOutput_ShouldWrite_WithPrefixColor()
        {
            using var stream = MemoryTextStream.Create();
            var output = CreateOutput("Z", LogLevel.Info, offset: 1, color: true, stream.Writer);

            output.Write("A", LogLevel.Info);
            output.WriteLine("B", LogLevel.Message);
            output.WriteLine("C", LogLevel.Message);

            Assert.AreEqual(
                "\u001b[90mZ:\u001b[0m  \u001b[37mA\u001b[0m\u001b[93mB\u001b[0m" + Environment.NewLine +
                "\u001b[90mZ:\u001b[0m \u001b[93mC\u001b[0m" + Environment.NewLine,
                stream.GetText());
        }
    }
}
