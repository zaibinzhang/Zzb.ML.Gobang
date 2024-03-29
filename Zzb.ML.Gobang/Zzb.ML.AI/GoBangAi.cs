﻿using System.Drawing;
using System.Threading.Channels;
using Tensorflow.Keras;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Optimizers;
using Tensorflow.NumPy;
using Tensorflow.Operations.Initializers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace Zzb.ML.AI
{
    public class GoBangAi
    {
        static GoBangAi()
        {
            var inputs = keras.Input(shape: (15, 15, 9), name: "棋盘qipan");
            var x1 = keras.layers.Conv2D(32, 3, activation: "relu", padding: "same").Apply(inputs);
            var x2 = keras.layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x1);
            var x3 = keras.layers.Conv2D(128, 3, activation: "relu", padding: "same").Apply(x2);
            var x4 = keras.layers.Conv2D(256, 3, activation: "relu", padding: "same").Apply(x3);
            var x5 = keras.layers.Conv2D(128, 3, activation: "relu", padding: "same").Apply(x4);
            var x6 = keras.layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x5);
            var x7 = keras.layers.Conv2D(32, 3, activation: "relu", padding: "same").Apply(x6);
            var x8 = keras.layers.Conv2D(16, 3, activation: "relu", padding: "same").Apply(x7);
            var x9 = keras.layers.Conv2D(4, 3, activation: "relu", padding: "same").Apply(x8);
            var x10 = keras.layers.Flatten().Apply(x9);
            var x11 = keras.layers.Dense(225, activation: "softmax").Apply(x10);

            _model = keras.Model(inputs, x11, name: "gomoku_cnn");

            _model.compile(keras.optimizers.Adam(0.0001f), keras.losses.CategoricalCrossentropy(), new[] { "accuracy" });

            //_model = new Sequential();
            //_model.add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu",
            //    input_shape: (15, 15, 9), padding: "same"));
            //_model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(128, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(256, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(128, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(16, kernel_size: (3, 3).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Conv2D(4, kernel_size: (1, 1).ToTuple(),
            //    activation: "relu", padding: "same"));
            //_model.Add(new Flatten());
            //_model.Add(new Dense(225, activation: "softmax"));
            //_model.Compile(loss: "categorical_crossentropy",
            //    optimizer: new Adam(0.0001F), metrics: new string[] { "accuracy" });
        }

        private static readonly IModel _model;

        private readonly Random rand = new();

        public Point CalculateNextStep(int[,] map, List<Point> whiteHistory, List<Point> blackHistory)
        {
            var gameSize = map.GetLength(0) - 1;
            while (true)
            {
                var point = new Point(rand.Next(gameSize), rand.Next(gameSize));
                if (map[point.Y, point.X] == 0)
                {
                    return point;
                }
            }
        }

        public (double loss, double accuracy) Train(List<Point> whiteHistory, List<Point> blackHistory)
        {
            //var (x, y) = LoadRawData(whiteHistory, blackHistory);
            int numSamples = 100;
            var inputsData = np.random.randn(numSamples, 15, 15, 9).astype(np.float32);
            var labelsData = np.random.randint(0, 225, size: numSamples).astype(np.float32);
            var h = _model.fit(inputsData, labelsData,
                  epochs: 1,
                  verbose: 0);
            var loss = h.history["loss"][0];
            var accuracy = h.history["accuracy"][0];
            return ((double)loss, (double)accuracy);
        }

        public float[,] Predict(List<Point> whiteHistory, List<Point> blackHistory)
        {
            var isBlackGo = blackHistory.Count == whiteHistory.Count;
            float[,,,] arrX = new float[1, 15, 15, 9];
            int[,] mapWhite = new int[15, 15];
            int[,] mapBlack = new int[15, 15];

            foreach (var point in whiteHistory)
            {
                mapWhite[point.X, point.Y] = 1;
            }

            foreach (var point in blackHistory)
            {
                mapBlack[point.X, point.Y] = 1;
            }

            if (isBlackGo)
            {
                //构建历史步数
                //构建黑色自己最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var blackHistCount = blackHistory.Count - 1 - j;
                    if (blackHistCount >= 0)
                    {
                        arrX[0, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j] = 1;
                    }
                }
                //构建白色最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var whiteHistCount = whiteHistory.Count - 1 - j;
                    if (whiteHistCount >= 0)
                    {
                        arrX[0, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j + 3] = 1;
                    }
                }

                //黑棋棋面
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[0, j, k, 6] = mapBlack[j, k];
                    }
                }

                //白棋棋面
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[0, j, k, 7] = mapWhite[j, k];
                    }
                }

                //是否先手
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[0, j, k, 8] = 1;
                    }
                }

            }
            else
            {
                //构建历史步数
                //构建白色最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var whiteHistCount = whiteHistory.Count - 1 - j;
                    if (whiteHistCount >= 0)
                    {
                        arrX[0, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j] = 1;
                    }
                }

                //构建黑色最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var blackHistCount = blackHistory.Count - 1 - j;
                    if (blackHistCount >= 0)
                    {
                        arrX[0, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j + 3] = 1;
                    }
                }

                //白棋棋面
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[0, j, k, 6] = mapWhite[j, k];
                    }
                }

                //黑棋棋面
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[0, j, k, 7] = mapBlack[j, k];
                    }
                }

            }

            var pre = _model.predict(np.array(arrX), verbose: 0).shape;


            float[,] res = new float[15, 15];

            //for (int i = 0; i < 15; i++)
            //{
            //    for (int j = 0; j < 15; j++)
            //    {
            //        res[i, j] = pre[i * 15 + j].item<float>();
            //    }
            //}
            return res;
        }

        //private (NDarray x, NDarray y) LoadRawData(List<Point> whiteHistory, List<Point> blackHistory)
        //{
        //    var isBlack = blackHistory.Count > whiteHistory.Count;
        //    var totalSize = blackHistory.Count + whiteHistory.Count;
        //    int[,,,] arrX = new int[totalSize, 15, 15, 9];
        //    int[,] arrY = new int[totalSize, 225];
        //    int[,] mapWhite = new int[15, 15];
        //    int[,] mapBlack = new int[15, 15];

        //    for (int i = 0; i < blackHistory.Count; i++)
        //    {
        //        //构建黑色的输入参数

        //        //构建历史步数
        //        //构建黑色自己最近走过的步数
        //        for (int j = 0; j < 3; j++)
        //        {
        //            var blackHistCount = i - 1 - j;
        //            if (blackHistCount >= 0)
        //            {
        //                arrX[i * 2, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j] = 1;
        //            }
        //        }
        //        //构建白色最近走过的步数
        //        for (int j = 0; j < 3; j++)
        //        {
        //            var whiteHistCount = i - 1 - j;
        //            if (whiteHistCount >= 0)
        //            {
        //                arrX[i * 2, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j + 3] = 1;
        //            }
        //        }

        //        //构建黑色目前局面的着点
        //        for (int j = 0; j < 15; j++)
        //        {
        //            for (int k = 0; k < 15; k++)
        //            {
        //                arrX[i * 2, j, k, 6] = mapBlack[j, k];
        //                if (mapBlack[j, k] == 1)
        //                {
        //                    arrY[i * 2, j * 15 + k] = -1;
        //                }
        //            }
        //        }

        //        //构建白色目前局面的着点
        //        for (int j = 0; j < 15; j++)
        //        {
        //            for (int k = 0; k < 15; k++)
        //            {
        //                arrX[i * 2, j, k, 7] = mapWhite[j, k];
        //                if (mapWhite[j, k] == 1)
        //                {
        //                    arrY[i * 2, j * 15 + k] = -1;
        //                }
        //            }
        //        }

        //        //是否先手
        //        for (int j = 0; j < 15; j++)
        //        {
        //            for (int k = 0; k < 15; k++)
        //            {
        //                arrX[i * 2, j, k, 8] = 1;
        //            }
        //        }

        //        arrY[i * 2, blackHistory[i].X * 15 + blackHistory[i].Y] = isBlack ? 1 : -1;
        //        mapBlack[blackHistory[i].X, blackHistory[i].Y] = 1;

        //        if (!isBlack)
        //        {
        //            //构建白色的输入参数
        //            //构建白色最近走过的步数
        //            for (int j = 0; j < 3; j++)
        //            {
        //                var whiteHistCount = i - 1 - j;
        //                if (whiteHistCount >= 0)
        //                {
        //                    arrX[i * 2 + 1, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j] = 1;
        //                }
        //            }
        //            //构建黑色最近走过的步数
        //            for (int j = 0; j < 3; j++)
        //            {
        //                var blackHistCount = i - 1 - j;
        //                if (blackHistCount >= 0)
        //                {
        //                    arrX[i * 2 + 1, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j + 3] = 1;
        //                }
        //            }

        //            //构建白色目前局面的着点
        //            for (int j = 0; j < 15; j++)
        //            {
        //                for (int k = 0; k < 15; k++)
        //                {
        //                    arrX[i * 2 + 1, j, k, 6] = mapWhite[j, k];
        //                    if (mapWhite[j, k] == 1)
        //                    {
        //                        arrY[i * 2 + 1, j * 15 + k] = -1;
        //                    }
        //                }
        //            }

        //            //构建黑色目前局面的着点
        //            for (int j = 0; j < 15; j++)
        //            {
        //                for (int k = 0; k < 15; k++)
        //                {
        //                    arrX[i * 2 + 1, j, k, 7] = mapBlack[j, k];
        //                    if (mapBlack[j, k] == 1)
        //                    {
        //                        arrY[i * 2 + 1, j * 15 + k] = -1;
        //                    }
        //                }
        //            }

        //            arrY[i * 2 + 1, whiteHistory[i].X * 15 + whiteHistory[i].Y] = isBlack ? -1 : 1;

        //            mapWhite[whiteHistory[i].X, whiteHistory[i].Y] = 1;
        //        }




        //    }
        //    return (np.array(arrX), np.array(arrY));
        //}
    }
}