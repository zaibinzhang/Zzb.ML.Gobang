using System;

namespace Zzb.ML.EF
{
    public class Test : BaseModel
    {
        public Guid TestId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
    }
}