using DATMOS.Core.Entities;
using DATMOS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DATMOS.Data
{
    public class AppDbContext : IdentityDbContext<AddUsers>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Footer> Footers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ExamSubject> ExamSubjects { get; set; }
        public DbSet<ExamList> ExamLists { get; set; }
        public DbSet<ExamProject> ExamProjects { get; set; }
        // public DbSet<ExamTask> ExamTasks { get; set; } // Temporarily removed to fix migration

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

            // Configure Menu entity
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(50);
                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Icon)
                    .HasMaxLength(50);
                entity.Property(e => e.Area)
                    .HasMaxLength(50);
                entity.Property(e => e.Controller)
                    .HasMaxLength(50);
                entity.Property(e => e.Action)
                    .HasMaxLength(50);
                entity.Property(e => e.Url)
                    .HasMaxLength(200);
                entity.Property(e => e.MenuType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Customer");
                entity.Property(e => e.ParentId)
                    .HasMaxLength(50);

                // Self-referential relationship
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.Order);
                entity.HasIndex(e => e.IsVisible);
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => e.MenuType);
            });

            // Configure Footer entity
            modelBuilder.Entity<Footer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(50);
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Icon)
                    .HasMaxLength(50);
                entity.Property(e => e.Content)
                    .HasMaxLength(500);
                entity.Property(e => e.Url)
                    .HasMaxLength(200);
                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Links");
                entity.Property(e => e.FooterType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Customer");
                entity.Property(e => e.ParentId)
                    .HasMaxLength(50);
                entity.Property(e => e.Area)
                    .HasMaxLength(50);
                entity.Property(e => e.Controller)
                    .HasMaxLength(50);
                entity.Property(e => e.Action)
                    .HasMaxLength(50);
                entity.Property(e => e.Target)
                    .HasMaxLength(50);
                entity.Property(e => e.CssClass)
                    .HasMaxLength(100);

                // Self-referential relationship
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.Order);
                entity.HasIndex(e => e.IsVisible);
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => e.FooterType);
                entity.HasIndex(e => e.Section);
                entity.HasIndex(e => new { e.FooterType, e.Section, e.Order });
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.ShortName)
                    .HasMaxLength(50);
                entity.Property(e => e.Title)
                    .HasMaxLength(200);
                entity.Property(e => e.Icon)
                    .HasMaxLength(50);
                entity.Property(e => e.ColorClass)
                    .HasMaxLength(50);
                entity.Property(e => e.Level)
                    .HasMaxLength(50);
                entity.Property(e => e.Duration)
                    .HasMaxLength(50);
                entity.Property(e => e.Instructor)
                    .HasMaxLength(100);
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                // Indexes for better query performance
                entity.HasIndex(e => e.Code);
                entity.HasIndex(e => e.SubjectId);
                entity.HasIndex(e => e.IsFree);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => new { e.SubjectId, e.IsFree });
            });

            // Configure ExamSubject entity
            modelBuilder.Entity<ExamSubject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.ShortName)
                    .HasMaxLength(50);
                entity.Property(e => e.Title)
                    .HasMaxLength(200);
                entity.Property(e => e.Icon)
                    .HasMaxLength(50);
                entity.Property(e => e.ColorClass)
                    .HasMaxLength(20);
                entity.Property(e => e.Duration)
                    .HasMaxLength(50);
                entity.Property(e => e.BadgeText)
                    .HasMaxLength(50);
                entity.Property(e => e.BadgeIcon)
                    .HasMaxLength(50);
                entity.Property(e => e.BadgeColorClass)
                    .HasMaxLength(50);

                // Indexes for better query performance
                entity.HasIndex(e => e.Code);
                entity.HasIndex(e => e.ColorClass);
            });

            // Configure ExamList entity
            modelBuilder.Entity<ExamList>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubjectId)
                    .IsRequired();
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Description);
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Mode)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.TotalProjects);
                entity.Property(e => e.TotalTasks);
                entity.Property(e => e.TimeLimit);
                entity.Property(e => e.PassingScore);
                entity.Property(e => e.Difficulty)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);

                // Foreign key relationship
                entity.HasOne(e => e.Subject)
                    .WithMany()
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes for better query performance
                entity.HasIndex(e => e.SubjectId);
                entity.HasIndex(e => e.Code);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Mode);
                entity.HasIndex(e => e.Difficulty);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.SubjectId, e.IsActive });
                entity.HasIndex(e => new { e.Type, e.Mode, e.IsActive });
            });

            // Configure ExamProject entity
            modelBuilder.Entity<ExamProject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExamListId)
                    .IsRequired();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Description);
                entity.Property(e => e.TotalTasks)
                    .IsRequired();
                entity.Property(e => e.OrderIndex)
                    .IsRequired();
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);

                // Foreign key relationship
                entity.HasOne(e => e.ExamList)
                    .WithMany(e => e.ExamProjects)
                    .HasForeignKey(e => e.ExamListId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes for better query performance
                entity.HasIndex(e => e.ExamListId);
                entity.HasIndex(e => e.OrderIndex);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.ExamListId, e.OrderIndex });
                entity.HasIndex(e => new { e.ExamListId, e.IsActive, e.OrderIndex });
            });

            // Configure ExamTask entity - Temporarily commented out to fix migration
            // modelBuilder.Entity<ExamTask>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.ExamProjectId)
            //         .IsRequired();
            //     entity.Property(e => e.Name)
            //         .IsRequired()
            //         .HasMaxLength(200);
            //     entity.Property(e => e.Description);
            //     entity.Property(e => e.Instructions);
            //     entity.Property(e => e.OrderIndex)
            //         .IsRequired();
            //     entity.Property(e => e.MaxScore)
            //         .IsRequired();
            //     entity.Property(e => e.TaskType)
            //         .IsRequired()
            //         .HasMaxLength(50);
            //     entity.Property(e => e.IsActive)
            //         .HasDefaultValue(true);
            //     entity.Property(e => e.CreatedAt)
            //         .IsRequired();
            //     entity.Property(e => e.UpdatedAt);
            // 
            //     // Foreign key relationship
            //     entity.HasOne(e => e.ExamProject)
            //         .WithMany()
            //         .HasForeignKey(e => e.ExamProjectId)
            //         .OnDelete(DeleteBehavior.Cascade);
            // 
            //     // Indexes for better query performance
            //     entity.HasIndex(e => e.ExamProjectId);
            //     entity.HasIndex(e => e.OrderIndex);
            //     entity.HasIndex(e => e.IsActive);
            //     entity.HasIndex(e => e.CreatedAt);
            //     entity.HasIndex(e => new { e.ExamProjectId, e.OrderIndex });
            // });
        }
    }
}
