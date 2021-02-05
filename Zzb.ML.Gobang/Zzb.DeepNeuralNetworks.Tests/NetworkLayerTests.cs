using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zzb.DeepNeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zzb.DeepNeuralNetworks.Tests
{
    [TestClass()]
    public class NetworkLayerTests
    {
        [TestMethod()]
        public void ForwardPropagationTest()
        {
            var net1 = new NetworkLayer(
                new[] { 0.05, 0.1 },
                new double[] { .15, .2 },
                new double[] { .25, .3 },
                new[] { .35, .35 }
            );
            var next = new NetworkLayer(2);
            net1.ForwardPropagation(next);
            Assert.IsTrue(Math.Abs(next.GetNeurons(0) - 0.593) < 0.001&& Math.Abs(next.GetNeurons(1) - 0.596) < 0.001);
        }
    }
}