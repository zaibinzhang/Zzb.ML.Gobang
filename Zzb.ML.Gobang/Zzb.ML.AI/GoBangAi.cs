using System.Drawing;
using System.Threading.Channels;

namespace Zzb.ML.AI
{
    public class GoBangAi
    {
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
            var isBlack = blackHistory.Count > whiteHistory.Count;
            var totalSize = blackHistory.Count + whiteHistory.Count;
            int[,,,] arrX = new int[totalSize, 15, 15, 9];
            int[,,] arrY = new int[totalSize, 15, 15];
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
                            arrY[i * 2, j, k] = -1;
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
                            arrY[i * 2, j, k] = -1;
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

                arrY[i * 2, blackHistory[i].X, blackHistory[i].Y] = isBlack ? 1 : -1;

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
                                arrY[i * 2 + 1, j, k] = -1;
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
                                arrY[i * 2 + 1, j, k] = -1;
                            }
                        }
                    }

                    arrY[i * 2 + 1, whiteHistory[i].X, whiteHistory[i].Y] = isBlack ? -1 : 1;

                    mapWhite[whiteHistory[i].X, whiteHistory[i].Y] = 1;
                }


                mapBlack[blackHistory[i].X, blackHistory[i].Y] = 1;

            }


        }
    }
}