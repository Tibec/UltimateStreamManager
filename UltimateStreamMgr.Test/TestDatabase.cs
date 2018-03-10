using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.Test
{
    [TestClass]
    public class TestDatabase
    {
        [TestMethod]
        public void TestMethod1()
        {
            Player p = new Player { Id = 1 };
            Add(p);
            
        }

        private void Add(Player p)
        {
            p.Id = 3;
        }
    }
}
