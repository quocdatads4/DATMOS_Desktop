using System;
using System.Threading.Tasks;
using DATMOS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DATMOS.Data
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting DATMOS Data Seeders...");

            // Create service collection
            var services = new ServiceCollection();
            
            // Configure DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql("Host=31.220.84.102;Port=5432;Database=datprofilecms;Username=datprofilecms_user;Password=PgI_hOcs8NkK_Kp9jzLT_kz0;Trust Server Certificate=true"));

            // Add Identity services
            services.AddIdentityCore<AddUsers>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Add simple logging
            services.AddLogging();

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    // Ensure database is created and migrations are applied
                    Console.WriteLine("Applying database migrations...");
                    context.Database.Migrate();
                    
                    // Run seeders
                    Console.WriteLine("Running MenuSeeder...");
                    MenuSeeder.Seed(context);
                    
                    Console.WriteLine("Running FooterSeeder...");
                    FooterSeeder.Seed(context);
                    
                    Console.WriteLine("Running CourseSeeder...");
                    CourseSeeder.Seed(context);
                    
                    Console.WriteLine("Running ExamSubjectSeeder...");
                    ExamSubjectSeeder.Seed(context);
                    
                    Console.WriteLine("Running ExamListSeeder...");
                    ExamListSeeder.Seed(context);
                    
                    Console.WriteLine("Running ExamProjectSeeder...");
                    await ExamProjectSeeder.SeedAsync(context);
                    
                    Console.WriteLine("Running ExamTaskSeeder...");
                    await ExamTaskSeeder.SeedAsync(context);

                    Console.WriteLine("Running IdentitySeeder...");
                    await IdentitySeeder.SeedAsync(serviceProvider);
                    
                    Console.WriteLine("All seeders completed successfully!");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during seeding");
                    Console.WriteLine($"Error during seeding: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }

            Console.WriteLine("Seeder execution completed.");
        }
    }
}
