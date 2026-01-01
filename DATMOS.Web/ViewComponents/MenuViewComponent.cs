using DATMOS.Web.Services.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DATMOS.Web.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        
        public MenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(string menuType, string? area = null)
        {
            var menuData = await _menuService.GetMenuDataAsync(menuType, area);
            return View(menuData);
        }
    }
}
