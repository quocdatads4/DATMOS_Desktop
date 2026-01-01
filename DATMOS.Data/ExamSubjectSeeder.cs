using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DATMOS.Data
{
    public static class ExamSubjectSeeder
    {
        public static void Seed(AppDbContext context)
        {
            try
            {
                if (!context.ExamSubjects.Any())
                {
                    var examSubjects = new List<ExamSubject>
                    {
                        new ExamSubject
                        {
                            Id = 1,
                            Code = "MOS-WD",
                            Name = "MOS Word",
                            ShortName = "Word",
                            Title = "Microsoft Office Specialist Word Certification",
                            Description = "Chứng chỉ Microsoft Office Specialist Word đánh giá kỹ năng sử dụng Microsoft Word ở cấp độ chuyên nghiệp.",
                            Icon = "ti-file-text",
                            ColorClass = "primary",
                            Duration = "50 phút",
                            TotalLessons = 18,
                            TotalExams = 5,
                            BadgeText = "Chuyên nghiệp",
                            BadgeIcon = "ti-medal",
                            BadgeColorClass = "primary"
                        },
                        new ExamSubject
                        {
                            Id = 2,
                            Code = "MOS-EX",
                            Name = "MOS Excel",
                            ShortName = "Excel",
                            Title = "Microsoft Office Specialist Excel Certification",
                            Description = "Chứng chỉ Microsoft Office Specialist Excel đánh giá kỹ năng sử dụng Microsoft Excel ở cấp độ chuyên nghiệp.",
                            Icon = "ti-table",
                            ColorClass = "success",
                            Duration = "50 phút",
                            TotalLessons = 22,
                            TotalExams = 5,
                            BadgeText = "Chuyên nghiệp",
                            BadgeIcon = "ti-medal",
                            BadgeColorClass = "success"
                        },
                        new ExamSubject
                        {
                            Id = 3,
                            Code = "MOS-PP",
                            Name = "MOS PowerPoint",
                            ShortName = "PowerPoint",
                            Title = "Microsoft Office Specialist PowerPoint Certification",
                            Description = "Chứng chỉ Microsoft Office Specialist PowerPoint đánh giá kỹ năng sử dụng Microsoft PowerPoint ở cấp độ chuyên nghiệp.",
                            Icon = "ti-presentation",
                            ColorClass = "info",
                            Duration = "50 phút",
                            TotalLessons = 15,
                            TotalExams = 5,
                            BadgeText = "Chuyên nghiệp",
                            BadgeIcon = "ti-medal",
                            BadgeColorClass = "info"
                        },
                        new ExamSubject
                        {
                            Id = 4,
                            Code = "MOS-OL",
                            Name = "MOS Outlook",
                            ShortName = "Outlook",
                            Title = "Microsoft Office Specialist Outlook Certification",
                            Description = "Chứng chỉ Microsoft Office Specialist Outlook đánh giá kỹ năng sử dụng Microsoft Outlook ở cấp độ chuyên nghiệp.",
                            Icon = "ti-email",
                            ColorClass = "warning",
                            Duration = "50 phút",
                            TotalLessons = 12,
                            TotalExams = 5,
                            BadgeText = "Chuyên nghiệp",
                            BadgeIcon = "ti-medal",
                            BadgeColorClass = "warning"
                        },
                        new ExamSubject
                        {
                            Id = 5,
                            Code = "MOS-AC",
                            Name = "MOS Access",
                            ShortName = "Access",
                            Title = "Microsoft Office Specialist Access Certification",
                            Description = "Chứng chỉ Microsoft Office Specialist Access đánh giá kỹ năng sử dụng Microsoft Access ở cấp độ chuyên gia.",
                            Icon = "ti-database",
                            ColorClass = "danger",
                            Duration = "50 phút",
                            TotalLessons = 20,
                            TotalExams = 5,
                            BadgeText = "Chuyên gia",
                            BadgeIcon = "ti-crown",
                            BadgeColorClass = "danger"
                        }
                    };

                    int successCount = 0;
                    foreach (var examSubject in examSubjects)
                    {
                        try
                        {
                            context.ExamSubjects.Add(examSubject);
                            context.SaveChanges();
                            successCount++;
                            Console.WriteLine($"Đã thêm môn thi: {examSubject.Code} - {examSubject.Name}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi thêm môn thi {examSubject.Code}: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"Inner: {ex.InnerException.Message}");
                            }
                            // Rollback changes for this exam subject
                            context.ChangeTracker.Entries().Where(e => e.State != EntityState.Detached).ToList()
                                .ForEach(e => e.State = EntityState.Detached);
                        }
                    }
                    
                    Console.WriteLine($"Đã thêm {successCount}/{examSubjects.Count} môn thi vào database.");
                }
                else
                {
                    Console.WriteLine("Bảng ExamSubjects đã có dữ liệu, bỏ qua seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi seed dữ liệu ExamSubject: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
