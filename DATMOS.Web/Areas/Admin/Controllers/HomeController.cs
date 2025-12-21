using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin,Manager")] // Temporarily disabled for testing
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
