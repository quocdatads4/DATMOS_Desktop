# THIẾT KẾ KIẾN TRÚC PLUGIN THEME + MODULE
*Ngày thiết kế: 02/01/2026*
*Phục vụ cho Modularization Project - Kế hoạch 1*

## KIẾN TRÚC TỔNG QUAN

### 1. LAYER ARCHITECTURE

```
┌─────────────────────────────────────────────────┐
│               DATMOS.Web (Host)                 │
│  - Program.cs (Host configuration)             │
│  - Startup configuration                       │
│  - Main entry point                            │
└─────────────────────────────────────────────────┘
                         │
                         │ Project References
                         ▼
┌─────────────────────────────────────────────────┐
│           DATMOS.Themes.Default (RCL)           │
│  - Base layouts & templates                    │
│  - Shared ViewComponents                       │
│  - Global CSS/JS framework                     │
│  - Authentication infrastructure               │
└─────────────────────────────────────────────────┘
                         │
                         │ Project References
                         ▼
┌───────────┬───────────┬───────────┬───────────┐
│  Module   │  Module   │  Module   │  Module   │
│   Admin   │ Customer  │ Identity  │  Teacher  │
│  (RCL)    │   (RCL)   │  (RCL)    │   (RCL)   │
└───────────┴───────────┴───────────┴───────────┘
```

### 2. PROJECT STRUCTURE

#### DATMOS.Themes.Default (Razor Class Library)
```
DATMOS.Themes.Default/
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml          (Base layout)
│   │   ├── _AdminLayout.cshtml     (Extends base)
│   │   ├── _CustomerLayout.cshtml  (Extends base)
│   │   ├── _TeacherLayout.cshtml   (Extends base)
│   │   └── _IdentityLayout.cshtml  (Extends base)
│   └── Components/
│       ├── Navbar/
│       │   └── Default.cshtml
│       ├── Menu/
│       │   └── Default.cshtml
│       └── Footer/
│           └── Default.cshtml
├── ViewComponents/
│   ├── ThemeNavbarViewComponent.cs
│   ├── ThemeMenuViewComponent.cs
│   └── ThemeFooterViewComponent.cs
├── wwwroot/
│   ├── css/
│   │   ├── theme.css
│   │   ├── components.css
│   │   └── utilities.css
│   ├── js/
│   │   ├── theme.js
│   │   └── components.js
│   └── images/
├── Services/
│   ├── IThemeService.cs
│   └── ThemeService.cs
├── DependencyInjection.cs
└── DATMOS.Themes.Default.csproj
```

#### DATMOS.Modules.[Name] (Razor Class Library Template)
```
DATMOS.Modules.Template/
├── Controllers/
│   └── [Area]Controller.cs
├── Views/
│   ├── [Controller]/
│   │   └── [Action].cshtml
│   └── _ViewImports.cshtml
├── ViewModels/
│   └── [ViewModel].cs
├── Services/
│   ├── I[Service].cs
│   └── [Service].cs
├── wwwroot/
│   ├── css/
│   │   └── module.css
│   └── js/
│       └── module.js
├── DependencyInjection.cs
└── DATMOS.Modules.Template.csproj
```

## 3. DEPENDENCY INJECTION DESIGN

### Theme Registration (DATMOS.Themes.Default)
```csharp
// DependencyInjection.cs
public static class ThemeServiceCollectionExtensions
{
    public static IServiceCollection AddThemeServices(this IServiceCollection services)
    {
        // Register theme services
        services.AddScoped<IThemeService, ThemeService>();
        
        // Register theme ViewComponents
        services.AddScoped<ThemeNavbarViewComponent>();
        services.AddScoped<ThemeMenuViewComponent>();
        services.AddScoped<ThemeFooterViewComponent>();
        
        return services;
    }
}
```

### Module Registration (DATMOS.Modules.[Name])
```csharp
// DependencyInjection.cs in each module
public static class ModuleServiceCollectionExtensions
{
    public static IServiceCollection AddAdminModule(this IServiceCollection services)
    {
        // Register module-specific services
        services.AddScoped<IAdminService, AdminService>();
        
        return services;
    }
}
```

### Host Configuration (DATMOS.Web Program.cs)
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add theme services
builder.Services.AddThemeServices();

// Add module services
builder.Services.AddAdminModule();
builder.Services.AddCustomerModule();
builder.Services.AddIdentityModule();
builder.Services.AddTeacherModule();

// MVC configuration
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
```

## 4. ROUTING CONFIGURATION

### Base Route Configuration
```csharp
// Program.cs
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### Module-Specific Routes (Optional)
```csharp
// In module's DependencyInjection.cs
public static IEndpointRouteBuilder MapAdminRoutes(this IEndpointRouteBuilder endpoints)
{
    endpoints.MapAreaControllerRoute(
        name: "admin_default",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
    
    return endpoints;
}
```

## 5. VIEW LOCATION EXPANDERS

### Custom ViewLocationExpander for Modules
```csharp
// In DATMOS.Web Program.cs
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());
});

public class ModuleViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context) { }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        // Add module view locations
        var locations = new List<string>
        {
            // Module views
            "/Areas/{2}/Views/{1}/{0}.cshtml",
            "/Areas/{2}/Views/Shared/{0}.cshtml",
            
            // Theme views
            "/Themes/Default/Views/{1}/{0}.cshtml",
            "/Themes/Default/Views/Shared/{0}.cshtml",
        };
        
        return locations.Concat(viewLocations);
    }
}
```

## 6. STATIC FILES CONFIGURATION

### Theme Static Files
```csharp
// In DATMOS.Themes.Default wwwroot
// Files automatically available at: ~/_content/DATMOS.Themes.Default/
```

