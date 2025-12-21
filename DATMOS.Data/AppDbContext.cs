using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DATMOS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This method is only used when DbContext is created without DI
            // For production, use DI configuration in WebEntryPoint and WinApp
            if (!optionsBuilder.IsConfigured)
            {
                // Default to PostgreSQL for development
                optionsBuilder.UseNpgsql("Host=localhost;Database=datmos_db;Username=postgres;Password=password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);

                // Index for better query performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}
