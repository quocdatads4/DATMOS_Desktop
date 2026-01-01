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
    public class FooterService : IFooterService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public FooterService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<FooterItemViewModel>> GetFooterAsync(string footerType, string? area = null)
        {
            var cacheKey = $"footer_{footerType}_{area ?? "all"}";
            
            if (!_cache.TryGetValue(cacheKey, out List<FooterItemViewModel>? footerItems))
            {
                var query = _context.Footers
                    .Where(f => f.FooterType == footerType && f.IsVisible);

                if (!string.IsNullOrEmpty(area))
                {
                    query = query.Where(f => f.Area == area || string.IsNullOrEmpty(f.Area));
                }

                var entities = await query.OrderBy(f => f.Order).ToListAsync();
                footerItems = MapToViewModels(entities);
                
                _cache.Set(cacheKey, footerItems, _cacheDuration);
            }

            return footerItems ?? new List<FooterItemViewModel>();
        }

        public async Task<FooterItemViewModel?> GetFooterItemByIdAsync(string id)
        {
            var cacheKey = $"footer_item_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out FooterItemViewModel? footerItem))
            {
                var entity = await _context.Footers
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (entity != null)
                {
                    footerItem = MapToViewModel(entity);
                    _cache.Set(cacheKey, footerItem, _cacheDuration);
                }
            }

            return footerItem;
        }

        public async Task<Dictionary<string, List<FooterItemViewModel>>> GetFooterBySectionsAsync(string footerType, string? area = null)
        {
            var cacheKey = $"footer_sections_{footerType}_{area ?? "all"}";
            
            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, List<FooterItemViewModel>>? footerBySections))
            {
                var footerItems = await GetFooterAsync(footerType, area);
                footerBySections = GroupBySections(footerItems);
                
                _cache.Set(cacheKey, footerBySections, _cacheDuration);
            }

            return footerBySections ?? new Dictionary<string, List<FooterItemViewModel>>();
        }

        public async Task<FooterDataViewModel> GetFooterDataAsync(string footerType, string? area = null)
        {
            var cacheKey = $"footer_data_{footerType}_{area ?? "all"}";
            
            if (!_cache.TryGetValue(cacheKey, out FooterDataViewModel? footerData))
            {
                var footerBySections = await GetFooterBySectionsAsync(footerType, area);
                
                footerData = new FooterDataViewModel
                {
                    FooterType = footerType,
                    Area = area,
                    FooterBySections = footerBySections
                };
                
                _cache.Set(cacheKey, footerData, _cacheDuration);
            }

            return footerData ?? new FooterDataViewModel { FooterType = footerType, Area = area };
        }

        public async Task ClearCacheAsync(string footerType)
        {
            var cacheKeys = new List<string>
            {
                $"footer_{footerType}_all",
                $"footer_sections_{footerType}_all",
                $"footer_data_{footerType}_all"
            };

            foreach (var key in cacheKeys)
            {
                _cache.Remove(key);
            }

            // Also clear area-specific caches if needed
            var areas = await _context.Footers
                .Where(f => f.FooterType == footerType && !string.IsNullOrEmpty(f.Area))
                .Select(f => f.Area)
                .Distinct()
                .ToListAsync();

            foreach (var area in areas)
            {
                if (!string.IsNullOrEmpty(area))
                {
                    _cache.Remove($"footer_{footerType}_{area}");
                    _cache.Remove($"footer_sections_{footerType}_{area}");
                    _cache.Remove($"footer_data_{footerType}_{area}");
                }
            }
        }

        private List<FooterItemViewModel> MapToViewModels(List<Footer> entities)
        {
            var viewModels = entities.Select(MapToViewModel).ToList();
            
            // Build hierarchy
            var lookup = viewModels.ToDictionary(vm => vm.Id);
            foreach (var vm in viewModels)
            {
                if (!string.IsNullOrEmpty(vm.ParentId) && lookup.TryGetValue(vm.ParentId, out var parent))
                {
                    vm.Parent = parent;
                    parent.Children.Add(vm);
                }
            }

            // Return only root items (items without parent or with null ParentId)
            return viewModels.Where(vm => string.IsNullOrEmpty(vm.ParentId)).ToList();
        }

        private FooterItemViewModel MapToViewModel(Footer entity)
        {
            return new FooterItemViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Icon = entity.Icon,
                Content = entity.Content,
                Url = entity.Url,
                Section = entity.Section,
                Order = entity.Order,
                IsVisible = entity.IsVisible,
                FooterType = entity.FooterType,
                ParentId = entity.ParentId,
                Area = entity.Area,
                Controller = entity.Controller,
                Action = entity.Action,
                Target = entity.Target,
                CssClass = entity.CssClass
            };
        }

        private Dictionary<string, List<FooterItemViewModel>> GroupBySections(List<FooterItemViewModel> footerItems)
        {
            var result = new Dictionary<string, List<FooterItemViewModel>>();
            
            // Flatten hierarchy for grouping (include children in their respective sections)
            var allItems = FlattenHierarchy(footerItems);
            
            foreach (var item in allItems.Where(i => i.IsVisible).OrderBy(i => i.Order))
            {
                if (!result.ContainsKey(item.Section))
                {
                    result[item.Section] = new List<FooterItemViewModel>();
                }
                
                result[item.Section].Add(item);
            }
            
            return result;
        }

        private List<FooterItemViewModel> FlattenHierarchy(List<FooterItemViewModel> items)
        {
            var flattened = new List<FooterItemViewModel>();
            
            foreach (var item in items)
            {
                flattened.Add(item);
                if (item.Children.Any())
                {
                    flattened.AddRange(FlattenHierarchy(item.Children));
                }
            }
            
            return flattened;
        }
    }
}
