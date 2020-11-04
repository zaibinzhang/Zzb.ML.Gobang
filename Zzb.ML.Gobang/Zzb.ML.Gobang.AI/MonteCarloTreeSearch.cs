using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zzb.ML.EF;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTreeSearch
    {
        private MonteCarloTreeService _service = new MonteCarloTreeService();

        private static Guid _currentId = Guid.Empty;

        public Point CalNext(int[,] map, bool isBlack)
        {
            if (_currentId == Guid.Empty)
            {
                var baseTree = _service.GetBaseTree();
                _currentId = baseTree.MonteCarloTreeId;
                MonteCarloTree.AllCount = baseTree.Count;
            }

            var list = GetEmptyPoints(map);

            var currentTree = _service.GeTree(_currentId);

            foreach (var point in list)
            {
                if (!(from t in currentTree.MonteCarloTrees where t.X == point.X && t.Y == point.Y select t).Any())
                {
                    var one = new MonteCarloTree() { ParentTreeId = _currentId, ParentTree = currentTree, X = point.X, Y = point.Y, IsBlack = isBlack };
                    QuickRun(map, isBlack, one);
                    _service.Add(one);
                }
            }

            var tree = _service.GetMaxUCTTree(_currentId);

            if (GameWin.IsGameEnd(new Point(tree.X, tree.Y), isBlack ? 1 : 2, map))
            {
                _currentId = Guid.Empty;
                BackLoad(tree, isBlack);
                MonteCarloTree.AllCount++;
                return new Point(tree.X, tree.Y);
            }

            _currentId = tree.MonteCarloTreeId;
            return new Point(tree.X, tree.Y);

            //foreach (var point in list)
            //{
            //    if (!(from t in _currentTree.Trees where t.X == point.X && t.Y == point.Y select t).Any())
            //    {
            //        var one = new MonteCarloTree { ParentTree = _currentTree, X = point.X, Y = point.Y, IsBlack = isBlack };
            //        _currentTree.Trees.Add(one);
            //        _treeCount++;
            //        QuickRun(map, isBlack, one);
            //    }
            //}

            //var tree = _currentTree.Trees.OrderByDescending(t => t.UCT).First();

            //var temps = (from t in _currentTree.Trees where t.UCT == tree.UCT select t).ToList();

            //tree = temps[new Random().Next(temps.Count())];

            //if (GameWin.IsGameEnd(new Point(tree.X, tree.Y), isBlack ? 1 : 2, map))
            //{
            //    _currentTree = null;
            //    MonteCarloTree.AllCount++;
            //    BackLoad(tree, isBlack);
            //    return new Point(tree.X, tree.Y);
            //}

            //_currentTree = tree;
            //return new Point(tree.X, tree.Y);

            throw new Exception("1");
        }

        private void QuickRun(int[,] map, bool isBlack, MonteCarloTree tempTree)
        {
            var list = GetEmptyPoints(map);
            var temp = list[new Random().Next(list.Count())];
            var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack };
            tempTree.MonteCarloTrees.Add(one);
            if (GameWin.IsGameEnd(new Point(temp.X, temp.Y), isBlack ? 1 : 2, map))
            {
                BackLoad(one, isBlack);
                MonteCarloTree.AllCount++;
                return;
            }

            map[one.Y, one.X] = isBlack ? 1 : 2;
            QuickRun(map, !isBlack, one);
            map[one.Y, one.X] = 0;
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
                tree.UpdateUCT();
            }
        }
        //private void Run(int[,] map, bool isBlack, MonteCarloTree tempTree = null)
        //{
        //    tempTree ??= _tree;

        //    var maxUCT = tempTree.Trees.OrderByDescending(t => t.UCT).FirstOrDefault();

        //    if (maxUCT == null || maxUCT.UCT < 0.5)
        //    {
        //        var list = GetEmptyPoints(map);
        //        var temps = (from l in list from t in tempTree.Trees where t.X == l.X && t.Y == l.Y select l).ToList();

        //        foreach (var point in temps)
        //        {
        //            list.Remove(point);
        //        }

        //        var temp = list[new Random().Next(list.Count())];
        //        var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack };
        //        tempTree.Trees.Add(one);
        //        maxUCT = one;
        //        _treeCount++;

        //    }

        //    if (GameWin.IsGameEnd(new Point(maxUCT.X, maxUCT.Y), isBlack ? 1 : 2, map))
        //    {
        //        MonteCarloTree.AllCount++;
        //        BackLoad(maxUCT, isBlack);
        //        return;
        //    }

        //    map[maxUCT.Y, maxUCT.X] = isBlack ? 1 : 2;
        //    Run(map, !isBlack, maxUCT);
        //    map[maxUCT.Y, maxUCT.X] = 0;
        //}


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
