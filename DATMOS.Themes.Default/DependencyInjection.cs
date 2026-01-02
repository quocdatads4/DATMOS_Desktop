using Microsoft.Extensions.DependencyInjection;

namespace DATMOS.Themes.Default
{
    /// <summary>
    /// Dependency injection extensions for DATMOS Theme
    /// </summary>
    public static class ThemeServiceCollectionExtensions
    {
        /// <summary>
        /// Adds theme services to the service collection
        /// </summary>
        public static IServiceCollection AddThemeServices(this IServiceCollection services)
        {
            // Register theme ViewComponents
            services.AddScoped<ViewComponents.ThemeNavbarViewComponent>();
            // Note: ThemeMenuViewComponent and ThemeFooterViewComponent will be added later
            
            // Register theme services (to be implemented)
            // services.AddScoped<IThemeService, ThemeService>();
            
            return services;
        }
        
        /// <summary>
        /// Configures theme options
        /// </summary>
        public static IServiceCollection ConfigureThemeOptions(this IServiceCollection services, Action<ThemeOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services;
        }
    }
    
    /// <summary>
    /// Theme configuration options
    /// </summary>
    public class ThemeOptions
    {
        public string SiteName { get; set; } = "DATMOS";
        public string DefaultLayout { get; set; } = "_Layout";
        public bool EnableDarkMode { get; set; } = true;
        public string PrimaryColor { get; set; } = "#007bff";
        public string SecondaryColor { get; set; } = "#6c757d";
        public string FontFamily { get; set; } = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
        
        // Layout settings
        public bool FixedNavbar { get; set; } = true;
        public bool ShowBreadcrumb { get; set; } = true;
        public bool ShowSidebar { get; set; } = true;
        
        // Feature flags
        public bool EnableAnimations { get; set; } = true;
        public bool EnableRTL { get; set; } = false;
        public bool EnableAccessibilityFeatures { get; set; } = true;
    }
}
