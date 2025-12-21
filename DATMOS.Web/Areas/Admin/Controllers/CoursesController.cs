using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesController : Controller
    {
        // GET: Admin/Courses
        public IActionResult Index()
        {
            // Tạo dữ liệu mẫu khóa học
            var sampleCourses = new List<dynamic>
            {
                new {
                    Id = "CRS001",
                    Code = "MOS-WORD-2019",
                    Name = "MOS Word 2019",
                    Description = "Khóa học chứng chỉ Microsoft Office Specialist Word 2019",
                    Level = "Cơ bản - Nâng cao",
                    Duration = "30 giờ",
                    Price = 1200000,
                    OriginalPrice = 1500000,
                    Status = "Active",
                    IsActive = true,
                    ImageUrl = "",
                    StudentCount = 45,
                    Rating = 4.8,
                    CreatedDate = new System.DateTime(2024, 1, 10, 9, 0, 0),
                    Category = "Microsoft Office",
                    Instructor = "Nguyễn Thị Mai"
                },
                new {
                    Id = "CRS002",
                    Code = "MOS-EXCEL-2019",
                    Name = "MOS Excel 2019",
                    Description = "Khóa học chứng chỉ Microsoft Office Specialist Excel 2019",
                    Level = "Cơ bản - Nâng cao",
                    Duration = "35 giờ",
                    Price = 1400000,
                    OriginalPrice = 1800000,
                    Status = "Active",
                    IsActive = true,
                    ImageUrl = "",
                    StudentCount = 52,
                    Rating = 4.9,
                    CreatedDate = new System.DateTime(2024, 2, 15, 10, 30, 0),
                    Category = "Microsoft Office",
                    Instructor = "Trần Văn Hùng"
                },
                new {
                    Id = "CRS003",
                    Code = "MOS-PPT-2019",
                    Name = "MOS PowerPoint 2019",
                    Description = "Khóa học chứng chỉ Microsoft Office Specialist PowerPoint 2019",
                    Level = "Cơ bản - Trung cấp",
                    Duration = "25 giờ",
                    Price = 1000000,
                    OriginalPrice = 1300000,
                    Status = "Active",
                    IsActive = true,
                    ImageUrl = "",
                    StudentCount = 38,
                    Rating = 4.7,
                    CreatedDate = new System.DateTime(2024, 3, 5, 14, 15, 0),
                    Category = "Microsoft Office",
                    Instructor = "Lê Thị Hương"
                },
                new {
                    Id = "CRS004",
                    Code = "MOS-WORD-EXPERT",
                    Name = "MOS Word 2019 Expert",
                    Description = "Khóa học chứng chỉ Microsoft Office Specialist Word 2019 Expert",
                    Level = "Chuyên gia",
                    Duration = "40 giờ",
                    Price = 1800000,
                    OriginalPrice = 2200000,
                    Status = "Inactive",
                    IsActive = false,
                    ImageUrl = "",
                    StudentCount = 22,
                    Rating = 4.9,
                    CreatedDate = new System.DateTime(2024, 4, 20, 11, 0, 0),
                    Category = "Microsoft Office",
                    Instructor = "Phạm Văn Đức"
                },
                new {
                    Id = "CRS005",
                    Code = "MOS-EXCEL-EXPERT",
                    Name = "MOS Excel 2019 Expert",
                    Description = "Khóa học chứng chỉ Microsoft Office Specialist Excel 2019 Expert",
                    Level = "Chuyên gia",
                    Duration = "45 giờ",
                    Price = 2000000,
                    OriginalPrice = 2500000,
                    Status = "Pending",
                    IsActive = false,
                    ImageUrl = "",
                    StudentCount = 15,
                    Rating = 4.6,
                    CreatedDate = new System.DateTime(2024, 5, 12, 16, 45, 0),
                    Category = "Microsoft Office",
                    Instructor = "Hoàng Thị Lan"
                }
            };

            return View(sampleCourses);
        }

        // GET: Admin/Courses/Create
        public IActionResult Create()
        {
            return PartialView("../Views/Courses/_CreateEditModal.cshtml", new { });
        }

        // GET: Admin/Courses/Edit/5
        public IActionResult Edit(string id)
        {
            // Placeholder for demo
            return PartialView("../Views/Courses/_CreateEditModal.cshtml", new { Id = id });
        }

        // POST: Admin/Courses/Save
        [HttpPost]
        public IActionResult Save(dynamic model)
        {
            // Placeholder for demo
            return Json(new { success = true, data = model?.Id ?? "CRS001" });
        }

        // POST: Admin/Courses/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            // Placeholder for demo
            return Json(new { success = true });
        }

        // POST: Admin/Courses/Toggle/5
        [HttpPost]
        public IActionResult Toggle(string id)
        {
            // Placeholder for demo
            return Json(new { success = true });
        }
    }
}
