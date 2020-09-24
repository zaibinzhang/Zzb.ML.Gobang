using System;

namespace Zzb.ML.GameComponent
{
    public class ChessmenNumChangeEventArgs: EventArgs
    {
        /// <summary>
        /// 棋子变化后的数目
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 棋子的颜色
        /// </summary>
        public int Color { get; set; }
        public ChessmenNumChangeEventArgs(int num, int color)
        {
            this.Num = num;
            this.Color = color;
        }
    }
}
