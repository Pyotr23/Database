using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBNeon.Models
{
    public class NeonContext : DbContext
    {
        public NeonContext() : base("NeonConnection") { }
        public DbSet<Type> Types { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Procurement> Procurements { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Moving> Movings { get; set; }
    }
}
