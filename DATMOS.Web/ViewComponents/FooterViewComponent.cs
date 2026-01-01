using DATMOS.Web.Services.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DATMOS.Web.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IFooterService _footerService;
        
        public FooterViewComponent(IFooterService footerService)
        {
            _footerService = footerService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(string footerType, string? area = null)
        {
            var footerData = await _footerService.GetFooterDataAsync(footerType, area);
            return View(footerData);
        }
    }
}
