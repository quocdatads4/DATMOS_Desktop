using DATMOS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DATMOS.Data
{
    public static class ExamListSeeder
    {
        public static void Seed(AppDbContext context)
        {
            try
            {
                // Kiểm tra xem đã có dữ liệu ExamList chưa
                if (context.ExamLists.Any())
                {
                    Console.WriteLine("Bảng ExamLists đã có dữ liệu, bỏ qua seeding.");
                    return;
                }

                // Kiểm tra xem có dữ liệu ExamSubject không
                if (!context.ExamSubjects.Any())
                {
                    Console.WriteLine("Không tìm thấy dữ liệu ExamSubject. Vui lòng chạy ExamSubjectSeeder trước.");
                    return;
                }

                var examLists = new[]
                {
                    new ExamList
                    {
                        SubjectId = 1, // Word
                        Code = "WORD_PE_01",
                        Name = "Word 2019 Practice Exam 1",
                        Description = "Bài thi thử tổng hợp số 1. Bao gồm 7 Project với các kỹ năng: Quản lý tài liệu, Chèn đối tượng, Quản lý tham chiếu...",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Medium",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 1, // Word
                        Code = "WORD_PE_02",
                        Name = "Word 2019 Practice Exam 2",
                        Description = "Bài thi thử tổng hợp số 2. Tập trung vào các kỹ năng nâng cao và xử lý văn bản phức tạp.",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Hard",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 1, // Word
                        Code = "WORD_SR_01",
                        Name = "Word 2019 Skill Review 1",
                        Description = "Bài ôn tập kỹ năng 1. Không giới hạn thời gian, có hướng dẫn chi tiết từng bước.",
                        Type = "SkillReview",
                        Mode = "Training",
                        TotalProjects = 5,
                        TotalTasks = 25,
                        TimeLimit = 0,
                        PassingScore = 0,
                        Difficulty = "Easy",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 2, // Excel
                        Code = "EXCEL_PE_01",
                        Name = "Excel 2019 Practice Exam 1",
                        Description = "Bài thi thử tổng hợp số 1. Bao gồm 7 Project với các kỹ năng: Quản lý trang tính, Công thức & Hàm, Biểu đồ...",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Medium",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 2, // Excel
                        Code = "EXCEL_PE_02",
                        Name = "Excel 2019 Practice Exam 2",
                        Description = "Bài thi thử tổng hợp số 2. Tập trung vào PivotTable, Hàm điều kiện và Quản lý dữ liệu.",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Hard",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 2, // Excel
                        Code = "EXCEL_SR_01",
                        Name = "Excel 2019 Skill Review 1",
                        Description = "Bài ôn tập kỹ năng 1. Rèn luyện thao tác cơ bản và làm quen với giao diện Excel.",
                        Type = "SkillReview",
                        Mode = "Training",
                        TotalProjects = 5,
                        TotalTasks = 25,
                        TimeLimit = 0,
                        PassingScore = 0,
                        Difficulty = "Easy",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 3, // PowerPoint
                        Code = "PPT_PE_01",
                        Name = "PowerPoint 2019 Practice Exam 1",
                        Description = "Bài thi thử tổng hợp số 1. Bao gồm 7 Project với các kỹ năng: Quản lý Slide, Chèn Media, Hiệu ứng chuyển cảnh...",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Medium",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 3, // PowerPoint
                        Code = "PPT_PE_02",
                        Name = "PowerPoint 2019 Practice Exam 2",
                        Description = "Bài thi thử tổng hợp số 2. Tập trung vào Slide Master, Animation nâng cao và Xuất bản bài thuyết trình.",
                        Type = "Practice",
                        Mode = "Testing",
                        TotalProjects = 7,
                        TotalTasks = 35,
                        TimeLimit = 50,
                        PassingScore = 700,
                        Difficulty = "Hard",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ExamList
                    {
                        SubjectId = 3, // PowerPoint
                        Code = "PPT_SR_01",
                        Name = "PowerPoint 2019 Skill Review 1",
                        Description = "Bài ôn tập kỹ năng 1. Rèn luyện thao tác thiết kế Slide và hiệu ứng.",
                        Type = "SkillReview",
                        Mode = "Training",
                        TotalProjects = 5,
                        TotalTasks = 25,
                        TimeLimit = 0,
                        PassingScore = 0,
                        Difficulty = "Easy",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                int successCount = 0;
                foreach (var examList in examLists)
                {
                    try
                    {
                        context.ExamLists.Add(examList);
                        context.SaveChanges();
                        successCount++;
                        Console.WriteLine($"Đã thêm bài thi: {examList.Code} - {examList.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi thêm bài thi {examList.Code}: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner: {ex.InnerException.Message}");
                        }
                        // Rollback changes for this exam list
                        context.ChangeTracker.Entries().Where(e => e.State != EntityState.Detached).ToList()
                            .ForEach(e => e.State = EntityState.Detached);
                    }
                }

                Console.WriteLine($"Đã thêm {successCount}/{examLists.Length} bài thi vào database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi seed dữ liệu ExamList: {ex.Message}");
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
