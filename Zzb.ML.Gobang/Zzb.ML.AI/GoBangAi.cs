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
        public static int GameSize => 6;

        static GoBangAi()
        {
            _model = new Sequential();
            _model.Add(new Conv2D(32, kernel_size: (5, 5).ToTuple(),
                activation: "relu",
                input_shape: (GameSize, GameSize, 3), padding: "same"));
            _model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Conv2D(128, kernel_size: (3, 3).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Conv2D(4, kernel_size: (1, 1).ToTuple(),
                activation: "relu", padding: "same"));
            _model.Add(new Flatten());
            _model.Add(new Dense(GameSize * GameSize, activation: "softmax"));
            _model.Compile(loss: "categorical_crossentropy",
                optimizer: new Adam(2e-4f), metrics: new string[] { "accuracy" });
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

        public (double loss, double accuracy) Train(List<Point> whiteHistory, List<Point> blackHistory, bool isWin = true)
        {
            var (x, y) = LoadRawData(whiteHistory, blackHistory, isWin);
            var h = _model.Fit(x, y,
                  epochs: 1,
                  verbose: 0);
            var loss = h.HistoryLogs["loss"].GetValue(h.Epoch.Length - 1);
            var accuracy = h.HistoryLogs["accuracy"].GetValue(h.Epoch.Length - 1);
            return ((double)loss, (double)accuracy);
        }

        public float[,] Predict(List<Point> whiteHistory, List<Point> blackHistory)
        {
            var isBlackGo = blackHistory.Count == whiteHistory.Count;
            float[,,,] arrX = new float[1, GameSize, GameSize, 3];
            int[,] mapWhite = new int[GameSize, GameSize];
            int[,] mapBlack = new int[GameSize, GameSize];

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
                ////构建历史步数

                //黑棋棋面
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[0, j, k, 0] = mapBlack[j, k];
                    }
                }

                //白棋棋面
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[0, j, k, 1] = mapWhite[j, k];
                    }
                }

                //是否先手
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[0, j, k, 2] = 1;
                    }
                }

            }
            else
            {
                //构建历史步数
                //白棋棋面
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[0, j, k, 0] = mapWhite[j, k];
                    }
                }

                //黑棋棋面
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[0, j, k, 1] = mapBlack[j, k];
                    }
                }

            }

            var pre = _model.Predict(np.array(arrX), verbose: 0).reshape(-1);


            float[,] res = new float[GameSize, GameSize];

            for (int i = 0; i < GameSize; i++)
            {
                for (int j = 0; j < GameSize; j++)
                {
                    res[i, j] = pre[i * GameSize + j].item<float>();
                }
            }
            return res;
        }

        private (NDarray x, NDarray y) LoadRawData(List<Point> whiteHistory, List<Point> blackHistory, bool isWin)
        {
            var isBlack = blackHistory.Count > whiteHistory.Count;
            var totalSize = blackHistory.Count + whiteHistory.Count;
            int[,,,] arrX = new int[totalSize, GameSize, GameSize, 3];
            int[,] arrY = new int[totalSize, GameSize * GameSize];
            int[,] mapWhite = new int[GameSize, GameSize];
            int[,] mapBlack = new int[GameSize, GameSize];

            for (int i = 0; i < blackHistory.Count; i++)
            {
                //构建黑色的输入参数

                //构建黑色目前局面的着点
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[i * 2, j, k, 0] = mapBlack[j, k];
                    }
                }

                //构建白色目前局面的着点
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[i * 2, j, k, 1] = mapWhite[j, k];
                    }
                }

                //是否先手
                for (int j = 0; j < GameSize; j++)
                {
                    for (int k = 0; k < GameSize; k++)
                    {
                        arrX[i * 2, j, k, 2] = 1;
                    }
                }

                if (isWin)
                {
                    arrY[i * 2, blackHistory[i].X * GameSize + blackHistory[i].Y] = isBlack ? 1 : -1;
                }

                mapBlack[blackHistory[i].X, blackHistory[i].Y] = 1;

                if (!isBlack)
                {
                    //构建白色的输入参数
                    //构建白色目前局面的着点
                    for (int j = 0; j < GameSize; j++)
                    {
                        for (int k = 0; k < GameSize; k++)
                        {
                            arrX[i * 2 + 1, j, k, 0] = mapWhite[j, k];
                        }
                    }

                    //构建黑色目前局面的着点
                    for (int j = 0; j < GameSize; j++)
                    {
                        for (int k = 0; k < GameSize; k++)
                        {
                            arrX[i * 2 + 1, j, k, 1] = mapBlack[j, k];
                        }
                    }

                    if (isWin)
                    {
                        arrY[i * 2 + 1, whiteHistory[i].X * GameSize + whiteHistory[i].Y] = isBlack ? -1 : 1;
                    }

                    mapWhite[whiteHistory[i].X, whiteHistory[i].Y] = 1;
                }
            }
            return (np.array(arrX), np.array(arrY));
        }
    }
}