using System;
using System.Collections.Generic;
using System.Linq;

namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 全连接神经网络
    /// </summary>
    public class FullyConnectedNeuralNetwork
    {
        public NetworkLayer HeadLayer { get; set; }

        private double Activation(double d)
        {
            return 1 / (1 + Math.Exp(-d));
        }

        public void ForwardPropagation()
        {
            var tempLayer = HeadLayer;
            while (tempLayer.NextLayer != null)
            {
                tempLayer = tempLayer.NextLayer;
                foreach (Node node in tempLayer.Nodes)
                {
                    node.Value = (from e in node.Edges where e.ToNode == node select e.FromNode.Value * e.Value).Sum();
                }
            }
        }
    }
}