using System;

using Dev.Terminals.Loggers;
using Dev.Terminals.Loggers.Abstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev.Tests.Loggers
{
    [TestClass]
    public class ChannelOutputTests
    {
        [TestMethod]
        public void ChannelOutputShouldBeFiFo()
        {
            var channelOutput = new ChannelOutput();

            channelOutput.Write("Test Write", LogLevel.Info);
            channelOutput.WriteLine("Test WriteLine", LogLevel.Info);

            Assert.AreEqual("Test Write", channelOutput.WaitAndRead().Message);
            Assert.AreEqual("Test WriteLine" + Environment.NewLine, channelOutput.WaitAndRead().Message);
        }
    }
}
