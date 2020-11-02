using System.Drawing;

namespace Zzb.ML.Gobang.AI
{
    public class GameWin
    {
        //游戏落子记录表，0表示空，1表示黑棋，2表示白棋
        public static int[,] map = new int[gameSize, gameSize];
        //size*size个点的游戏，横向size个点，纵向size个点
        private const int gameSize = 15;
        //横向计数
        private static int hSum;
        //纵向计数
        private static int vSum;
        //左斜线计数
        private static int lSum;
        //右斜线计数
        private static int rSum;
        /// <summary>
        /// 判断游戏是否结束
        /// </summary>
        /// <param name="p">内部坐标点</param>
        /// <returns></returns>
        public static bool IsGameEnd(Point p, int color, int[,] map1)
        {
            map = map1;
            AllDirectionsCount(p.X, p.Y, color);
            if (hSum >= 5 || vSum >= 5 || lSum >= 5 || rSum >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 各个方向上计数
        /// </summary>
        /// <param name="xpos">横坐标</param>
        /// <param name="ypos">纵坐标</param>
        /// <param name="color">棋子颜色</param>
        private static void AllDirectionsCount(int xpos, int ypos, int color)
        {
            //水平方向的计数
            hSum = 1;
            for (int i = xpos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }
            for (int i = xpos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }

            //纵向的计数
            vSum = 1;
            for (int i = ypos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }
            for (int i = ypos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }

            //左斜线计数
            lSum = 1;
            for (int i = xpos - 1, j = ypos - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            for (int i = xpos + 1, j = ypos + 1; i <= gameSize && j <= gameSize; i++, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            //右斜线的判断
            rSum = 1;
            for (int i = xpos - 1, j = ypos + 1; i >= 0 && j <= gameSize; i--, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;
            }
            for (int i = xpos + 1, j = ypos - 1; i <= gameSize && j >= 0; i++, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;//横向的计数
            }
        }

        /// <summary>
        /// 在没有越界的基础上，是否存在同颜色的棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="color">棋子颜色</param>
        /// <returns></returns>
        private static bool ExistSameColor(int x, int y, int color)
        {
            return NoCrossBorder(x, y) && map[y, x] == color;
        }

        /// <summary>
        /// 判断一个坐标是否没越界
        /// </summary>
        /// 
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns></returns>
        private static bool NoCrossBorder(int x, int y)
        {
            return x >= 0 && x < gameSize && y >= 0 && y < gameSize;
        }
    }
}