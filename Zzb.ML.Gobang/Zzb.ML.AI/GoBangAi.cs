using System.Drawing;
using System.Threading.Channels;
using Keras.Layers;
using Keras.Models;
using Keras.Optimizers;
using Numpy;

namespace Zzb.ML.AI
{
    public class GoBangAi
    {
        static GoBangAi()
        {
            _model = new Sequential();
            _model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
                activation: "relu",
                input_shape: (15, 15, 9), padding: "same"));
            _model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Conv2D(128, kernel_size: (3, 3).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(),
    activation: "relu", padding: "same"));
            _model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
    activation: "relu", padding: "same"));
            _model.Add(new Conv2D(16, kernel_size: (3, 3).ToTuple(),
    activation: "relu", padding: "same"));
            _model.Add(new Conv2D(4, kernel_size: (1, 1).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Flatten());
            _model.Add(new Dense(225, activation: "softmax"));
            _model.Compile(loss: "categorical_crossentropy",
                optimizer: new Adam(0.0001F), metrics: new string[] { "accuracy" });
        }

        private static readonly Sequential _model;

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

        public void Train(List<Point> whiteHistory, List<Point> blackHistory)
        {
            var (x, y) = LoadRawData(whiteHistory, blackHistory);
            var h = _model.Fit(x, y,
                  epochs: 1,
                  verbose: 0);
            var loss = h.HistoryLogs["loss"].GetValue(0);
            var accuracy = h.HistoryLogs["accuracy"].GetValue(0);
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

            var pre = _model.Predict(np.array(arrX), verbose: 0).reshape(-1);


            float[,] res = new float[15, 15];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    res[i, j] = pre[i * 15 + j].item<float>();
                }
            }
            return res;
        }

        private (NDarray x, NDarray y) LoadRawData(List<Point> whiteHistory, List<Point> blackHistory)
        {
            var isBlack = blackHistory.Count > whiteHistory.Count;
            var totalSize = blackHistory.Count + whiteHistory.Count;
            int[,,,] arrX = new int[totalSize, 15, 15, 9];
            int[,] arrY = new int[totalSize, 225];
            int[,] mapWhite = new int[15, 15];
            int[,] mapBlack = new int[15, 15];

            for (int i = 0; i < blackHistory.Count; i++)
            {
                //构建黑色的输入参数

                //构建历史步数
                //构建黑色自己最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var blackHistCount = i - 1 - j;
                    if (blackHistCount >= 0)
                    {
                        arrX[i * 2, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j] = 1;
                    }
                }
                //构建白色最近走过的步数
                for (int j = 0; j < 3; j++)
                {
                    var whiteHistCount = i - 1 - j;
                    if (whiteHistCount >= 0)
                    {
                        arrX[i * 2, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j + 3] = 1;
                    }
                }

                //构建黑色目前局面的着点
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[i * 2, j, k, 6] = mapBlack[j, k];
                        if (mapBlack[j, k] == 1)
                        {
                            arrY[i * 2, j * 15 + k] = -1;
                        }
                    }
                }

                //构建白色目前局面的着点
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[i * 2, j, k, 7] = mapWhite[j, k];
                        if (mapWhite[j, k] == 1)
                        {
                            arrY[i * 2, j * 15 + k] = -1;
                        }
                    }
                }

                //是否先手
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        arrX[i * 2, j, k, 8] = 1;
                    }
                }

                //arrY[i * 2, blackHistory[i].X * 15 + blackHistory[i].Y] = isBlack ? 1 : -1;

                if (!isBlack)
                {
                    //构建白色的输入参数
                    //构建白色最近走过的步数
                    for (int j = 0; j < 3; j++)
                    {
                        var whiteHistCount = i - 1 - j;
                        if (whiteHistCount >= 0)
                        {
                            arrX[i * 2 + 1, whiteHistory[whiteHistCount].X, whiteHistory[whiteHistCount].Y, j] = 1;
                        }
                    }
                    //构建黑色最近走过的步数
                    for (int j = 0; j < 3; j++)
                    {
                        var blackHistCount = i - 1 - j;
                        if (blackHistCount >= 0)
                        {
                            arrX[i * 2 + 1, blackHistory[blackHistCount].X, blackHistory[blackHistCount].Y, j + 3] = 1;
                        }
                    }

                    //构建白色目前局面的着点
                    for (int j = 0; j < 15; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            arrX[i * 2 + 1, j, k, 6] = mapWhite[j, k];
                            if (mapWhite[j, k] == 1)
                            {
                                arrY[i * 2 + 1, j * 15 + k] = -1;
                            }
                        }
                    }

                    //构建黑色目前局面的着点
                    for (int j = 0; j < 15; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            arrX[i * 2 + 1, j, k, 7] = mapBlack[j, k];
                            if (mapBlack[j, k] == 1)
                            {
                                arrY[i * 2 + 1, j * 15 + k] = -1;
                            }
                        }
                    }

                    //arrY[i * 2 + 1, whiteHistory[i].X * 15 + whiteHistory[i].Y] = isBlack ? -1 : 1;

                    mapWhite[whiteHistory[i].X, whiteHistory[i].Y] = 1;
                }


                mapBlack[blackHistory[i].X, blackHistory[i].Y] = 1;

            }
            return (np.array(arrX), np.array(arrY));
        }
    }
}