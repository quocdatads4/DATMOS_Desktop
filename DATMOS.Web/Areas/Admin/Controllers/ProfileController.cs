using DATMOS.Web.Interfaces;
using DATMOS.Web.Areas.Identity.Controllers;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ProfileController : BaseProfileController
    {
        public ProfileController(
            IProfileService profileService,
            UserManager<DATMOS.Core.Entities.Identity.AddUsers> userManager,
            ILogger<BaseProfileController> logger)
            : base(profileService, userManager, logger)
        {
        }

        // GET: Admin/Profile
        public override async Task<IActionResult> Index()
        {
            var result = await base.Index();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsAdminArea = true;
                ViewBag.AreaName = "Admin";
                ViewBag.AreaDisplayName = "Quản trị viên";
            }
            return result;
        }

        // GET: Admin/Profile/Account
        public override async Task<IActionResult> Account()
        {
            var result = await base.Account();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsAdminArea = true;
                ViewBag.AreaName = "Admin";
                ViewBag.AreaDisplayName = "Quản trị viên";
            }
            return result;
        }

        // GET: Admin/Profile/Security
        public override async Task<IActionResult> Security()
        {
            var result = await base.Security();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsAdminArea = true;
                ViewBag.AreaName = "Admin";
                ViewBag.AreaDisplayName = "Quản trị viên";
            }
            return result;
        }

        // GET: Admin/Profile/Billing
        public override IActionResult Billing()
        {
            ViewBag.IsAdminArea = true;
            ViewBag.AreaName = "Admin";
            ViewBag.AreaDisplayName = "Quản trị viên";
            return base.Billing();
        }

        // GET: Admin/Profile/Notifications
        public override IActionResult Notifications()
        {
            ViewBag.IsAdminArea = true;
            ViewBag.AreaName = "Admin";
            ViewBag.AreaDisplayName = "Quản trị viên";
            return base.Notifications();
        }

        // GET: Admin/Profile/Connections
        public override IActionResult Connections()
        {
            ViewBag.IsAdminArea = true;
            ViewBag.AreaName = "Admin";
            ViewBag.AreaDisplayName = "Quản trị viên";
            return base.Connections();
        }

        // Override the mapping method to add admin-specific data
        protected override UserProfileViewModel MapToUserProfileViewModel(DATMOS.Core.Entities.Identity.AddUsers user)
        {
            var viewModel = base.MapToUserProfileViewModel(user);
            PrepareViewModel(viewModel);
            return viewModel;
        }

        // Override to add admin-specific preparation
        protected override void PrepareViewModel(UserProfileViewModel model)
        {
            base.PrepareViewModel(model);
            
            // Add admin-specific data
            ViewBag.AdminFeatures = new
            {
                CanManageUsers = true,
                CanManageCourses = true,
                CanManageContent = true,
                CanViewAnalytics = true,
                CanManageSettings = true
            };
            
            ViewBag.AdminStats = new
            {
                TotalUsers = 0, // Would be populated from service
                ActiveCourses = 0,
                PendingApprovals = 0,
                SystemHealth = "Good"
            };
        }
    }
}
