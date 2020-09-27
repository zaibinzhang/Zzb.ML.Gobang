using System;

namespace Zzb.ML.EF
{
    public class BaseModel
    {
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public bool IsEnable { get; set; } = true;
    }
}