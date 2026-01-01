using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DATMOS.Data
{
    public static class MenuSeeder
    {
public static void Seed(AppDbContext context)
{
    // Ensure the database schema is up‑to‑date before seeding
    Console.WriteLine("Applying pending migrations (if any)...");
    context.Database.Migrate();
    Console.WriteLine("Migrations applied.");

    // Clear existing data first
    Console.WriteLine("Clearing existing Menus data...");
    context.Menus.RemoveRange(context.Menus);
    context.SaveChanges();
    Console.WriteLine("Existing data cleared.");

    // Create seed data for all menu types
    var menus = CreateSeedData();
    
            // Wrap the insert in a retry block to handle transient PostgreSQL failures
            var strategy = context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    context.Menus.AddRange(menus);
                    context.SaveChanges();
                    transaction.Commit();
                    Console.WriteLine($"Saved {menus.Count} menu items to database.");
                    Console.WriteLine($"Menu types: {string.Join(", ", menus.Select(m => m.MenuType).Distinct())}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error saving menus to database: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    throw;
                }
            });

            Console.WriteLine($"Seed completed successfully! Total menus: {menus.Count}");
        }

        private static List<Menu> CreateSeedData()
        {
            var menus = new List<Menu>();

            // Admin Menu
            menus.Add(new Menu
            {
                Id = "admin-dashboard",
                Text = "Dashboard",
                Icon = "ti-smart-home",
                Area = "Admin",
                Controller = "Home",
                Action = "Index",
                MenuType = "Admin",
                Order = 1,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "admin-courses",
                Text = "Khóa học",
                Icon = "ti-book",
                Area = "Admin",
                Controller = "Courses",
                Action = "Index",
                MenuType = "Admin",
                Order = 2,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "admin-users",
                Text = "Người dùng",
                Icon = "ti-users",
                Area = "Admin",
                Controller = "Users",
                Action = "Index",
                MenuType = "Admin",
                Order = 3,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "admin-teachers",
                Text = "Giáo viên",
                Icon = "ti-user",
                Area = "Admin",
                Controller = "TeacherList",
                Action = "Index",
                MenuType = "Admin",
                Order = 4,
                IsVisible = true
            });

            // Customer Menu
            menus.Add(new Menu
            {
                Id = "customer-dashboard",
                Text = "Trang chủ",
                Icon = "ti-smart-home",
                Area = "Customer",
                Controller = "Home",
                Action = "Index",
                MenuType = "Customer",
                Order = 1,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "customer-courses",
                Text = "Khóa học",
                Icon = "ti-book",
                Area = "Customer",
                Controller = "Courses",
                Action = "Index",
                MenuType = "Customer",
                Order = 2,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "customer-exam-practice",
                Text = "Luyện thi",
                Icon = "ti-pencil",
                Area = "Customer",
                Controller = "ExamSubject",
                Action = "Index",
                MenuType = "Customer",
                Order = 3,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "customer-results",
                Text = "Kết quả",
                Icon = "ti-chart-bar",
                Area = "Customer",
                Controller = "Results",
                Action = "Index",
                MenuType = "Customer",
                Order = 4,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "customer-pricing",
                Text = "Bảng giá",
                Icon = "ti-tag",
                Area = "Customer",
                Url = "#pricing",
                MenuType = "Customer",
                Order = 5,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "customer-payment",
                Text = "Thanh toán",
                Icon = "ti-credit-card",
                Area = "Customer",
                Url = "#payment",
                MenuType = "Customer",
                Order = 6,
                IsVisible = true
            });

            // Teacher Menu
            menus.Add(new Menu
            {
                Id = "teacher-dashboard",
                Text = "Dashboard",
                Icon = "ti-smart-home",
                Area = "Teacher",
                Controller = "Home",
                Action = "Index",
                MenuType = "Teacher",
                Order = 1,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "teacher-classes",
                Text = "Lớp học",
                Icon = "ti-school",
                Area = "Teacher",
                Controller = "Classes",
                Action = "Index",
                MenuType = "Teacher",
                Order = 2,
                IsVisible = true
            });

            // Landing Menu
            menus.Add(new Menu
            {
                Id = "landing-home",
                Text = "Trang chủ",
                Icon = "",
                Area = "",
                Controller = "Home",
                Action = "Index",
                MenuType = "Landing",
                Order = 1,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "landing-features",
                Text = "Tính năng",
                Icon = "",
                Url = "#features",
                MenuType = "Landing",
                Order = 2,
                IsVisible = true
            });

            menus.Add(new Menu
            {
                Id = "landing-pricing",
                Text = "Giá cả",
                Icon = "",
                Url = "#pricing",
                MenuType = "Landing",
                Order = 3,
                IsVisible = true
            });

            return menus;
        }
    }
}
