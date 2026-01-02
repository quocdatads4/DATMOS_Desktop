using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class LessonsController : Controller
    {
        // GET: Admin/Lessons
        public IActionResult Index()
        {
            // Tạo dữ liệu mẫu bài học với 40 bài học để phân trang
            var sampleLessons = GenerateSampleLessons(40);

            return View(sampleLessons);
        }

        private List<dynamic> GenerateSampleLessons(int count)
        {
            var lessons = new List<dynamic>();
            var random = new Random();
            var courseOptions = new[]
            {
                new { Id = "CRS001", Code = "MOS-WORD-2019", Name = "MOS Word 2019", Instructor = "Nguyễn Thị Mai" },
                new { Id = "CRS002", Code = "MOS-EXCEL-2019", Name = "MOS Excel 2019", Instructor = "Trần Văn Hùng" },
                new { Id = "CRS003", Code = "MOS-PPT-2019", Name = "MOS PowerPoint 2019", Instructor = "Lê Thị Hương" },
                new { Id = "CRS004", Code = "MOS-WORD-EXPERT", Name = "MOS Word 2019 Expert", Instructor = "Phạm Văn Đức" },
                new { Id = "CRS005", Code = "MOS-EXCEL-EXPERT", Name = "MOS Excel 2019 Expert", Instructor = "Hoàng Thị Lan" }
            };

            var typeOptions = new[] { "Video", "Bài đọc", "Quiz", "Bài tập", "Thực hành" };
            var statusOptions = new[] { "Active", "Hidden", "Pending" };
            
            var baseTitles = new[]
            {
                "Giới thiệu", "Cơ bản", "Nâng cao", "Thực hành", "Tổng kết",
                "Lý thuyết", "Bài tập", "Case study", "Demo", "Review",
                "Hướng dẫn", "Thực hành nâng cao", "Chuyên đề", "Workshop", "Tutorial"
            };

            var courseSubjects = new[]
            {
                "Microsoft Word", "Microsoft Excel", "Microsoft PowerPoint", 
                "Microsoft Access", "Microsoft Outlook", "Office 365",
                "OneDrive", "Teams", "SharePoint", "Power BI"
            };

            for (int i = 1; i <= count; i++)
            {
                var course = courseOptions[random.Next(courseOptions.Length)];
                var type = typeOptions[random.Next(typeOptions.Length)];
                var status = statusOptions[random.Next(statusOptions.Length)];
                var isActive = status == "Active";
                var isFree = random.Next(0, 100) < 30; // 30% free lessons
                
                var subject = courseSubjects[random.Next(courseSubjects.Length)];
                var baseTitle = baseTitles[random.Next(baseTitles.Length)];
                var title = $"{baseTitle} {subject} {random.Next(2019, 2024)}";
                
                // Ensure unique IDs
                var coursePrefix = course.Code.Replace("MOS-", "").Replace("-2019", "").Replace("-EXPERT", "");
                var id = $"LES{coursePrefix}{i:D3}";
                var code = $"LES-{coursePrefix}-{i:D3}";
                
                lessons.Add(new
                {
                    Id = id,
                    Code = code,
                    Title = title,
                    Description = $"Bài học về {subject} - {baseTitle.ToLower()}. Bài học này cung cấp kiến thức và kỹ năng cần thiết.",
                    CourseId = course.Id,
                    CourseCode = course.Code,
                    CourseName = course.Name,
                    Duration = random.Next(15, 120), // 15-120 minutes
                    Type = type,
                    Order = random.Next(1, 20),
                    Status = status,
                    IsActive = isActive,
                    IsFree = isFree,
                    ThumbnailUrl = "",
                    VideoUrl = "",
                    CreatedDate = new System.DateTime(2024, random.Next(1, 12), random.Next(1, 28), 
                                                     random.Next(8, 17), random.Next(0, 60), 0),
                    UpdatedDate = new System.DateTime(2024, random.Next(1, 12), random.Next(1, 28),
                                                     random.Next(8, 17), random.Next(0, 60), 0),
                    Instructor = course.Instructor,
                    ViewCount = random.Next(100, 5000),
                    CompletionRate = Math.Round(random.NextDouble() * 30 + 50, 1) // 50-80%
                });
            }

            // Sort by created date descending for more realistic data
            return lessons.OrderByDescending(l => ((dynamic)l).CreatedDate).ToList();
        }

        // GET: Admin/Lessons/Create
        public IActionResult Create()
        {
            return PartialView("../Views/Lessons/_CreateEditModal.cshtml", new { });
        }

        // GET: Admin/Lessons/Edit/5
        public IActionResult Edit(string id)
        {
            // Placeholder for demo
            return PartialView("../Views/Lessons/_CreateEditModal.cshtml", new { Id = id });
        }

        // GET: Admin/Lessons/Preview/5
        public IActionResult Preview(string id)
        {
            // Placeholder for demo
            return Json(new { success = true, message = $"Preview lesson {id}" });
        }

        // POST: Admin/Lessons/Save
        [HttpPost]
        public IActionResult Save(dynamic model)
        {
            // Placeholder for demo
            return Json(new { success = true, data = model?.Id ?? "LES001" });
        }

        // POST: Admin/Lessons/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            // Placeholder for demo
            return Json(new { success = true });
        }

        // POST: Admin/Lessons/Toggle/5
        [HttpPost]
        public IActionResult Toggle(string id)
        {
            // Placeholder for demo
            return Json(new { success = true });
        }

        // GET: Admin/Lessons/GetByCourse/{courseId}
        public IActionResult GetByCourse(string courseId)
        {
            // Placeholder for demo - return empty list
            return Json(new { success = true, data = new List<dynamic>() });
        }

        // POST: Admin/Lessons/BulkAction
        [HttpPost]
        public IActionResult BulkAction(string action, List<string> ids)
        {
            // Placeholder for demo
            return Json(new { 
                success = true, 
                message = $"Performed {action} on {ids?.Count ?? 0} lessons" 
            });
        }

        // GET: Admin/Lessons/Export
        public IActionResult Export(string format = "csv", List<string> ids = null)
        {
            // Placeholder for demo
            return Json(new { 
                success = true, 
                message = $"Exported {ids?.Count ?? 0} lessons as {format}" 
            });
        }
    }
}
