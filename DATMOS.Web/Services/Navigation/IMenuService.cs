using DATMOS.Web.Models.ViewModels.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services.Navigation
{
    public interface IMenuService
    {
        Task<List<MenuItemViewModel>> GetMenuAsync(string menuType, string? area = null);
        Task<MenuItemViewModel?> GetMenuItemByIdAsync(string id);
        Task<List<MenuItemViewModel>> GetActiveMenuAsync(string menuType, string? area = null);
        Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null);
        Task ClearCacheAsync(string menuType);
    }
}
