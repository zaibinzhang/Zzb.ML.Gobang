using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zzb.ML.EF
{
    public class MonteCarloTree
    {
        public Guid MonteCarloTreeId { get; set; }

        public Guid? ParentTreeId { get; set; }

        [ForeignKey("ParentTreeId")]
        public virtual MonteCarloTree ParentTree { get; set; }

        [InverseProperty("ParentTree")]
        public virtual List<MonteCarloTree> MonteCarloTrees { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public long Count { get; set; }

        public long Win { get; set; }

        public double UCT { get; set; }

        public bool IsBlack { get; set; }
    }
}