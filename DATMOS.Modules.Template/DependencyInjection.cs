using Microsoft.Extensions.DependencyInjection;

namespace DATMOS.Modules.Template
{
    /// <summary>
    /// Dependency injection extensions for DATMOS Module Template
    /// This serves as a template for all module dependency registrations
    /// </summary>
    public static class ModuleServiceCollectionExtensions
    {
        /// <summary>
        /// Adds module services to the service collection
        /// Replace [ModuleName] with actual module name when using this template
        /// </summary>
        public static IServiceCollection Add[ModuleName]Module(this IServiceCollection services)
        {
            // Register module-specific services
            // Example: services.AddScoped<I[ModuleName]Service, [ModuleName]Service>();
            
            // Register module controllers (if using feature folders or specific routing)
            // services.AddControllers()
            //     .AddApplicationPart(typeof([ModuleName]Controller).Assembly);
            
            return services;
        }
        
        /// <summary>
        /// Configures module-specific options
        /// </summary>
        public static IServiceCollection Configure[ModuleName]Options(this IServiceCollection services, Action<ModuleOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services;
        }
        
        /// <summary>
        /// Adds module-specific authorization policies
        /// </summary>
        public static IServiceCollection Add[ModuleName]Authorization(this IServiceCollection services)
        {
            // Example authorization policies
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("[ModuleName]Access", policy =>
            //         policy.RequireRole("[ModuleName]_Admin", "[ModuleName]_User"));
            // });
            
            return services;
        }
    }
    
    /// <summary>
    /// Module configuration options
    /// Customize based on module requirements
    /// </summary>
    public class ModuleOptions
    {
        public string ModuleName { get; set; } = "Template";
        public bool IsEnabled { get; set; } = true;
        public string Version { get; set; } = "1.0.0";
        
        // Module-specific settings
        public int DefaultPageSize { get; set; } = 10;
        public bool EnableCaching { get; set; } = true;
        public string ApiEndpoint { get; set; } = string.Empty;
        
        // Feature flags
        public bool EnableAdvancedFeatures { get; set; } = false;
        public bool EnableExport { get; set; } = true;
        public bool EnableImport { get; set; } = true;
    }
    
    /// <summary>
    /// Constants for module configuration
    /// </summary>
    public static class ModuleConstants
    {
        public const string ModuleArea = "[ModuleName]";
        public const string ModuleRoutePrefix = "[module-name]";
        public const string ModulePolicy = "[ModuleName]Access";
        public const string ModuleClaimType = "[ModuleName]Permissions";
        
        // Configuration section name
        public const string ConfigurationSection = "Modules:[ModuleName]";
        
        // Cache keys
        public const string CacheKeyPrefix = "[ModuleName]_";
        
        // Route templates
        public const string DefaultRoute = "[module-name]/{controller=Home}/{action=Index}/{id?}";
        public const string ApiRoute = "api/[module-name]/{controller}/{action}/{id?}";
    }
    
    /// <summary>
    /// Module information provider
    /// </summary>
    public interface IModuleInfo
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        string Author { get; }
        bool IsActive { get; }
        IEnumerable<string> Dependencies { get; }
    }
    
    /// <summary>
    /// Base implementation of module info
    /// </summary>
    public abstract class BaseModuleInfo : IModuleInfo
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual string Version => "1.0.0";
        public virtual string Author => "DATMOS Team";
        public virtual bool IsActive => true;
        public virtual IEnumerable<string> Dependencies => Array.Empty<string>();
    }
}
