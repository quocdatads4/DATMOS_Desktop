using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to Customer Area Home by default
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
