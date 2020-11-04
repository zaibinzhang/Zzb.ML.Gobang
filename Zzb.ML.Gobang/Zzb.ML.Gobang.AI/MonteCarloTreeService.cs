using System;
using System.Collections.Generic;
using System.Linq;
using Zzb.ML.EF;

namespace Zzb.ML.Gobang.AI
{
    public class MonteCarloTreeService
    {
        private static readonly ZzbContext Context = new ZzbContext();
        public MonteCarloTree GetBaseTree()
        {
            var tree = (from t in Context.MonteCarloTrees where t.ParentTreeId == null select t).FirstOrDefault();
            if (tree != null)
            {
                return tree;
            }

            var one = Context.MonteCarloTrees.Add(new MonteCarloTree() { IsBlack = false });
            Context.SaveChanges();
            return one.Entity;
        }

        public MonteCarloTree GeTree(Guid id)
        {
            return (from t in Context.MonteCarloTrees where t.MonteCarloTreeId == id select t).First();
        }

        public void Add(MonteCarloTree one)
        {
            Context.MonteCarloTrees.Add(one);
            Context.SaveChanges();
        }

        public MonteCarloTree GetMaxUCTTree(Guid id)
        {
            return (from t in Context.MonteCarloTrees where t.ParentTreeId == id orderby t.UCT descending select t)
                .First();
        }

        public List<MonteCarloTree> GetTrees(Guid id)
        {
            return (from t in Context.MonteCarloTrees where t.ParentTreeId == id select t).ToList();
        }
    }
}