using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DATMOS.Data
{
    public static class CourseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            try
            {
                if (!context.Courses.Any())
                {
                    var courses = new List<Course>
                    {
                        new Course
                        {
                            Id = 1,
                            SubjectId = 1,
                            Code = "WD2019",
                            Name = "MOS Word 2019",
                            ShortName = "Word 2019",
                            Title = "Luyện thi chứng chỉ MOS Word 2019",
                            Description = "Khóa học luyện thi chứng chỉ Microsoft Office Specialist Word 2019, bao gồm toàn bộ kiến thức và kỹ năng cần thiết để đạt điểm cao trong kỳ thi.",
                            Icon = "ti-file-text",
                            ColorClass = "primary",
                            Level = "Chuyên nghiệp",
                            Duration = "6 tuần",
                            TotalLessons = 18,
                            TotalProjects = 8,
                            TotalTasks = 45,
                            Price = 0,
                            IsFree = true,
                            Instructor = "Chuyên gia MOS",
                            Rating = 4.8,
                            EnrolledStudents = 1500
                        },
                        new Course
                        {
                            Id = 2,
                            SubjectId = 2,
                            Code = "EX2019",
                            Name = "MOS Excel 2019",
                            ShortName = "Excel 2019",
                            Title = "Luyện thi chứng chỉ MOS Excel 2019",
                            Description = "Khóa học luyện thi chứng chỉ Microsoft Office Specialist Excel 2019, tập trung vào các hàm, công thức và phân tích dữ liệu chuyên sâu.",
                            Icon = "ti-table",
                            ColorClass = "success",
                            Level = "Chuyên nghiệp",
                            Duration = "8 tuần",
                            TotalLessons = 22,
                            TotalProjects = 10,
                            TotalTasks = 55,
                            Price = 0,
                            IsFree = true,
                            Instructor = "Chuyên gia MOS",
                            Rating = 4.9,
                            EnrolledStudents = 1800
                        },
                        new Course
                        {
                            Id = 3,
                            SubjectId = 3,
                            Code = "PP2019",
                            Name = "MOS PowerPoint 2019",
                            ShortName = "PowerPoint 2019",
                            Title = "Luyện thi chứng chỉ MOS PowerPoint 2019",
                            Description = "Khóa học luyện thi chứng chỉ Microsoft Office Specialist PowerPoint 2019, hướng dẫn thiết kế bài thuyết trình chuyên nghiệp và hiệu quả.",
                            Icon = "ti-presentation",
                            ColorClass = "info",
                            Level = "Chuyên nghiệp",
                            Duration = "5 tuần",
                            TotalLessons = 15,
                            TotalProjects = 6,
                            TotalTasks = 35,
                            Price = 0,
                            IsFree = true,
                            Instructor = "Chuyên gia MOS",
                            Rating = 4.7,
                            EnrolledStudents = 1200
                        }
                    };

                    // Thử từng course để xác định lỗi
                    int successCount = 0;
                    foreach (var course in courses)
                    {
                        try
                        {
                            context.Courses.Add(course);
                            context.SaveChanges();
                            successCount++;
                            Console.WriteLine($"Đã thêm khóa học: {course.Code} - {course.Name}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi thêm khóa học {course.Code}: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"Inner: {ex.InnerException.Message}");
                            }
                            // Rollback changes for this course
                            context.ChangeTracker.Entries().Where(e => e.State != EntityState.Detached).ToList()
                                .ForEach(e => e.State = EntityState.Detached);
                        }
                    }
                    
                    Console.WriteLine($"Đã thêm {successCount}/{courses.Count} khóa học MOS vào database.");
                }
                else
                {
                    Console.WriteLine("Bảng Courses đã có dữ liệu, bỏ qua seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi seed dữ liệu Course: {ex.Message}");
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
