using DATMOS.Core.Entities;
using DATMOS.Core.Entities.Identity;
using DATMOS.Data;
using DATMOS.Web.Areas.Admin.ViewModels;
using DATMOS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _coursesService;
        private readonly AppDbContext _context;
        private readonly UserManager<AddUsers> _userManager;

        public CoursesController(ICoursesService coursesService, AppDbContext context, UserManager<AddUsers> userManager)
        {
            _coursesService = coursesService;
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _coursesService.GetAllCoursesAsync();
            var adminCourses = courses.Select(c => MapToAdminCourseViewModel(c)).ToList();
            return View(adminCourses);
        }

        // GET: Admin/Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _coursesService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var adminCourse = MapToAdminCourseViewModel(course);
            return View(adminCourse);
        }

        // GET: Admin/Courses/Create
        public async Task<IActionResult> Create()
        {
            await LoadTeachers();
            return View(new AdminCourseCreateEditViewModel());
        }

        // POST: Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCourseCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Code = model.Code,
                    Name = model.Name,
                    ShortName = model.ShortName,
                    Title = model.Title,
                    Description = model.Description,
                    Icon = model.Icon,
                    ColorClass = model.ColorClass,
                    Level = model.Level,
                    Duration = model.Duration,
                    TotalLessons = model.TotalLessons,
                    TotalProjects = model.TotalProjects,
                    TotalTasks = model.TotalTasks,
                    Price = model.Price,
                    IsFree = model.IsFree,
                    Instructor = model.Instructor,
                    Rating = model.Rating,
                    EnrolledStudents = model.EnrolledStudents,
                    SubjectId = model.SubjectId
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                // Clear cache
                _coursesService.GetType().GetMethod("ClearCache")?.Invoke(_coursesService, new object[] { "all_courses" });

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Admin/Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new AdminCourseCreateEditViewModel
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                ShortName = course.ShortName,
                Title = course.Title,
                Description = course.Description,
                Icon = course.Icon,
                ColorClass = course.ColorClass,
                Level = course.Level,
                Duration = course.Duration,
                TotalLessons = course.TotalLessons,
                TotalProjects = course.TotalProjects,
                TotalTasks = course.TotalTasks,
                Price = course.Price,
                IsFree = course.IsFree,
                Instructor = course.Instructor,
                Rating = course.Rating,
                EnrolledStudents = course.EnrolledStudents,
                SubjectId = course.SubjectId,
                OriginalPrice = course.Price * 1.25m, // Giả định giá gốc cao hơn 25%
                Category = "Microsoft Office" // Mặc định
            };

            await LoadTeachers();
            return View(model);
        }

        // POST: Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCourseCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _context.Courses.FindAsync(id);
                    if (course == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin khóa học
                    course.Code = model.Code;
                    course.Name = model.Name;
                    course.ShortName = model.ShortName;
                    course.Title = model.Title;
                    course.Description = model.Description;
                    course.Icon = model.Icon;
                    course.ColorClass = model.ColorClass;
                    course.Level = model.Level;
                    course.Duration = model.Duration;
                    course.TotalLessons = model.TotalLessons;
                    course.TotalProjects = model.TotalProjects;
                    course.TotalTasks = model.TotalTasks;
                    course.Price = model.Price;
                    course.IsFree = model.IsFree;
                    course.Instructor = model.Instructor;
                    course.Rating = model.Rating;
                    course.EnrolledStudents = model.EnrolledStudents;
                    course.SubjectId = model.SubjectId;

                    _context.Courses.Update(course);
                    await _context.SaveChangesAsync();

                    // Clear cache
                    _coursesService.GetType().GetMethod("ClearCache")?.Invoke(_coursesService, new object[] { "all_courses" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Admin/Courses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var adminCourse = MapToAdminCourseViewModel(await _coursesService.GetCourseByIdAsync(id));
            return View(adminCourse);
        }

        // POST: Admin/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                // Clear cache
                _coursesService.GetType().GetMethod("ClearCache")?.Invoke(_coursesService, new object[] { "all_courses" });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Courses/Toggle/5
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return Json(new { success = false, message = "Khóa học không tồn tại" });
            }

            // Ở đây có thể thêm logic toggle trạng thái nếu có field IsActive
            // Hiện tại Course entity không có IsActive, nên chỉ trả về success
            // Có thể thêm field IsActive vào Course entity nếu cần

            await _context.SaveChangesAsync();

            // Clear cache
            _coursesService.GetType().GetMethod("ClearCache")?.Invoke(_coursesService, new object[] { "all_courses" });

            return Json(new { success = true });
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        private AdminCourseViewModel MapToAdminCourseViewModel(DATMOS.Web.Areas.Customer.ViewModels.CourseViewModel course)
        {
            return new AdminCourseViewModel
            {
                Id = course.Id,
                Code = course.Code ?? string.Empty,
                Name = course.Name ?? string.Empty,
                Description = course.Description ?? string.Empty,
                Level = course.Level ?? string.Empty,
                Duration = course.Duration ?? string.Empty,
                Price = course.Price,
                OriginalPrice = course.IsFree ? 0 : course.Price * 1.25m, // Giả định giá gốc cao hơn 25%
                Status = course.IsFree ? "Free" : "Paid",
                IsActive = true, // Mặc định
                ImageUrl = "",
                StudentCount = course.EnrolledStudents,
                Rating = course.Rating,
                CreatedDate = DateTime.Now.AddDays(-course.Id * 10), // Giả định ngày tạo
                Category = "Microsoft Office", // Mặc định
                Instructor = course.Instructor ?? string.Empty,
                ShortName = course.ShortName ?? string.Empty,
                Title = course.Title ?? string.Empty,
                Icon = course.Icon ?? string.Empty,
                ColorClass = course.ColorClass ?? string.Empty,
                TotalLessons = course.TotalLessons,
                TotalProjects = course.TotalProjects,
                TotalTasks = course.TotalTasks,
                IsFree = course.IsFree,
                SubjectId = course.SubjectId
            };
        }

        private async Task LoadTeachers()
        {
            // Lấy danh sách tất cả người dùng có vai trò "Teacher"
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            
            // Nếu không có role Teacher, lấy tất cả người dùng
            if (teachers == null || teachers.Count == 0)
            {
                teachers = await _userManager.Users.ToListAsync();
            }

            // Tạo SelectList cho dropdown
            var teacherList = teachers.Select(t => new
            {
                Id = t.Id,
                Name = t.UserName ?? t.Email ?? t.Id
            }).ToList();

            ViewBag.Teachers = new SelectList(teacherList, "Id", "Name");
        }
    }
}
