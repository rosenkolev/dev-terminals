// cspell:ignore errorlevel

using DevOps.Terminals.Syntax;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class SyntaxTests
    {
        [TestMethod]
        public void UnixShShouldMatchParams()
        {
            var syntax = new UnixShSyntax();

            Assert.AreEqual("/bin/sh", syntax.CommandName);
            Assert.AreEqual("-c \"{0}\"", syntax.CommandArguments);
            Assert.AreEqual("$?", syntax.ReturnCodeCommand);
            Assert.AreEqual("-test \"\\\"test\\\"\" $something", syntax.BuildCommand(new [] { "-test", "\"test\"", "$something" }));
        }

        [TestMethod]
        public void WindowsCmdShouldMatchParams()
        {
            var syntax = new WindowsCmdSyntax();

            Assert.AreEqual("cmd.exe", syntax.CommandName);
            Assert.AreEqual("/C {0}", syntax.CommandArguments);
            Assert.AreEqual("%errorlevel%", syntax.ReturnCodeCommand);
            Assert.AreEqual("-test \"\\\"test\\\"\" $something", syntax.BuildCommand(new[] { "-test", "\"test\"", "$something" }));
        }
    }
}
