using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Zzb.ML.EF
{
    public class ZzbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connect = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ML;Data Source=LAPTOP-LSCIN6QR\\SQLEXPRESS";
            String connectFile = Path.Combine(AppContext.BaseDirectory, "ConnectString.txt");
            if (File.Exists(connectFile))
            {
                connect = File.ReadAllText(connectFile);
            }
            options.UseLazyLoadingProxies().UseSqlServer(connect);
        }

        public DbSet<Test> Tests { get; set; }

        public static ZzbContext CreateContext()
        {
            return new ZzbContext();
        }
    }
}
