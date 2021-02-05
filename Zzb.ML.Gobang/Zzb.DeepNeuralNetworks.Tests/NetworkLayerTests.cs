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
            net1.ForwardPropagation(new NetworkLayer(2));
        }
    }
}