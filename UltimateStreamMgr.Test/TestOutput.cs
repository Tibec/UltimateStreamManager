using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.Test
{
    [TestClass]
    public class TestOutput
    {
        [TestMethod]
        public void RetrieveInfo()
        {
            var output = Output.RetrieveAllProperties();
        }
    }
}
