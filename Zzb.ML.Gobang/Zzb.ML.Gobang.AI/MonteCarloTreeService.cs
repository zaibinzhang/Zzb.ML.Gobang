using System;
using System.Linq;
using Zzb.ML.EF;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTreeService
    {
        public MonteCarloTree GetBaseTree()
        {
            using var context = new ZzbContext();
            var tree = (from t in context.MonteCarloTrees where t.ParentTreeId == null select t).FirstOrDefault();
            if (tree != null)
            {
                return tree;
            }

            var one = context.MonteCarloTrees.Add(new MonteCarloTree() { IsBlack = false });
            context.SaveChanges();
            return one.Entity;
        }

        public MonteCarloTree GeTree(Guid id)
        {
            using var context = new ZzbContext();
            return (from t in context.MonteCarloTrees where t.MonteCarloTreeId == id select t).First();
        }

        public void Add(MonteCarloTree one)
        {
            using var context = new ZzbContext();
            context.MonteCarloTrees.Add(one);
            context.SaveChanges();
        }

        public MonteCarloTree GetMaxUCTTree(Guid id)
        {
            using var context = new ZzbContext();
            return (from t in context.MonteCarloTrees where t.ParentTreeId == id orderby t.UCT descending select t)
                .First();
        }
    }
}