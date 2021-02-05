using System;
using System.Collections.Generic;
using System.Linq;

namespace Zzb.DeepNeuralNetworks
{
    public class NetworkLayer
    {
        public NetworkLayer(int length)
        {
            _neurons = new double[length];
        }

        public NetworkLayer(double[] neurons)
        {
            _neurons = neurons;
        }

        public NetworkLayer(double[] neurons, params double[][] weight) : this(neurons)
        {
            var list = (from w in weight select w.ToList()).ToList();
            _weights = list.GetRange(0, list.Count - 1);
            _bWeights = list[^1];
        }

        public double GetNeurons(int index)
        {
            return _neurons[index];
        }

        private double[] _neurons;

        /// <summary>
        /// 权重
        /// </summary>
        private List<List<double>> _weights;

        /// <summary>
        /// 截距项权重
        /// </summary>
        private List<double> _bWeights;

        /// <summary>
        /// 下一层的权重误差，用于计算上一层的误差
        /// </summary>
        private double[,] _tolerances;

        /// <summary>
        /// 前向传播
        /// </summary>
        /// <param name="nextLayer"></param>
        public void ForwardPropagation(NetworkLayer nextLayer)
        {
            if (nextLayer == null)
            {
                return;
            }

            CheckWeights(nextLayer);

            CalculateNeurons(nextLayer);
        }

        /// <summary>
        /// 激活函数，默认sigmoid
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private double Activation(double d)
        {
            return 1 / (1 + Math.Exp(-d));
        }

        /// <summary>
        /// 计算神经元
        /// </summary>
        private void CalculateNeurons(NetworkLayer nextLayer)
        {
            for (int i = 0; i < nextLayer._neurons.Length; i++)
            {
                double d = 0;
                for (int j = 0; j < _neurons.Length; j++)
                {
                    d += _neurons[j] * _weights[i][j];
                }

                d += _bWeights[i];

                nextLayer._neurons[i] = Activation(d);
            }
        }

        /// <summary>
        /// 检查权重有没有初始化
        /// </summary>
        private void CheckWeights(NetworkLayer nextLayer)
        {
            if (_weights == null)
            {
                _weights = new List<List<double>>();
                foreach (var n in nextLayer._neurons)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < _neurons.Length; j++)
                    {
                        list.Add(1);
                    }
                    _weights.Add(list);
                }

                //添加偏倚
                //if (_previousLayer != null) //输入层不需要添加偏倚
                {
                    for (int j = 0; j < _neurons.Length; j++)
                    {
                        _bWeights.Add(1);
                    }
                }
            }
        }

        /// <summary>
        /// 反向传播
        /// </summary>
        /// <param name="previousLayer">上一层神经网络</param>
        /// <param name="targets">目标值</param>
        /// <param name="learningRate">学习速率</param>
        private void BackPropagation(NetworkLayer previousLayer, List<double> targets, double learningRate)
        {
            if (previousLayer == null)
            {
                return;
            }

            previousLayer._tolerances ??= new double[previousLayer._neurons.Length, _neurons.Length];

            //选择上一层的某个神经单元
            for (int i = 0; i < previousLayer._weights.Count; i++)
            {
                //遍历这个神经单元的所有权重
                for (int j = 0; j < previousLayer._weights[i].Count; j++)
                {
                    double tolerance = CalculateNeuronWeightTolerance(i, j, targets[i], previousLayer._neurons[i]);

                    _tolerances[i, j] = tolerance;

                    //计算反向传播后的权重大小
                    previousLayer._weights[i][j] =
                        previousLayer._weights[i][j] - learningRate * tolerance;
                }
            }
        }

        /// <summary>
        /// 计算反向传播权重误差
        /// </summary>
        /// <returns></returns>
        private double CalculateNeuronWeightTolerance(int i, int j, double target, double previousOut)
        {
            //求误差一共分三项
            //第一项是总误差对out的求导
            double d1 = 0;

            //如果下一层已经计算过误差，可以拿过来用
            if (_tolerances != null)
            {
                for (int k = 0; k < _weights[i].Count; k++)
                {
                    d1 += _tolerances[i, k];
                }
            }
            else
            {
                d1 = _neurons[j] - target;
            }

            //第二项是激活函数的求导
            double d2 = _neurons[j] * (1 - _neurons[j]);
            //第三项是向前传播的求导
            double d3 = previousOut;

            //三者相乘
            return d1 * d2 * d3;
        }
    }
}