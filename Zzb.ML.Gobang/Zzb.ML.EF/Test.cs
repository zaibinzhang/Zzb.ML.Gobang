using System;

namespace Zzb.ML.EF
{
    public class Test : BaseModel
    {
        public Guid TestId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Name1 { get; set; }

        public string Name2 { get; set; }

        public string Name3 { get; set; }

        public string Name4 { get; set; }
        public string Name5 { get; set; }

        public string Name6 { get; set; }

        public string Name7 { get; set; }

        public string Name8 { get; set; }

    }
}