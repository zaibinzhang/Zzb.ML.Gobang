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

        private static List<MonteCarloTree> _list = new List<MonteCarloTree>();

        public Point CalNext(int[,] map, bool isBlack)
        {
            if (_currentTree == null)
            {
                _currentTree = _tree;
            }

            DateTime dt = DateTime.Now;


            while (dt.AddSeconds(1) > DateTime.Now)
            {
                Run(map, isBlack, _currentTree);
            }

            var tree = _currentTree.Trees.OrderByDescending(t => (double)t.Win / t.Count).FirstOrDefault();

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

            var maxUCT = tempTree.Trees.OrderByDescending(t => t.UCT).FirstOrDefault();

            if (maxUCT.UCT == 0.5)
            {
                var temps = tempTree.Trees.Where(t => t.UCT == maxUCT.UCT);

                maxUCT = temps.ToArray()[new Random().Next(temps.Count())];
            }


            if (maxUCT != null)
            {

                if (GameWin.IsGameEnd(new Point(maxUCT.X, maxUCT.Y), isBlack ? 1 : 2, map))
                {
                    if (maxUCT.IsEnd)
                    {
                        return;
                    }

                    MonteCarloTree.AllCount++;
                    maxUCT.IsEnd = true;
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
