using DATMOS.Web.Interfaces;
using DATMOS.Web.Areas.Identity.Controllers;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class ProfileController : BaseProfileController
    {
        public ProfileController(
            IProfileService profileService,
            UserManager<DATMOS.Core.Entities.Identity.AddUsers> userManager,
            ILogger<BaseProfileController> logger)
            : base(profileService, userManager, logger)
        {
        }

        // GET: Customer/Profile
        public override async Task<IActionResult> Index()
        {
            var result = await base.Index();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsCustomerArea = true;
                ViewBag.AreaName = "Customer";
                ViewBag.AreaDisplayName = "Học viên";
            }
            return result;
        }

        // GET: Customer/Profile/Account
        public override async Task<IActionResult> Account()
        {
            var result = await base.Account();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsCustomerArea = true;
                ViewBag.AreaName = "Customer";
                ViewBag.AreaDisplayName = "Học viên";
            }
            return result;
        }

        // GET: Customer/Profile/Security
        public override async Task<IActionResult> Security()
        {
            var result = await base.Security();
            if (result is ViewResult viewResult)
            {
                ViewBag.IsCustomerArea = true;
                ViewBag.AreaName = "Customer";
                ViewBag.AreaDisplayName = "Học viên";
            }
            return result;
        }

        // GET: Customer/Profile/Billing
        public override IActionResult Billing()
        {
            ViewBag.IsCustomerArea = true;
            ViewBag.AreaName = "Customer";
            ViewBag.AreaDisplayName = "Học viên";
            return base.Billing();
        }

        // GET: Customer/Profile/Notifications
        public override IActionResult Notifications()
        {
            ViewBag.IsCustomerArea = true;
            ViewBag.AreaName = "Customer";
            ViewBag.AreaDisplayName = "Học viên";
            return base.Notifications();
        }

        // GET: Customer/Profile/Connections
        public override IActionResult Connections()
        {
            ViewBag.IsCustomerArea = true;
            ViewBag.AreaName = "Customer";
            ViewBag.AreaDisplayName = "Học viên";
            return base.Connections();
        }

        // Override the mapping method to add customer-specific data
        protected override UserProfileViewModel MapToUserProfileViewModel(DATMOS.Core.Entities.Identity.AddUsers user)
        {
            var viewModel = base.MapToUserProfileViewModel(user);
            PrepareViewModel(viewModel);
            return viewModel;
        }

        // Override to add customer-specific preparation
        protected override void PrepareViewModel(UserProfileViewModel model)
        {
            base.PrepareViewModel(model);
            
            // Add customer-specific data
            ViewBag.CustomerFeatures = new
            {
                CanViewCourses = true,
                CanTakeExams = true,
                CanViewProgress = true,
                CanDownloadMaterials = true
            };
            
            ViewBag.CustomerStats = new
            {
                EnrolledCourses = 0, // Would be populated from service
                CompletedCourses = 0,
                AverageScore = 0,
                LearningHours = 0
            };
        }
    }
}
