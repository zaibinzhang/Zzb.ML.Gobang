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
        public MonteCarloTree GetBaseTree(ZzbContext context)
        {

            var tree = (from t in context.MonteCarloTrees where t.ParentTreeId == null select t).FirstOrDefault();
            if (tree != null)
            {
                return tree;
            }

            var one = context.MonteCarloTrees.Add(new MonteCarloTree() { IsBlack = false });
            context.SaveChanges();
            return one.Entity;
        }

        public void Save(List<MonteCarloTree> addList, List<MonteCarloTree> updateList)
        {
            using var context = new ZzbContext();
            using var tran = context.Database.BeginTransaction();
            long time = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var temp in addList)
            {
                sb.Append($"insert into MonteCarloTrees values('{temp.MonteCarloTreeId}','{temp.ParentTreeId.Value}',{temp.X},{temp.Y},{temp.Count},{temp.Win},{(temp.IsBlack ? 1 : 0)});");
                time++;
                
                if (time > 100000)
                {
                    context.Database.ExecuteSqlRaw(sb.ToString());
                    sb = new StringBuilder();
                    time = 0;
                }
            }

            foreach (MonteCarloTree tree in updateList)
            {
                sb.Append(
                    $"update MonteCarloTrees set Count={tree.Count},Win={tree.Win} where MonteCarloTreeId='{tree.MonteCarloTreeId}'");
            }
            context.Database.ExecuteSqlRaw(sb.ToString());
            tran.Commit();
        }
    }
}