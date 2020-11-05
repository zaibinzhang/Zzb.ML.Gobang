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
        private static ZzbContext _context = new ZzbContext();

        public void Clear()
        {
            _context.Dispose();
            _context = new ZzbContext();
        }

        public long GetMaxId()
        {
            return (from t in _context.MonteCarloTrees orderby t.MonteCarloTreeId descending select t).First()
                .MonteCarloTreeId;
        }

        public MonteCarloTree GetBaseTree()
        {
            var tree = (from t in _context.MonteCarloTrees where t.ParentTreeId == null select t).FirstOrDefault();
            if (tree != null)
            {
                return tree;
            }

            var one = _context.MonteCarloTrees.Add(new MonteCarloTree() { IsBlack = false });
            _context.SaveChanges();
            return one.Entity;
        }

        public List<MonteCarloTree> GetTrees(long id)
        {
            return (from t in _context.MonteCarloTrees where t.ParentTreeId == id select t).ToList();
        }

        public void Save(List<MonteCarloTree> addList, List<MonteCarloTree> updateList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var temp in addList)
            {
                sb.Append($"insert into MonteCarloTrees values('{temp.MonteCarloTreeId}','{temp.ParentTreeId.Value}',{temp.X},{temp.Y},{temp.Count},{temp.Win},{(temp.IsBlack ? 1 : 0)});");
            }

            foreach (MonteCarloTree tree in updateList)
            {
                sb.Append(
                    $"update MonteCarloTrees set Count={tree.Count},Win={tree.Win} where MonteCarloTreeId='{tree.MonteCarloTreeId}'");
            }
            _context.Database.ExecuteSqlRaw(sb.ToString());
        }
    }
}