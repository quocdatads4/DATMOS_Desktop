using DATMOS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DATMOS.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AddUsers>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

            logger.LogInformation("Starting Identity Seeder...");

            try
            {
                // Tạo roles
                await CreateRoleIfNotExistsAsync(roleManager, "Administrator", logger);
                await CreateRoleIfNotExistsAsync(roleManager, "User", logger);
                await CreateRoleIfNotExistsAsync(roleManager, "Teacher", logger);

                // Tạo users với DisplayName tiếng Việt
                await CreateUserIfNotExistsAsync(userManager, "admin@datmos.com", "Admin@123", "Administrator", 
                    GenerateVietnameseName("admin"), logger);
                await CreateUserIfNotExistsAsync(userManager, "user@datmos.com", "User@123", "User", 
                    GenerateVietnameseName("user"), logger);
                await CreateUserIfNotExistsAsync(userManager, "teacher@datmos.com", "Teacher@123", "Teacher", 
                    GenerateVietnameseName("teacher"), logger);

                logger.LogInformation("Identity Seeder completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding identity data");
                throw;
            }
        }

        private static async Task CreateRoleIfNotExistsAsync(RoleManager<IdentityRole> roleManager, string roleName, ILogger logger)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation($"Created role '{roleName}'");
                else
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors)}");
            }
            else
            {
                logger.LogInformation($"Role '{roleName}' already exists");
            }
        }

        private static async Task CreateUserIfNotExistsAsync(
            UserManager<AddUsers> userManager,
            string email,
            string password,
            string role,
            string displayName,
            ILogger logger)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AddUsers
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = displayName
                };

                // Phân tích tên để thiết lập FirstName/LastName
                ParseNameParts(user, displayName);

                var createResult = await userManager.CreateAsync(user, password);
                if (createResult.Succeeded)
                {
                    logger.LogInformation($"Created user '{email}'");
                    
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    if (roleResult.Succeeded)
                        logger.LogInformation($"Assigned role '{role}' to '{email}'");
                    else
                        logger.LogWarning($"Failed to assign role '{role}' to '{email}': {string.Join(", ", roleResult.Errors)}");
                }
                else
                {
                    logger.LogError($"Failed to create user '{email}': {string.Join(", ", createResult.Errors)}");
                }
            }
            else
            {
                logger.LogInformation($"User '{email}' already exists");
                
                // Đảm bảo user có role
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    if (roleResult.Succeeded)
                        logger.LogInformation($"Assigned role '{role}' to existing user '{email}'");
                }
            }
        }

        /// <summary>
        /// Tạo tên tiếng Việt ngẫu nhiên
        /// </summary>
        private static string GenerateVietnameseName(string userType)
        {
            var random = new Random();
            
            // Dữ liệu tên tiếng Việt
            string[] lastNames = { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Phan", "Vũ", "Đặng" };
            string[] middleNames = { "Văn", "Thị", "Minh", "Xuân", "Đức", "Quang", "Thanh" };
            string[] firstNames = { "An", "Bình", "Chi", "Dũng", "Hà", "Hải", "Hùng", "Lan", "Long", "Mai", "Nam", "Ngọc", "Phương", "Quân", "Thảo", "Trung" };

            string lastName = lastNames[random.Next(lastNames.Length)];
            string middleName = random.Next(2) == 0 ? "" : middleNames[random.Next(middleNames.Length)] + " ";
            string firstName = firstNames[random.Next(firstNames.Length)];

            string fullName = $"{lastName} {middleName}{firstName}".Trim();

            // Thêm chức danh nếu cần
            return userType.ToLower() switch
            {
                "admin" => $"{fullName} (Quản trị viên)",
                "teacher" => $"{fullName} (Giáo viên)",
                _ => fullName
            };
        }

        /// <summary>
        /// Phân tích DisplayName để thiết lập FirstName và LastName
        /// </summary>
        private static void ParseNameParts(AddUsers user, string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) return;

            // Loại bỏ phần chức danh trong ngoặc
            string cleanName = Regex.Replace(displayName, @"\s*\([^)]*\)", "").Trim();
            var parts = cleanName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return;
            
            if (parts.Length == 1)
            {
                user.FirstName = user.LastName = parts[0];
            }
            else
            {
                user.LastName = parts[0];
                user.FirstName = parts[^1]; // Phần tử cuối cùng
            }
        }

        /// <summary>
        /// Helper method để chạy seeder từ command line
        /// </summary>
        public static async Task RunSeederAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Starting Identity Seeder...");
            await SeedAsync(serviceProvider);
            Console.WriteLine("Identity Seeder completed!");
        }
    }
}
