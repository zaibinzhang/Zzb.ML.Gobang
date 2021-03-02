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
            var layer1 = new NetworkLayer() { Nodes = new List<Node>() { new Node(0.05), new Node(0.1), new Node(1) } };
            var layer2 = new NetworkLayer() { Nodes = new List<Node>() { new Node(0.05), new Node(0.1), new Node(1) } };
            var layer3 = new NetworkLayer() { Nodes = new List<Node>() { new Node(0.05), new Node(0.1) } };
            layer1.NextLayer = layer2;
            layer2.PreLayer = layer1;
            layer2.NextLayer = layer3;
            layer3.PreLayer = layer2;

            //第一层权重
            new Edge(layer1.Nodes[0], layer2.Nodes[0], 0.15).Connect();
            new Edge(layer1.Nodes[0], layer2.Nodes[1], 0.2).Connect();
            new Edge(layer1.Nodes[1], layer2.Nodes[0], 0.25).Connect();
            new Edge(layer1.Nodes[1], layer2.Nodes[1], 0.3).Connect();
            new Edge(layer1.Nodes[2], layer2.Nodes[0], 0.35).Connect();
            new Edge(layer1.Nodes[2], layer2.Nodes[1], 0.35).Connect();

            //第二层
            new Edge(layer2.Nodes[0], layer3.Nodes[0], 0.4).Connect();
            new Edge(layer2.Nodes[0], layer3.Nodes[1], 0.45).Connect();
            new Edge(layer2.Nodes[1], layer3.Nodes[0], 0.5).Connect();
            new Edge(layer2.Nodes[1], layer3.Nodes[1], 0.55).Connect();
            new Edge(layer2.Nodes[2], layer3.Nodes[0], 0.60).Connect();
            new Edge(layer2.Nodes[2], layer3.Nodes[1], 0.60).Connect();

            FullyConnectedNeuralNetwork network = new FullyConnectedNeuralNetwork() { HeadLayer = layer1 };
            network.ForwardPropagation();
        }
    }
}