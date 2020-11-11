using System;
using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    public class NetworkLayer
    {
        private List<double> _neurons = new List<double>();

        private List<List<double>> _ws;

        private NetworkLayer _previousLayer;

        private NetworkLayer _nextLayer;
        private void ForwardPropagation()
        {
            if (_nextLayer == null)
            {
                return;
            }

            CheckWs();

            CalculateNeurons();
        }

        private double Activation(double d)
        {
            return 1 / (1 + Math.Exp(-d));
        }

        /// <summary>
        /// 计算神经元
        /// </summary>
        private void CalculateNeurons()
        {
            for (int i = 0; i < _nextLayer._neurons.Count; i++)
            {
                double d = 0;
                for (int j = 0; j < _neurons.Count; j++)
                {
                    d += _neurons[j] * _ws[i][j];
                }

                d += _ws[i][_neurons.Count];

                _nextLayer._neurons[i] = Activation(d);
            }
        }

        /// <summary>
        /// 检查ws
        /// </summary>
        private void CheckWs()
        {
            if (_ws == null)
            {
                _ws = new List<List<double>>();
                foreach (var n in _nextLayer._neurons)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < _neurons.Count; j++)
                    {
                        list.Add(1);
                    }
                    _ws.Add(list);
                }

                //添加偏倚
                if (_previousLayer != null)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < _neurons.Count; j++)
                    {
                        list.Add(1);
                    }
                    _ws.Add(list);
                }
            }
        }
    }
}