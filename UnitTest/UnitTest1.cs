using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolModXdLib;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string source = @"D:\RuntimeHDD\DotaXdWorkDir\test.txt";
            var t = new VersionInjector(source, null);
            t.Inject();
        }
    }
}
