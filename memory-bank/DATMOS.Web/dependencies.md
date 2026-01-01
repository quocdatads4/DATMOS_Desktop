# DATMOS.Web - Dependencies

## Tổng quan
Tài liệu này liệt kê tất cả dependencies của DATMOS.Web project, bao gồm NuGet packages, project references, và external dependencies.

## Project References

### 1. Internal Projects
```xml
<!-- DATMOS.Web.csproj -->
<ItemGroup>
  <ProjectReference Include="..\DATMOS.Data\DATMOS.Data.csproj" />
  <ProjectReference Include="..\DATMOS.Core\DATMOS.Core.csproj" />
</ItemGroup>
```

**DATMOS.Data**:
- **Mục đích**: Data access layer với Entity Framework Core
- **Version**: .NET 8.0
- **Chức năng**: DbContext, Migrations, Seed data

**DATMOS.Core**:
- **Mục đích**: Domain layer với entities và interfaces
- **Version**: .NET 8.0
- **Chức năng**: Domain models, business interfaces

### 2. External NuGet Packages

#### ASP.NET Core Framework
```xml
<PackageReference Include="Microsoft.AspNetCore.App" />
```
- **Version**: Implicit (framework-dependent)
- **Mục đích**: ASP.NET Core runtime và libraries
- **Includes**: MVC, Razor, Kestrel, Dependency Injection, etc.

#### Entity Framework Core
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```
- **Microsoft.EntityFrameworkCore**: EF Core core library
- **Microsoft.EntityFrameworkCore.Design**: Design-time tools (migrations)
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider cho EF Core

#### JSON Serialization
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.0" />
```
- **Newtonsoft.Json**: JSON serialization/deserialization
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson**: Integration với ASP.NET Core MVC

#### WebView2 (cho WinApp integration)
```xml
<!-- Trong DATMOS.WinApp.csproj -->
<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.2365.46" />
```
- **Microsoft.Web.WebView2**: WebView2 control cho WinForms

#### Development Tools
```xml
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.0" />
```
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: Scaffolding tools

## Runtime Dependencies

### 1. .NET Runtime
- **.NET 8.0 SDK**: Required for development và build
- **.NET 8.0 Runtime**: Required for execution

### 2. Database
- **PostgreSQL 14+**: Production database
- **PostgreSQL Service**: Must be running và accessible
- **Connection String**: Configured in appsettings.json

### 3. WebView2 Runtime
- **Microsoft Edge WebView2 Runtime**: Required for WinApp integration
- **Auto-installed**: Usually included với Windows updates
- **Fallback**: Can be distributed với application

## Configuration Dependencies

### 1. appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=datmos_db;Username=postgres;Password=password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. launchSettings.json
```json
{
  "profiles": {
    "DATMOS.Web": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7243;http://localhost:5243",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Build Dependencies

### 1. Build Tools
- **.NET CLI**: `dotnet` command-line tools
- **Entity Framework Core Tools**: `dotnet ef` commands
- **MSBuild**: Build system

### 2. Build Configuration
```xml
<!-- DATMOS.Web.csproj -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## Development Dependencies

### 1. IDE/Editor
- **Visual Studio 2022+**: Recommended IDE
- **Visual Studio Code**: Alternative với C# extension
- **Rider**: JetBrains alternative

### 2. Development Tools
- **Git**: Version control
- **PostgreSQL Client**: pgAdmin, DBeaver, hoặc psql
- **Browser Developer Tools**: For frontend debugging

### 3. Testing Tools
- **xUnit/NUnit**: Unit testing frameworks
- **Moq**: Mocking framework
- **Selenium/Playwright**: E2E testing

## Deployment Dependencies

### 1. Self-contained Deployment
```bash
dotnet publish -c Release -r win-x64 --self-contained
```
- **Runtime**: Included trong distribution
- **Dependencies**: All bundled với application
- **Size**: Larger distribution size

### 2. Framework-dependent Deployment
```bash
dotnet publish -c Release
```
- **Runtime**: Requires .NET 8.0 runtime trên target machine
- **Dependencies**: Smaller distribution size
- **Prerequisite**: .NET runtime must be installed

### 3. Database Deployment
- **PostgreSQL**: Must be installed và configured
- **Migrations**: Auto-applied on startup
- **Seed Data**: Optional seed data

