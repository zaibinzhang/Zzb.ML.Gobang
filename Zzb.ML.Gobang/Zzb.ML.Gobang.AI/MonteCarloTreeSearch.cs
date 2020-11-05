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

        public static Action<string> Log { get; set; }

        private static MonteCarloTree _currentTree;

        private static Dictionary<long, MonteCarloTree> _addList = new Dictionary<long, MonteCarloTree>();

        private static List<MonteCarloTree> _updateList = new List<MonteCarloTree>();

        private static long _maxId = -1;

        public Point CalNext(int[,] map, bool isBlack)
        {
            if (_currentTree == null)
            {
                _currentTree = _service.GetBaseTree();
                MonteCarloTree.AllCount = _currentTree.Count;
            }

            if (_maxId < 0)
            {
                _maxId = _service.GetMaxId() + 1;
            }

            var list = GetEmptyPoints(map);

            var currentTrees = _service.GetTrees(_currentTree.MonteCarloTreeId);

            foreach (var point in list)
            {
                if (!(from t in currentTrees where t.X == point.X && t.Y == point.Y select t).Any())
                {
                    var one = new MonteCarloTree() { MonteCarloTreeId = _maxId++, ParentTreeId = _currentTree.MonteCarloTreeId, ParentTree = _currentTree, X = point.X, Y = point.Y, IsBlack = isBlack };
                    _addList.Add(one.MonteCarloTreeId, one);
                    QuickRun(map, !isBlack, one);
                    var temp = _currentTree;
                    while (temp != null)
                    {
                        _updateList.Add(temp);
                        temp = temp.ParentTree;
                    }
                    _service.Save(_addList.Values.ToList(), _updateList);
                    _addList = new Dictionary<long, MonteCarloTree>();
                    _updateList = new List<MonteCarloTree>();
                }
            }

            var trees = _service.GetTrees(_currentTree.MonteCarloTreeId).GroupBy(t => t.UCT).OrderByDescending(t => t.Key).First();
            var tree = (from t in trees orderby t.RandomNumber select t).First();

            if (GameWin.IsGameEnd(new Point(tree.X, tree.Y), isBlack ? 1 : 2, map))
            {
                MonteCarloTree.AllCount++;
                BackLoad(tree, isBlack, true);
                Log($"{(isBlack ? "黑棋" : "白棋")}下子【{tree.X + 1},{tree.Y + 1}】后胜利,{DateTime.Now}");
                _service.Save(_addList.Values.ToList(), _updateList);
                _currentTree = null;
                _updateList = new List<MonteCarloTree>();
                _service.Clear();
                return new Point(tree.X, tree.Y);
            }

            Log($"{(isBlack ? "黑棋" : "白棋")}下子【{tree.X + 1},{tree.Y + 1}】,{DateTime.Now}");
            _currentTree = tree;

            return new Point(tree.X, tree.Y);
        }

        private void QuickRun(int[,] map, bool isBlack, MonteCarloTree tempTree)
        {
            var list = GetEmptyPoints(map);
            var temp = list[new Random().Next(list.Count())];
            var one = new MonteCarloTree { MonteCarloTreeId = _maxId++, ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack, ParentTreeId = tempTree.MonteCarloTreeId };
            tempTree.MonteCarloTrees.Add(one);
            _addList.Add(one.MonteCarloTreeId, one);
            if (GameWin.IsGameEnd(new Point(temp.X, temp.Y), isBlack ? 1 : 2, map))
            {
                MonteCarloTree.AllCount++;
                BackLoad(one, isBlack);
                return;
            }

            map[one.Y, one.X] = isBlack ? 1 : 2;
            QuickRun(map, !isBlack, one);
            map[one.Y, one.X] = 0;
        }

        private void BackLoad(MonteCarloTree tree, bool isBlack, bool isUpdate = false)
        {
            if (tree != null)
            {
                tree.Count++;
                if (tree.IsBlack == isBlack)
                {
                    tree.Win++;
                }

                if (isUpdate)
                {
                    _updateList.Add(tree);
                }
                BackLoad(tree.ParentTree, isBlack, isUpdate);
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
