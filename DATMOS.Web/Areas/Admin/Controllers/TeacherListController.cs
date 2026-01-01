using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DATMOS.Core.Entities.Identity;
using DATMOS.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class TeacherListController : Controller
    {
        private readonly UserManager<AddUsers> _userManager;

        public TeacherListController(UserManager<AddUsers> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/TeacherList
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người dùng có role "Teacher"
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            
            // Chuyển đổi sang ViewModel
            var teacherViewModels = new List<AdminUserViewModel>();
            
            foreach (var teacher in teachers)
            {
                var roles = await _userManager.GetRolesAsync(teacher);
                var teacherViewModel = AdminUserViewModel.FromUser(teacher, roles);
                teacherViewModels.Add(teacherViewModel);
            }

            // Tính toán thống kê
            ViewBag.TotalTeachers = teacherViewModels.Count;
            ViewBag.ActiveTeachers = teacherViewModels.Count(t => t.IsActive && t.Status == "Active");
            ViewBag.InactiveTeachers = teacherViewModels.Count(t => !t.IsActive || t.Status != "Active");
            
            return View(teacherViewModels);
        }
    }
}
