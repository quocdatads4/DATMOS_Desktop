using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DATMOS.Web.Areas.Admin.ViewModels;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public IActionResult Index()
        {
            var users = DATMOS.Web.Areas.Admin.Models.InMemoryUserStore.GetAll();
            return View(users);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return PartialView("../Views/Users/_CreateEditModal.cshtml", new UserViewModel());
        }

        // GET: Admin/Users/Edit/5
        public IActionResult Edit(int id)
        {
            var user = DATMOS.Web.Areas.Admin.Models.InMemoryUserStore.GetAll().FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return PartialView("../Views/Users/_CreateEditModal.cshtml", user);
        }

        // POST: Admin/Users/Save
        [HttpPost]
        public IActionResult Save(UserViewModel model)
        {
            if (model == null) return BadRequest();
            if (model.Id > 0)
            {
                DATMOS.Web.Areas.Admin.Models.InMemoryUserStore.Update(model);
            }
            else
            {
                DATMOS.Web.Areas.Admin.Models.InMemoryUserStore.Add(model);
            }
            return Json(new { success = true, data = model.Id });
        }

        // POST: Admin/Users/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            DATMOS.Web.Areas.Admin.Models.InMemoryUserStore.Delete(id);
            return Json(new { success = true });
        }

        // POST: Admin/Users/ResetPassword/5
        [HttpPost]
        public IActionResult ResetPassword(int id)
        {
            // placeholder for demo
            return Json(new { success = true });
        }
    }
}
