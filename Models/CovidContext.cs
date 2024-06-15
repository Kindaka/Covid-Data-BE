using System;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
namespace ODataCovid.Models
{
    public class CovidContext : DbContext
    {
        public CovidContext()
        {
        }

        public CovidContext(DbContextOptions<CovidContext> options) : base(options)
        {
        }

        public DbSet<CovidDaily> CovidDailies { get; set; }
        public DbSet<CountryRegion> CountryRegions { get; set; }
        public DbSet<Death> Deaths { get; set; }
        public DbSet<Active> Actives { get; set; }
        public DbSet<Recovered> Recovereds { get; set; }
        public DbSet<Confirmed> Confirmeds { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Confirmed>()
                .ToTable("Confirmeds");
            modelBuilder.Entity<Death>()
                .ToTable("Deaths");
            modelBuilder.Entity<Active>()
               .ToTable("Actives");
            modelBuilder.Entity<Recovered>()
               .ToTable("Recovereds");
            modelBuilder.Entity<CovidDaily>()
                .ToTable("CovidDaily")
                .HasOne<CountryRegion>(c=>c.CountryRegion);
            modelBuilder.Entity<CountryRegion>()
                .ToTable("CountryRegion")
                .HasMany<CovidDaily>(g=>g.CovidDailies);
            //modelBuilder.Entity<CovidDaily>().OwnsOne(c => c.CountryRegion);
            modelBuilder.Entity<CovidDaily>().Property(x => x.CountryRegionId).HasColumnName("countryregionId");
        }
    }
}

