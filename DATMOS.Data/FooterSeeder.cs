using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DATMOS.Data
{
    public static class FooterSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Check if there are already footer items
            if (context.Footers.Any())
            {
                Console.WriteLine("Footer data already exists. Skipping seed.");
                return;
            }

            // Create seed data for all footer types
            var footers = CreateSeedData();
            
            try
            {
                context.Footers.AddRange(footers);
                context.SaveChanges();
                Console.WriteLine($"Saved {footers.Count} footer items to database.");
                Console.WriteLine($"Footer types: {string.Join(", ", footers.Select(f => f.FooterType).Distinct())}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving footers to database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }

            Console.WriteLine($"Footer seed completed successfully! Total footers: {footers.Count}");
        }

        private static List<Footer> CreateSeedData()
        {
            var footers = new List<Footer>();

            // Customer Footer Seed Data
            // Brand Section
            footers.Add(new Footer
            {
                Id = "customer-brand",
                Title = "DATMOS Learning",
                Content = "Hệ thống học tập và thi thử MOS 2019 chuyên nghiệp, giúp bạn nắm vững kỹ năng tin học văn phòng và đạt chứng chỉ MOS quốc tế.",
                Section = "Brand",
                FooterType = "Customer",
                Order = 1,
                IsVisible = true,
                CssClass = "footer-brand"
            });

            // Social Links (Children of Brand)
            footers.Add(new Footer
            {
                Id = "customer-facebook",
                Title = "Facebook",
                Icon = "ti ti-brand-facebook",
                Url = "https://facebook.com/datmoslearning",
                Section = "Brand",
                FooterType = "Customer",
                ParentId = "customer-brand",
                Order = 1,
                IsVisible = true,
                Target = "_blank",
                CssClass = "social-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-youtube",
                Title = "YouTube",
                Icon = "ti ti-brand-youtube",
                Url = "https://youtube.com/datmoslearning",
                Section = "Brand",
                FooterType = "Customer",
                ParentId = "customer-brand",
                Order = 2,
                IsVisible = true,
                Target = "_blank",
                CssClass = "social-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-tiktok",
                Title = "TikTok",
                Icon = "ti ti-brand-tiktok",
                Url = "https://tiktok.com/@datmoslearning",
                Section = "Brand",
                FooterType = "Customer",
                ParentId = "customer-brand",
                Order = 3,
                IsVisible = true,
                Target = "_blank",
                CssClass = "social-link"
            });

            // Quick Links Section
            footers.Add(new Footer
            {
                Id = "customer-courses-all",
                Title = "Tất cả khóa học",
                Area = "Customer",
                Controller = "Course",
                Action = "Index",
                Section = "Links",
                FooterType = "Customer",
                Order = 1,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-word",
                Title = "Word 2019",
                Url = "#",
                Section = "Links",
                FooterType = "Customer",
                Order = 2,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-excel",
                Title = "Excel 2019",
                Url = "#",
                Section = "Links",
                FooterType = "Customer",
                Order = 3,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-powerpoint",
                Title = "PowerPoint 2019",
                Url = "#",
                Section = "Links",
                FooterType = "Customer",
                Order = 4,
                IsVisible = true,
                CssClass = "footer-link"
            });

            // Support Section
            footers.Add(new Footer
            {
                Id = "customer-home",
                Title = "Trang chủ",
                Area = "Customer",
                Controller = "Home",
                Action = "Index",
                Section = "Support",
                FooterType = "Customer",
                Order = 1,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-progress",
                Title = "Tiến độ học tập",
                Area = "Customer",
                Controller = "Home",
                Action = "Progress",
                Section = "Support",
                FooterType = "Customer",
                Order = 2,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-faq",
                Title = "Câu hỏi thường gặp",
                Url = "#faq",
                Section = "Support",
                FooterType = "Customer",
                Order = 3,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-help",
                Title = "Trợ giúp",
                Url = "#help",
                Section = "Support",
                FooterType = "Customer",
                Order = 4,
                IsVisible = true,
                CssClass = "footer-link"
            });

            // Contact Section
            footers.Add(new Footer
            {
                Id = "customer-address",
                Title = "123 Đường ABC, Quận XYZ, TP. Hồ Chí Minh",
                Icon = "ti ti-map-pin",
                Section = "Contact",
                FooterType = "Customer",
                Order = 1,
                IsVisible = true,
                CssClass = "contact-item"
            });

            footers.Add(new Footer
            {
                Id = "customer-email",
                Title = "info@datmos.edu.vn",
                Icon = "ti ti-mail",
                Url = "mailto:info@datmos.edu.vn",
                Section = "Contact",
                FooterType = "Customer",
                Order = 2,
                IsVisible = true,
                CssClass = "contact-item"
            });

            footers.Add(new Footer
            {
                Id = "customer-phone",
                Title = "(+84) 123 456 789",
                Icon = "ti ti-phone",
                Url = "tel:+84123456789",
                Section = "Contact",
                FooterType = "Customer",
                Order = 3,
                IsVisible = true,
                CssClass = "contact-item"
            });

            // Copyright Section
            footers.Add(new Footer
            {
                Id = "customer-copyright",
                Title = "© 2025 DATMOS Learning Platform. All rights reserved.",
                Section = "Copyright",
                FooterType = "Customer",
                Order = 1,
                IsVisible = true,
                CssClass = "copyright-text"
            });

            footers.Add(new Footer
            {
                Id = "customer-terms",
                Title = "Điều khoản sử dụng",
                Url = "#terms",
                Section = "Copyright",
                FooterType = "Customer",
                Order = 2,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "customer-privacy",
                Title = "Chính sách bảo mật",
                Url = "#privacy",
                Section = "Copyright",
                FooterType = "Customer",
                Order = 3,
                IsVisible = true,
                CssClass = "footer-link"
            });

            // Admin Footer Seed Data
            // Copyright Section for Admin
            footers.Add(new Footer
            {
                Id = "admin-copyright",
                Title = "© 2025 DATMOS Admin. All rights reserved.",
                Section = "Copyright",
                FooterType = "Admin",
                Order = 1,
                IsVisible = true,
                CssClass = "copyright-text"
            });

            footers.Add(new Footer
            {
                Id = "admin-license",
                Title = "Bản quyền",
                Url = "#license",
                Section = "Copyright",
                FooterType = "Admin",
                Order = 2,
                IsVisible = true,
                CssClass = "footer-link"
            });

            footers.Add(new Footer
            {
                Id = "admin-support",
                Title = "Hỗ trợ",
                Url = "#support",
                Section = "Copyright",
                FooterType = "Admin",
                Order = 3,
                IsVisible = true,
                CssClass = "footer-link"
            });

            // Teacher Footer (minimal)
            footers.Add(new Footer
            {
                Id = "teacher-copyright",
                Title = "© 2025 DATMOS Teacher Portal",
                Section = "Copyright",
                FooterType = "Teacher",
                Order = 1,
                IsVisible = true,
                CssClass = "copyright-text"
            });

            // Landing Page Footer
            footers.Add(new Footer
            {
                Id = "landing-copyright",
                Title = "© 2025 DATMOS - Hệ thống học tập MOS chuyên nghiệp",
                Section = "Copyright",
                FooterType = "Landing",
                Order = 1,
                IsVisible = true,
                CssClass = "copyright-text"
            });

            return footers;
        }
    }
}
