using DATMOS.Core.Entities;
using DATMOS.Data;
using DATMOS.Web.Models.ViewModels.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Services.Navigation
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public MenuService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<MenuItemViewModel>> GetMenuAsync(string menuType, string? area = null)
        {
            var cacheKey = $"Menu_{menuType}_{area ?? "all"}";
            
            if (!_cache.TryGetValue(cacheKey, out List<MenuItemViewModel>? menuItems))
            {
                var query = _context.Menus
                    .Where(m => m.MenuType == menuType && m.IsVisible);

                if (!string.IsNullOrEmpty(area))
                {
                    query = query.Where(m => m.Area == area);
                }

                query = query.OrderBy(m => m.Order);

                var dbMenus = await query.ToListAsync();
                menuItems = BuildMenuHierarchy(dbMenus, null);
                
                _cache.Set(cacheKey, menuItems, _cacheDuration);
            }

            return menuItems ?? new List<MenuItemViewModel>();
        }

        public async Task<MenuItemViewModel?> GetMenuItemByIdAsync(string id)
        {
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
                return null;

            return MapToViewModel(menu);
        }

        public async Task<List<MenuItemViewModel>> GetActiveMenuAsync(string menuType, string? area = null)
        {
            // For active menu, we might want to apply additional business logic
            // For now, just return the visible menu
            return await GetMenuAsync(menuType, area);
        }

        public async Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null)
        {
            var menuItems = await GetMenuAsync(menuType, area);
            
            return new MenuDataViewModel
            {
                MenuItems = menuItems,
                Metadata = new MenuMetadataViewModel
                {
                    Version = "1.0.0",
                    LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = $"{menuType} menu for area: {area ?? "all"}"
                }
            };
        }

        public Task ClearCacheAsync(string menuType)
        {
            // Clear all cache entries for this menu type
            var cacheKeys = new List<string>
            {
                $"Menu_{menuType}_all",
                $"Menu_{menuType}_Admin",
                $"Menu_{menuType}_Customer",
                $"Menu_{menuType}_Teacher",
                $"Menu_{menuType}_Landing"
            };

            foreach (var key in cacheKeys)
            {
                _cache.Remove(key);
            }

            return Task.CompletedTask;
        }

        private List<MenuItemViewModel> BuildMenuHierarchy(List<Menu> menus, string? parentId)
        {
            var result = new List<MenuItemViewModel>();

            var parentMenus = menus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .ToList();

            foreach (var menu in parentMenus)
            {
                var viewModel = MapToViewModel(menu);
                viewModel.Children = BuildMenuHierarchy(menus, menu.Id);
                result.Add(viewModel);
            }

            return result;
        }

        private MenuItemViewModel MapToViewModel(Menu menu)
        {
            return new MenuItemViewModel
            {
                Id = menu.Id,
                Text = menu.Text,
                Icon = menu.Icon,
                Area = menu.Area,
                Controller = menu.Controller,
                Action = menu.Action,
                Url = menu.Url,
                Order = menu.Order,
                IsVisible = menu.IsVisible,
                MenuType = menu.MenuType,
                Children = new List<MenuItemViewModel>()
            };
        }
    }
}
