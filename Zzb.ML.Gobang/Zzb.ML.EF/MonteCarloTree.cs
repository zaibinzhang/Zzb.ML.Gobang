﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zzb.ML.EF
{
    public class MonteCarloTree
    {
        private static Random _random = new Random();
        public static long AllCount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MonteCarloTreeId { get; set; }

        public long? ParentTreeId { get; set; }

        [ForeignKey("ParentTreeId")]
        public virtual MonteCarloTree ParentTree { get; set; }

        [InverseProperty("ParentTree")]
        public virtual List<MonteCarloTree> MonteCarloTrees { get; set; } = new List<MonteCarloTree>();

        public int X { get; set; }

        public int Y { get; set; }

        public long Count { get; set; }

        public long Win { get; set; }

        public double UCT => (double)this.Win / this.Count + Math.Sqrt(Math.Log10(AllCount) / Count);

        public bool IsBlack { get; set; }

        public double RandomNumber => _random.NextDouble();
    }
}