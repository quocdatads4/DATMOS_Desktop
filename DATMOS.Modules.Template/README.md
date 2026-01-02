# DATMOS Module Template

This is a template project for creating DATMOS modules. Use this as a starting point for developing new modules in the DATMOS modular architecture.

## 📁 Project Structure

```
DATMOS.Modules.Template/
├── Controllers/
│   └── TemplateController.cs          # Example controller with Area routing
├── Views/
│   └── Template/
│       └── Index.cshtml              # Example view with layout inheritance
├── ViewModels/                        # ViewModels for your module
├── Services/                          # Business logic services
├── wwwroot/
│   ├── css/                          # Module-specific CSS
│   └── js/                           # Module-specific JavaScript
├── DependencyInjection.cs            # DI configuration for the module
└── README.md                         # This file
```

## 🚀 Getting Started

### 1. Create a New Module

```bash
# Copy the template
cp -r DATMOS.Modules.Template DATMOS.Modules.YourModuleName

# Update namespaces and project name
# Replace all occurrences of:
#   [ModuleName] → YourModuleName
#   [module-name] → your-module-name
#   Template → YourModuleName
```

### 2. Update Project File

Edit `DATMOS.Modules.YourModuleName.csproj`:
- Update the project name
- Ensure correct references to DATMOS.Core and DATMOS.Themes.Default

### 3. Register the Module

In `DATMOS.Web/Program.cs`:
```csharp
// Add module services
builder.Services.AddYourModuleNameModule();

// Configure module options
builder.Services.ConfigureYourModuleNameOptions(options =>
{
    options.ModuleName = "YourModuleName";
    options.IsEnabled = true;
    // ... other options
});
```

### 4. Add to Solution

```bash
dotnet sln DATMOS_Desktop.sln add DATMOS.Modules.YourModuleName
```

## 🛠️ Development Guidelines

### Controller Naming
- Use `[Area("YourModuleName")]` attribute
- Follow naming convention: `YourModuleNameController`
- Use route prefix: `[Route("your-module-name")]`

### Views
- Inherit from theme layout: `Layout = "~/Views/Shared/_Layout.cshtml";`
- Store views in `Views/YourModuleName/` directory
- Use ViewData for module-specific information

### Services
- Create interfaces in `Services/` directory
- Implement services with proper dependency injection
- Register services in `DependencyInjection.cs`

### Static Assets
- Place CSS in `wwwroot/css/`
- Place JavaScript in `wwwroot/js/`
- Reference using `~/_content/DATMOS.Modules.YourModuleName/`

## 📦 Module Configuration

### Options Class
Each module should have an `Options` class in `DependencyInjection.cs`:
```csharp
public class YourModuleNameOptions
{
    public string ModuleName { get; set; } = "YourModuleName";
    public bool IsEnabled { get; set; } = true;
    // Add module-specific settings
}
```

### Configuration in appsettings.json
```json
{
  "Modules": {
    "YourModuleName": {
      "IsEnabled": true,
      "DefaultPageSize": 10,
      "EnableCaching": true
    }
  }
}
```

## 🔧 Dependency Injection

### Module Registration
```csharp
public static IServiceCollection AddYourModuleNameModule(this IServiceCollection services)
{
    // Register services
    services.AddScoped<IYourModuleNameService, YourModuleNameService>();
    
    // Configure options
    services.AddOptions<YourModuleNameOptions>()
        .BindConfiguration("Modules:YourModuleName");
    
    return services;
}
```

## 🚨 Error Handling

### Module-Specific Exceptions
```csharp
public class YourModuleNameException : Exception
{
    public YourModuleNameException(string message) : base(message) { }
}
```

### Error Views
Create error views in `Views/YourModuleName/Error/` directory.

## 📊 Health Checks

### Implement Health Check
```csharp
public class YourModuleNameHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Check module health
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
```

### Register Health Check
```csharp
services.AddHealthChecks()
    .AddCheck<YourModuleNameHealthCheck>("your-module-name-health");
```

## 🔒 Security

### Authorization Policies
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("YourModuleNameAccess", policy =>
        policy.RequireRole("YourModuleName_Admin", "YourModuleName_User"));
});
```

### Secure Controllers
```csharp
[Authorize(Policy = "YourModuleNameAccess")]
[Area("YourModuleName")]
public class YourModuleNameController : Controller
{
    // Controller actions
}
```

## 📈 Monitoring & Logging

### Module-Specific Logging
```csharp
public class YourModuleNameController : Controller
{
    private readonly ILogger<YourModuleNameController> _logger;
    
    public YourModuleNameController(ILogger<YourModuleNameController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        _logger.LogInformation("Accessing {ModuleName} dashboard", "YourModuleName");
        return View();
    }
}
```

## 🧪 Testing

### Unit Tests
- Create test project: `DATMOS.Modules.YourModuleName.Tests`
- Test controllers, services, and models
- Use xUnit or NUnit

### Integration Tests
- Test module integration with DATMOS.Web
- Test database interactions
- Test API endpoints

## 📝 Documentation

### Module Documentation
Create documentation in:
1. `docs/` directory within the module
2. XML documentation comments
3. README.md updates

### API Documentation
Use Swagger/OpenAPI for API endpoints:
```csharp
[ApiExplorerSettings(GroupName = "YourModuleName")]
public class YourModuleNameApiController : ControllerBase
{
    // API actions
}
```

## 🔄 Versioning

### Module Versioning
- Follow Semantic Versioning (MAJOR.MINOR.PATCH)
- Update version in `ModuleOptions`
- Document breaking changes

### Database Migrations
If your module requires database changes:
1. Create migrations in the module
2. Apply migrations during module initialization
3. Handle migration rollbacks

## 🤝 Contributing

### Code Style
- Follow DATMOS coding conventions
- Use meaningful variable names
- Add XML documentation comments
- Write unit tests

### Pull Requests
1. Create feature branch from `develop`
2. Implement changes
3. Add/update tests
4. Update documentation
5. Submit PR for review

## 📄 License

This module template is part of the DATMOS project. See the main project for licensing information.

---

**Happy Module Development!** 🎉

For questions or support, contact the DATMOS development team.
