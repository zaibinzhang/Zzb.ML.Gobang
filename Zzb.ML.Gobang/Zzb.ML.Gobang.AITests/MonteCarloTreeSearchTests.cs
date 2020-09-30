using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zzb.ML.Gobang.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zzb.ML.Gobang.AI.Tests
{
    [TestClass()]
    public class MonteCarloTreeSearchTests
    {
        [TestMethod()]
        public void CalNextTest()
        {

            MonteCarloTreeSearch tree = new MonteCarloTreeSearch();
            MonteCarloTreeSearch.IsWin = (map, point, isBlack) =>
            {
                GameBoard board = new GameBoard();
                board.map = map;
                board.map[point.Y, point.X] = isBlack ? 1 : 2;
                return board.IsGameEnd(point);
            };
            tree.CalNext(new int[15, 15], true);
            Assert.IsTrue(true);
        }
    }
}