using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTree
    {
        public static long AllCount { get; set; } = 0;

        public double UCT
        {
            get
            {
                if (this.Count == 0)
                {
                    return 0.5;
                }

                return (double)this.Win / this.Count + Math.Sqrt(Math.Log10(AllCount) / Count);
            }
        }

        /// <summary>
        /// 当前下棋点X
        /// </summary>
        public int X { get; set; } = -1;

        /// <summary>
        /// 当前下棋点Y
        /// </summary>
        public int Y { get; set; } = -1;

        public List<MonteCarloTree> Trees { get; set; } = new List<MonteCarloTree>();

        public long Count { get; set; }

        public long Win { get; set; }

        public MonteCarloTree ParentTree { get; set; }

        public bool IsBlack { get; set; }

        public bool IsEnd { get; set; } = false;

        public List<Point> ListPoints = new List<Point>();
    }
}