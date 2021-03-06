using FastCode.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastCode.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FastCodeContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FastCodeContext>>()))
            {
                // Look for any movies.
                if (context.Dipendente.Any())
                {
                    return;   // DB has been seeded
                }

                context.Dipendente.AddRange(
                    new Dipendente
                    {
                        Nome = "Marco",
                        Matricola = "001",
                        Manager = 1
                    },

                    new Dipendente
                    {
                        Nome = "Fabio",
                        Matricola = "002",
                        Manager = 1
                    },

                    new Dipendente
                    {
                        Nome = "Luca",
                        Matricola = "003",
                        Manager = 1
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