## Security Dependencies

### 1. Authentication/Authorization
- **ASP.NET Core Identity**: User management (có thể tích hợp)
- **JWT Bearer**: API authentication (có thể tích hợp)
- **OAuth/OpenID Connect**: External authentication (có thể tích hợp)

### 2. Security Libraries
- **DataAnnotations**: Input validation
- **AntiForgery**: CSRF protection
- **HSTS**: HTTP Strict Transport Security

## Monitoring Dependencies

### 1. Logging
- **Serilog**: Structured logging (có thể tích hợp)
- **Console/File Logging**: Built-in ASP.NET Core logging
- **Application Insights**: Azure monitoring (có thể tích hợp)

### 2. Performance Monitoring
- **MiniProfiler**: Performance profiling (có thể tích hợp)
- **Health Checks**: Application health monitoring

## Compatibility Matrix

### 1. .NET Versions
| Component | Minimum Version | Recommended Version |
|-----------|----------------|---------------------|
| .NET SDK | 8.0.0 | 8.0.100+ |
| .NET Runtime | 8.0.0 | 8.0.0+ |

### 2. Database
| Component | Minimum Version | Recommended Version |
|-----------|----------------|---------------------|
| PostgreSQL | 14.0 | 16.0+ |
| Npgsql | 8.0.0 | 8.0.0+ |

### 3. Operating Systems
| OS | Development | Production |
|----|-------------|------------|
| Windows 10 | ✅ | ✅ |
| Windows 11 | ✅ | ✅ |
| macOS | ✅ (development only) | ⚠️ (limited) |
| Linux | ✅ (development only) | ⚠️ (limited) |

## Dependency Management

### 1. NuGet Package Management
```bash
# Add package
dotnet add package PackageName

# Update package
dotnet update package PackageName

# Remove package
dotnet remove package PackageName
```

### 2. Version Pinning
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```
- **Exact versions**: Để tránh breaking changes
- **Version ranges**: Có thể dùng cho patch updates
- **Dependency review**: Regular updates để fix security issues

### 3. Dependency Conflicts
- **Resolution**: MSBuild tự động resolve conflicts
- **Binding redirects**: Auto-generated nếu cần
- **Diagnostics**: `dotnet restore --verbosity detailed`

## Known Issues & Workarounds

### 1. WebView2 Runtime
- **Issue**: WebView2 not installed trên older Windows
- **Workaround**: Bundle WebView2 runtime với installer
- **Detection**: Check registry hoặc try-catch initialization

### 2. PostgreSQL Connection
- **Issue**: Connection string configuration
- **Workaround**: Use environment variables
- **Validation**: Test connection on startup

### 3. Port Conflicts
- **Issue**: Port 5000/5243 already in use
- **Workaround**: Dynamic port assignment
- **Configuration**: Use `appsettings.json` để config port

## Upgrade Guidelines

### 1. .NET Version Upgrades
1. Update TargetFramework trong .csproj files
2. Update NuGet packages đến versions tương thích
3. Test breaking changes
4. Update deployment scripts

### 2. Package Updates
```bash
# Update all packages
dotnet outdated
dotnet update
```

### 3. Database Migrations
1. Create new migration
2. Test migration trên staging
3. Backup production database
4. Apply migration

## License Considerations

### 1. Open Source Licenses
- **MIT License**: Most ASP.NET Core components
- **Apache 2.0**: Some dependencies
- **PostgreSQL License**: Open source

### 2. Commercial Considerations
- **Development**: Free với Community Edition
- **Production**: No licensing costs cho open source stack
- **Support**: Community support hoặc commercial support options

## Performance Implications

### 1. Memory Usage
- **Kestrel**: ~50MB baseline
- **Entity Framework**: Connection pooling
- **Caching**: MemoryCache impact

### 2. Startup Time
- **JIT Compilation**: First-run slower
- **Warm-up**: Subsequent requests faster
- **Optimization**: ReadyToRun compilation

### 3. Disk Space
- **Application**: ~100MB (bao gồm runtime)
- **Database**: Depends on data volume
- **Logs**: Rotating log files

---
*Document version: 1.0 - Last updated: 2025-12-23*
