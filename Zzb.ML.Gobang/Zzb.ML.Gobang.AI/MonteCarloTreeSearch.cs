using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTreeSearch
    {
        private static MonteCarloTree _tree = new MonteCarloTree();

        private static MonteCarloTree _currentTree;

        public static Func<int[,], Point, bool, bool> IsWin;

        private static List<MonteCarloTree> _list = new List<MonteCarloTree>();

        public Point CalNext(int[,] map, bool isBlack)
        {
            if (_currentTree == null)
            {
                _currentTree = _tree;
            }

            DateTime dt = DateTime.Now;

    
            while (dt.AddSeconds(5) > DateTime.Now)
            {
                Run(map, isBlack, _currentTree);
            }
            //bool[] isEnd = new bool[10];
            //for (int i = 0; i < isEnd.Length; i++)
            //{
            //    isEnd[i] = false;
            //    new Task((t) =>
            //    {
            //        int tt = (int)t;
            //        while (dt.AddSeconds(5) > DateTime.Now)
            //        {
            //            Run(map, isBlack, _currentTree);
            //        }

            //        isEnd[tt] = true;
            //    }, i).Start();

            //}

            //while (isEnd.Any(t => !t))
            //{
            //    Thread.Sleep(100);
            //}

            var tree = _currentTree.Trees.OrderByDescending(t => t.UCT).FirstOrDefault();

            if (tree != null)
            {
                _currentTree = tree;
                return new Point(tree.X, tree.Y);
            }

            return new Point();
        }

        private void Run(int[,] map, bool isBlack, MonteCarloTree tempTree = null)
        {
            tempTree ??= _tree;

            lock (_tree)
            {
                var list = GetEmptyPoints(map);

                if (!tempTree.Trees.Any())
                {
                    foreach (var point in list)
                    {
                        var one = new MonteCarloTree { ParentTree = tempTree, X = point.X, Y = point.Y, IsBlack = isBlack };
                        tempTree.Trees.Add(one);
                        _list.Add(one);
                    }
                }
            }

            var temp = tempTree.Trees.OrderByDescending(t => t.UCT).FirstOrDefault();

            var temps = tempTree.Trees.Where(t => t.UCT == temp.UCT);

            var maxUCT = temps.ToArray()[new Random().Next(temps.Count())];

            if (maxUCT != null)
            {
                if (IsWin(map, new Point(maxUCT.X, maxUCT.Y), isBlack))
                {
                    BackLoad(maxUCT, isBlack);
                    return;
                }

                map[maxUCT.Y, maxUCT.X] = isBlack ? 1 : 2;
                Run(map, !isBlack, maxUCT);
                map[maxUCT.Y, maxUCT.X] = 0;
            }
        }

        private void BackLoad(MonteCarloTree tree, bool isBlack)
        {
            if (tree != null)
            {
                tree.Count++;
                if (tree.IsBlack == isBlack)
                {
                    tree.Win++;
                }
                BackLoad(tree.ParentTree, isBlack);
            }
        }

        private List<Point> GetEmptyPoints(int[,] map)
        {
            List<Point> list = new List<Point>();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (map[i, j] == 0)
                    {
                        list.Add(new Point(j, i));
                    }
                }
            }

            return list;
        }
    }


}
