using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SuperFunction.Contexts
{
    public class CosmosContext : DbContext
    {
        public CosmosContext()
        {

        }

        public CosmosContext(DbContextOptions<CosmosContext> options) : base(options)
        {

        }

        public DbSet<Duck> Ober { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Duck>(entity =>
            {
                entity.ToContainer("uberContainer");
                entity.HasPartitionKey(item => item.dsfdsfggfdhfgh); // Replace YourPartitionKeyProperty with the actual property name.
            });
        }
    }
}
