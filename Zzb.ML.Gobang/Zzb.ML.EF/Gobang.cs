using System;

namespace Zzb.ML.EF
{
    public class Gobang : BaseModel
    {
        public Guid GobangId { get; set; } = Guid.NewGuid();

        public string Map { get; set; }

        public bool IsBlack { get; set; }

        public float Target { get; set; }

        public bool IsWin { get; set; }

    }
}