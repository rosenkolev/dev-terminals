using DevOps.Loggers.Abstraction;
using DevOps.Loggers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests.Loggers
{
    [TestClass]
    public class TextOutputTests
    {
        [TestMethod]
        public void TextOutputShouldCollectError()
        {
            var output = new TextOutput();

            output.Write("Test Error", LogLevel.Error);

            Assert.AreEqual("Test Error", output.Logger.Error);
            Assert.AreEqual(string.Empty, output.Logger.Output);
        }

        [TestMethod]
        public void TextOutputShouldCollectOutputAndTrim()
        {
            var output = new TextOutput();

            output.Write("Test Message ", LogLevel.Message);
            output.Write("Test Info ", LogLevel.Info);
            output.Write("Test Verbose ", LogLevel.Verbose);
            output.Write("Test Debug ", LogLevel.Debug);
            output.Write("Test None     ", LogLevel.None);

            Assert.AreEqual("Test Message Test Info Test Verbose Test Debug Test None", output.Logger.Output);
            Assert.AreEqual(string.Empty, output.Logger.Error);
        }
    }
}
