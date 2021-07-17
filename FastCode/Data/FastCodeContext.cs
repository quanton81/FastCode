using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FastCode.Models;

namespace FastCode.Data
{
    public class FastCodeContext : DbContext
    {
        public FastCodeContext (DbContextOptions<FastCodeContext> options)
            : base(options)
        {
        }

        public DbSet<FastCode.Models.Dipendente> Dipendente { get; set; }
    }
}
