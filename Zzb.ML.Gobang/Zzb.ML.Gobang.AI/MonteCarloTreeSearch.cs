using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTreeSearch
    {
        private static MonteCarloTree _tree = new MonteCarloTree();

        private static MonteCarloTree _currentTree;

        private static long _treeCount = 0;

        //private static List<MonteCarloTree> _list = new List<MonteCarloTree>();

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

            //var tree = _currentTree.Trees.FirstOrDefault(t => t.IsEnd && t.Win == t.Count);

            //if (tree != null)
            //{
            //    _currentTree = tree;
            //    return new Point(tree.X, tree.Y);
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

            //lock (_tree)
            //{
            //    var list = GetEmptyPoints(map);

            //    if (!tempTree.Trees.Any())
            //    {
            //        foreach (var point in list)
            //        {
            //            var one = new MonteCarloTree { ParentTree = tempTree, X = point.X, Y = point.Y, IsBlack = isBlack };
            //            tempTree.Trees.Add(one);
            //            _treeCount++;
            //            //_list.Add(one);
            //        }
            //    }
            //}

            var maxUCT = tempTree.Trees.OrderByDescending(t => t.UCT).FirstOrDefault();

            //if (maxUCT.UCT == 0.5)
            //{
            //    var temps = tempTree.Trees.Where(t => t.UCT == maxUCT.UCT);

            //    maxUCT = temps.ToArray()[new Random().Next(temps.Count())];
            //}
            if (maxUCT == null || maxUCT.UCT < 0.5)
            {
                var list = GetEmptyPoints(map);
                var temps = (from l in list from t in tempTree.Trees where t.X == l.X && t.Y == l.Y select l).ToList();

                foreach (var point in temps)
                {
                    list.Remove(point);
                }

                var temp = list[new Random().Next(list.Count())];
                var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack };
                tempTree.Trees.Add(one);
                maxUCT = one;
                _treeCount++;

            }
            //if (maxUCT != null && maxUCT.UCT < 0.5)
            //{
            //    var temp = maxUCT.ListPoints[new Random().Next(maxUCT.ListPoints.Count())];
            //    maxUCT.ListPoints.Remove(temp);
            //    var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack, ListPoints = list };
            //    tempTree.Trees.Add(one);
            //}

            //if (maxUCT == null)
            //{
            //    var list = GetEmptyPoints(map);

            //    var temp = list[new Random().Next(list.Count())];
            //    list.Remove(temp);
            //    var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack, ListPoints = list };
            //    tempTree.Trees.Add(one);
            //    maxUCT = one;
            //    _treeCount++;
            //}

            if (GameWin.IsGameEnd(new Point(maxUCT.X, maxUCT.Y), isBlack ? 1 : 2, map))
            {
                //if (maxUCT.IsEnd)
                //{
                //    return;
                //}

                //maxUCT.IsEnd = true;

                MonteCarloTree.AllCount++;
                //maxUCT.ParentTree.ParentTree.IsEnd = true;
                BackLoad(maxUCT, isBlack);
                return;
            }

            map[maxUCT.Y, maxUCT.X] = isBlack ? 1 : 2;
            Run(map, !isBlack, maxUCT);
            map[maxUCT.Y, maxUCT.X] = 0;
        }

        private void BackLoad(MonteCarloTree tree, bool isBlack)
        {
            if (tree != null)
            {
                //if (tree.IsEnd && tree.ParentTree?.ParentTree != null)
                //{
                //    if (tree.ParentTree.Trees.All(t => t.IsEnd))
                //    {
                //        tree.ParentTree.ParentTree.IsEnd = true;
                //    }
                //}
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
