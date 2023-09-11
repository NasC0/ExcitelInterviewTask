namespace Leads.Database.Ef
{
    using Microsoft.EntityFrameworkCore;
    using Leads.Database.Ef.Models;

    public class LeadsContext : DbContext
    {
        public LeadsContext(DbContextOptions<LeadsContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads { get; set; }
        public DbSet<SubArea> SubAreas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lead>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<Lead>().HasIndex(i => i.Id);
            modelBuilder.Entity<Lead>()
                .Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<Lead>()
                .Property(p => p.SubAreaId)
                .IsRequired();
            modelBuilder.Entity<Lead>()
                .Property(p => p.PinCode)
                .IsRequired();
            modelBuilder.Entity<Lead>()
                .Property(p => p.Address)
                .IsRequired();

            modelBuilder.Entity<SubArea>().HasKey(k => k.Id);
            modelBuilder.Entity<SubArea>().HasIndex(i => i.Id);
            modelBuilder.Entity<SubArea>().HasIndex(i => i.PinCode);
            modelBuilder.Entity<SubArea>()
                .Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<SubArea>()
                .Property(p => p.PinCode)
                .IsRequired();

            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 1, PinCode = "123", Name = "Name1" });
            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 2, PinCode = "123", Name = "Name2" });
            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 3, PinCode = "123", Name = "Name3" });
            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 4, PinCode = "567", Name = "Name4" });
            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 5, PinCode = "567", Name = "Name5" });
            modelBuilder.Entity<SubArea>().HasData(new SubArea { Id = 6, PinCode = "567", Name = "Name6" });
        }
    }
}
