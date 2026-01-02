using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DATMOS.Themes.Default.ViewComponents
{
    /// <summary>
    /// Theme Navbar ViewComponent
    /// Provides navigation bar for the entire application
    /// </summary>
    public class ThemeNavbarViewComponent : ViewComponent
    {
        private readonly ILogger<ThemeNavbarViewComponent> _logger;

        public ThemeNavbarViewComponent(ILogger<ThemeNavbarViewComponent> logger)
        {
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var model = await GetNavbarModelAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading navbar data");
                // Return empty model on error
                return View(new NavbarViewModel());
            }
        }

        private async Task<NavbarViewModel> GetNavbarModelAsync()
        {
            // In a real application, this would fetch data from services
            // For now, we'll return a static model
            
            var model = new NavbarViewModel
            {
                ApplicationName = "DATMOS",
                IsAuthenticated = HttpContext?.User?.Identity?.IsAuthenticated ?? false,
                UserName = HttpContext?.User?.Identity?.Name ?? "Guest",
                CurrentArea = GetCurrentArea(),
                MenuItems = await GetMenuItemsAsync()
            };

            return model;
        }

        private string GetCurrentArea()
        {
            var routeData = ViewContext.RouteData;
            return routeData.Values["area"]?.ToString() ?? string.Empty;
        }

        private async Task<List<NavbarMenuItem>> GetMenuItemsAsync()
        {
            var items = new List<NavbarMenuItem>
            {
                new NavbarMenuItem
                {
                    Text = "Home",
                    Url = "/",
                    Icon = "fas fa-home",
                    IsActive = GetCurrentArea() == string.Empty
                },
                new NavbarMenuItem
                {
                    Text = "Customer",
                    Url = "/Customer",
                    Icon = "fas fa-user-graduate",
                    IsActive = GetCurrentArea() == "Customer",
                    IsVisible = true // Always visible for demo
                },
                new NavbarMenuItem
                {
                    Text = "Teacher",
                    Url = "/Teacher",
                    Icon = "fas fa-chalkboard-teacher",
                    IsActive = GetCurrentArea() == "Teacher",
                    IsVisible = true
                },
                new NavbarMenuItem
                {
                    Text = "Admin",
                    Url = "/Admin",
                    Icon = "fas fa-cog",
                    IsActive = GetCurrentArea() == "Admin",
                    IsVisible = HttpContext?.User?.IsInRole("Admin") ?? false
                }
            };

            // Add authentication menu items
            if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                items.Add(new NavbarMenuItem
                {
                    Text = "Profile",
                    Url = "/Identity/Profile",
                    Icon = "fas fa-user",
                    IsActive = GetCurrentArea() == "Identity"
                });
                
                items.Add(new NavbarMenuItem
                {
                    Text = "Logout",
                    Url = "/Identity/Account/Logout",
                    Icon = "fas fa-sign-out-alt",
                    IsActive = false
                });
            }
            else
            {
                items.Add(new NavbarMenuItem
                {
                    Text = "Login",
                    Url = "/Identity/Account/Login",
                    Icon = "fas fa-sign-in-alt",
                    IsActive = GetCurrentArea() == "Identity"
                });
                
                items.Add(new NavbarMenuItem
                {
                    Text = "Register",
                    Url = "/Identity/Account/Register",
                    Icon = "fas fa-user-plus",
                    IsActive = false
                });
            }

            return await Task.FromResult(items);
        }
    }

    /// <summary>
    /// ViewModel for Navbar
    /// </summary>
    public class NavbarViewModel
    {
        public string ApplicationName { get; set; } = "DATMOS";
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = "Guest";
        public string CurrentArea { get; set; } = string.Empty;
        public List<NavbarMenuItem> MenuItems { get; set; } = new List<NavbarMenuItem>();
    }

    /// <summary>
    /// Represents a single menu item in the navbar
    /// </summary>
    public class NavbarMenuItem
    {
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; } = true;
        public List<NavbarMenuItem> Children { get; set; } = new List<NavbarMenuItem>();
    }
}