### Module Static Files
```csharp
// In DATMOS.Modules.[Name] wwwroot
// Files automatically available at: ~/_content/DATMOS.Modules.[Name]/
```

### Host Static Files Configuration
```csharp
// Program.cs
app.UseStaticFiles(); // For wwwroot in DATMOS.Web
// Module and theme static files are automatically served via static web assets
```

## 7. DATABASE & ENTITY DESIGN

### Shared Entities (DATMOS.Core)
- Tất cả entities giữ nguyên trong DATMOS.Core
- Modules reference DATMOS.Core để sử dụng entities

### DbContext (DATMOS.Data)
- `AppDbContext` giữ nguyên trong DATMOS.Data
- Modules sử dụng DbContext thông qua dependency injection

### Module-Specific Configurations
```csharp
// In each module, can add Fluent API configurations
public class ModuleDbContextConfiguration : IEntityTypeConfiguration<ModuleEntity>
{
    public void Configure(EntityTypeBuilder<ModuleEntity> builder)
    {
        // Module-specific configurations
    }
}
```

## 8. AUTHENTICATION & AUTHORIZATION

### Centralized in Theme
```csharp
// DATMOS.Themes.Default Services/
public interface IThemeAuthenticationService
{
    Task<bool> HasAccessAsync(string area, string controller, string action);
}

// Implementation uses ASP.NET Core Identity
```

### Module Authorization Attributes
```csharp
// Custom attribute for module authorization
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ModuleAuthorizeAttribute : AuthorizeAttribute
{
    public ModuleAuthorizeAttribute(string moduleName)
    {
        Roles = $"{moduleName}_Access";
    }
}

// Usage in module controllers
[ModuleAuthorize("Admin")]
public class AdminController : Controller
{
    // ...
}
```

## 9. BUILD CONFIGURATION

### Solution Structure
```
DATMOS_Desktop.sln
├── DATMOS.Core.csproj
├── DATMOS.Data.csproj
├── DATMOS.Web.csproj (Host)
├── DATMOS.Themes.Default.csproj
├── DATMOS.Modules.Admin.csproj
├── DATMOS.Modules.Customer.csproj
├── DATMOS.Modules.Identity.csproj
└── DATMOS.Modules.Teacher.csproj
```

### Project References
```xml
<!-- DATMOS.Web.csproj -->
<ItemGroup>
  <ProjectReference Include="..\DATMOS.Core\DATMOS.Core.csproj" />
  <ProjectReference Include="..\DATMOS.Data\DATMOS.Data.csproj" />
  <ProjectReference Include="..\DATMOS.Themes.Default\DATMOS.Themes.Default.csproj" />
  <ProjectReference Include="..\DATMOS.Modules.Admin\DATMOS.Modules.Admin.csproj" />
  <ProjectReference Include="..\DATMOS.Modules.Customer\DATMOS.Modules.Customer.csproj" />
  <ProjectReference Include="..\DATMOS.Modules.Identity\DATMOS.Modules.Identity.csproj" />
  <ProjectReference Include="..\DATMOS.Modules.Teacher\DATMOS.Modules.Teacher.csproj" />
</ItemGroup>

<!-- DATMOS.Modules.[Name].csproj -->
<ItemGroup>
  <ProjectReference Include="..\DATMOS.Core\DATMOS.Core.csproj" />
  <ProjectReference Include="..\DATMOS.Themes.Default\DATMOS.Themes.Default.csproj" />
</ItemGroup>
```

## 10. DEVELOPMENT WORKFLOW

### 1. Create New Module
```bash
# Copy template
cp -r DATMOS.Modules.Template DATMOS.Modules.NewModule

# Update namespaces and project name
# Add project reference to solution
# Register module in Program.cs
```

### 2. Build Process
```bash
# Build all projects
dotnet build

# Or build specific module
dotnet build DATMOS.Modules.Admin
```

### 3. Testing
- Unit tests in each module
- Integration tests in DATMOS.Web
- End-to-end tests for full workflow

## 11. MIGRATION STRATEGY

### Phase 1: Create Theme
1. Tạo DATMOS.Themes.Default
2. Di chuyển shared layouts và components
3. Cập nhật Areas hiện tại sử dụng Theme

### Phase 2: Create Module Template
1. Tạo DATMOS.Modules.Template
2. Thiết lập cấu trúc chuẩn
3. Tạo build scripts

### Phase 3: Migrate Modules
1. DATMOS.Modules.Identity (Highest priority)
2. DATMOS.Modules.Admin
3. DATMOS.Modules.Customer
4. DATMOS.Modules.Teacher

### Phase 4: Cleanup
1. Xóa Areas cũ từ DATMOS.Web
2. Update documentation
3. Performance testing

## 12. RISK MITIGATION

### Technical Risks:
1. **Build time increase**
   - Solution: Incremental builds, parallel compilation
   
2. **Dependency conflicts**
   - Solution: Strict versioning, dependency isolation
   
3. **Runtime performance**
   - Solution: Caching, optimized static file serving

### Process Risks:
1. **Team coordination**
   - Solution: Clear module boundaries, API contracts
   
2. **Testing complexity**
   - Solution: Comprehensive test suite, CI/CD pipeline

## 13. SUCCESS METRICS

### Technical Metrics:
- Build time: < 30% increase
- Runtime performance: < 10% degradation
- Test coverage: > 80% for each module
- Code duplication: < 5%

### Business Metrics:
- Development velocity: +30%
- Bug rate: -20%
- Feature deployment time: -50%

---

*Thiết kế này sẽ được điều chỉnh dựa trên feedback và phát hiện trong quá trình thực hiện.*
