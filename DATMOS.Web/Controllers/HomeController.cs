using DATMOS.Web.Services.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DATMOS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFooterService _footerService;

        public HomeController(IFooterService footerService)
        {
            _footerService = footerService;
        }

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

        public IActionResult TestFooter()
        {
            return View();
        }

        public async Task<IActionResult> TestFooterService()
        {
            try
            {
                var customerFooter = await _footerService.GetFooterAsync("Customer");
                var adminFooter = await _footerService.GetFooterAsync("Admin");
                
                return Json(new
                {
                    status = "Success",
                    customerCount = customerFooter?.Count ?? 0,
                    adminCount = adminFooter?.Count ?? 0,
                    message = "Footer service is working correctly"
                });
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    status = "Error",
                    customerCount = 0,
                    adminCount = 0,
                    message = ex.Message
                });
            }
        }
    }
}
