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

        private static Dictionary<Guid, MonteCarloTree> _addList = new Dictionary<Guid, MonteCarloTree>();

        private static List<MonteCarloTree> _updateList = new List<MonteCarloTree>();

        private static ZzbContext context = new ZzbContext();

        public Point CalNext(int[,] map, bool isBlack)
        {
            if (_currentTree == null)
            {
                _currentTree = _service.GetBaseTree(context);
                MonteCarloTree.AllCount = _currentTree.Count;
            }

            if (!_addList.ContainsKey(_currentTree.MonteCarloTreeId))
            {
                _updateList.Add(_currentTree);
            }

            var list = GetEmptyPoints(map);

            foreach (var point in list)
            {
                if (!(from t in _currentTree.MonteCarloTrees where t.X == point.X && t.Y == point.Y select t).Any())
                {
                    var one = new MonteCarloTree() { ParentTreeId = _currentTree.MonteCarloTreeId, ParentTree = _currentTree, X = point.X, Y = point.Y, IsBlack = isBlack };
                    QuickRun(map, !isBlack, one);
                    _currentTree.MonteCarloTrees.Add(one);
                    _addList.Add(one.MonteCarloTreeId, one);
                }
            }

            //var trees = (from t in _currentTree.MonteCarloTrees orderby t.UCT descending select t).ToList();
            //var tree = trees.OrderByDescending(t => t.MonteCarloTreeId).First();
            var trees = _currentTree.MonteCarloTrees.GroupBy(t => t.UCT).OrderByDescending(t => t.Key).First();
            var tree = (from t in trees orderby t.MonteCarloTreeId select t).First();

            if (GameWin.IsGameEnd(new Point(tree.X, tree.Y), isBlack ? 1 : 2, map))
            {
                MonteCarloTree.AllCount++;
                BackLoad(tree, isBlack);
                _service.Save(_addList.Values.ToList(), _updateList);
                Log($"{(isBlack ? "黑棋" : "白棋")}下子【{tree.X + 1},{tree.Y + 1}】后胜利，新增了[{_addList.Count}]条数据，更新了[{_updateList.Count}]条数据,{DateTime.Now}");
                _currentTree = null;
                _addList = new Dictionary<Guid, MonteCarloTree>();
                _updateList = new List<MonteCarloTree>();
                context.Dispose();
                context = new ZzbContext();
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
            var one = new MonteCarloTree { ParentTree = tempTree, X = temp.X, Y = temp.Y, IsBlack = isBlack, ParentTreeId = tempTree.MonteCarloTreeId };
            tempTree.MonteCarloTrees.Add(one);
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
