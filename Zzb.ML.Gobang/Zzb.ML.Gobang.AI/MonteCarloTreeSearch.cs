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

        private static Guid _currentId = Guid.Empty;

        public Point CalNext(int[,] map, bool isBlack)
        {
            //List<MonteCarloTree> list1 = new List<MonteCarloTree>();
            //for (int i = 0; i < 100000; i++)
            //{
            //    list1.Add(new MonteCarloTree());
            //}

            //using (var context=new ZzbContext())
            //{
            //    context.MonteCarloTrees.AddRange(list1);
            //    context.SaveChanges();
            //}

            if (_currentId == Guid.Empty)
            {
                var baseTree = _service.GetBaseTree();
                _currentId = baseTree.MonteCarloTreeId;
                MonteCarloTree.AllCount = baseTree.Count;
            }

            var list = GetEmptyPoints(map);

            var currentTree = _service.GeTree(_currentId);

            var trees = _service.GetTrees(_currentId);

            foreach (var point in list)
            {
                if (!(from t in trees where t.X == point.X && t.Y == point.Y select t).Any())
                {
                    var one = new MonteCarloTree() { ParentTreeId = _currentId, ParentTree = currentTree, X = point.X, Y = point.Y, IsBlack = isBlack };
                    QuickRun(map, !isBlack, one);
                    _service.Add(one);
                }
            }

            trees = _service.GetTrees(_currentId);

            var tree = (from t in trees orderby t.UCT descending select t).First();

            if (GameWin.IsGameEnd(new Point(tree.X, tree.Y), isBlack ? 1 : 2, map))
            {
                _currentId = Guid.Empty;
                MonteCarloTree.AllCount++;
                BackLoad(tree, isBlack);
                return new Point(tree.X, tree.Y);
            }

            Log($"{(isBlack ? "黑棋" : "白棋")}下子【{tree.X + 1},{tree.Y + 1}】,{DateTime.Now}");
            _currentId = tree.MonteCarloTreeId;
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
