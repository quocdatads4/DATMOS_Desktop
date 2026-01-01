using DATMOS.Web.Interfaces;
using DATMOS.Web.Areas.Identity.Controllers;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class ProfileController : BaseProfileController
    {
        public ProfileController(
            IProfileService profileService,
            UserManager<DATMOS.Core.Entities.Identity.AddUsers> userManager,
            ILogger<BaseProfileController> logger)
            : base(profileService, userManager, logger)
        {
        }

        // GET: Teacher/Profile
        public override async Task<IActionResult> Index()
        {
            var result = await base.Index();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsTeacherArea = true;
                ViewBag.AreaName = "Teacher";
                ViewBag.AreaDisplayName = "Giáo viên";
            }
            return result;
        }

        // GET: Teacher/Profile/Account
        public override async Task<IActionResult> Account()
        {
            var result = await base.Account();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsTeacherArea = true;
                ViewBag.AreaName = "Teacher";
                ViewBag.AreaDisplayName = "Giáo viên";
            }
            return result;
        }

        // GET: Teacher/Profile/Security
        public override async Task<IActionResult> Security()
        {
            var result = await base.Security();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsTeacherArea = true;
                ViewBag.AreaName = "Teacher";
                ViewBag.AreaDisplayName = "Giáo viên";
            }
            return result;
        }

        // GET: Teacher/Profile/Billing
        public override IActionResult Billing()
        {
            ViewBag.IsTeacherArea = true;
            ViewBag.AreaName = "Teacher";
            ViewBag.AreaDisplayName = "Giáo viên";
            return base.Billing();
        }

        // GET: Teacher/Profile/Notifications
        public override IActionResult Notifications()
        {
            ViewBag.IsTeacherArea = true;
            ViewBag.AreaName = "Teacher";
            ViewBag.AreaDisplayName = "Giáo viên";
            return base.Notifications();
        }

        // GET: Teacher/Profile/Connections
        public override IActionResult Connections()
        {
            ViewBag.IsTeacherArea = true;
            ViewBag.AreaName = "Teacher";
            ViewBag.AreaDisplayName = "Giáo viên";
            return base.Connections();
        }

        // Override the mapping method to add teacher-specific data
        protected override UserProfileViewModel MapToUserProfileViewModel(DATMOS.Core.Entities.Identity.AddUsers user)
        {
            var viewModel = base.MapToUserProfileViewModel(user);
            PrepareViewModel(viewModel);
            return viewModel;
        }

        // Override to add teacher-specific preparation
        protected override void PrepareViewModel(UserProfileViewModel model)
        {
            base.PrepareViewModel(model);
            
            // Add teacher-specific data
            ViewBag.TeacherFeatures = new
            {
                CanCreateCourses = true,
                CanManageStudents = true,
                CanGradeAssignments = true,
                CanScheduleClasses = true
            };
            
            ViewBag.TeacherStats = new
            {
                CreatedCourses = 0, // Would be populated from service
                TotalStudents = 0,
                AverageRating = 0,
                TeachingHours = 0
            };
        }
    }
}
