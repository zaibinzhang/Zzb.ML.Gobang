using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zzb.ML.Gobang.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zzb.ML.Gobang.AI.Tests
{
    [TestClass()]
    public class MonteCarloTreeSearchTests
    {
        [TestMethod()]
        public void CalNextTest()
        {

            MonteCarloTreeSearch tree = new MonteCarloTreeSearch();

            tree.CalNext(new int[15, 15], true);
            Assert.IsTrue(true);
        }
    }
}