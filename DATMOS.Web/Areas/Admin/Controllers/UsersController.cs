using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATMOS.Core.Entities.Identity;
using DATMOS.Web.Areas.Admin.ViewModels;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<AddUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AddUsers> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userViewModel = AdminUserViewModel.FromUser(user, roles);
                userViewModels.Add(userViewModel);
            }
            
            return View(userViewModels);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return PartialView("../Views/Users/_CreateEditModal.cshtml", new UserViewModel());
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            
            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = AdminUserViewModel.FromUser(user, roles);
            
            // Convert to UserViewModel for compatibility with existing modal
            var viewModel = new UserViewModel
            {
                Id = int.TryParse(user.Id, out int intId) ? intId : 0,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "User",
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now ? "Locked" : "Active"
            };
            
            return PartialView("../Views/Users/_CreateEditModal.cshtml", viewModel);
        }

        // POST: Admin/Users/Save
        [HttpPost]
        public async Task<IActionResult> Save(UserViewModel model)
        {
            if (model == null) return BadRequest();
            
            if (model.Id > 0) // Update existing user
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null) return NotFound();
                
                user.UserName = model.UserName;
                user.Email = model.Email;
                
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
                }
                
                // Update role if changed
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            else // Create new user
            {
                var user = new AddUsers
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
                }
                
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            
            return Json(new { success = true });
        }

        // POST: Admin/Users/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
            }
            
            return Json(new { success = true });
        }

        // POST: Admin/Users/ResetPassword/5
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = "Temp@123"; // Generate temporary password
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            
            if (!result.Succeeded)
            {
                return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
            }
            
            return Json(new { success = true, newPassword = newPassword });
        }
        
        // POST: Admin/Users/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now)
            {
                // Unlock user
                user.LockoutEnd = null;
            }
            else
            {
                // Lock user for 365 days
                user.LockoutEnd = DateTimeOffset.Now.AddDays(365);
            }
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
            }
            
            return Json(new { success = true, isActive = !(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now) });
        }
    }
}
