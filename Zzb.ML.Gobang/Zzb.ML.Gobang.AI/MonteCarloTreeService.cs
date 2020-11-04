using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
            StringBuilder sb = new StringBuilder();
            var temp = one;
            while (temp != null)
            {
                sb.Append($"insert into MonteCarloTrees values('{temp.MonteCarloTreeId}','{temp.ParentTreeId.Value}',{temp.X},{temp.Y},{temp.Count},{temp.Win},{(temp.IsBlack ? 1 : 0)});");
                if (temp.MonteCarloTrees.Any())
                {
                    temp = temp.MonteCarloTrees[0];
                }
                else
                {
                    temp = null;
                }
            }

            Context.Database.ExecuteSqlCommand(sb.ToString());
            Context.SaveChanges();
        }
        public List<MonteCarloTree> GetTrees(Guid id)
        {
            return (from t in Context.MonteCarloTrees where t.ParentTreeId == id select t).ToList();
        }
    }
}