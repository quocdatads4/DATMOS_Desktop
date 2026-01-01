# Workspace Rules for DATMOS_Desktop

## 1. Tổng Quan
Tài liệu này định nghĩa các quy tắc workspace cho dự án DATMOS_Desktop - một giải pháp Hybrid Desktop Application kết hợp WinForms và ASP.NET Core MVC. Các quy tắc này áp dụng cho tất cả các thành viên trong dự án và được thiết kế để đảm bảo tính nhất quán, khả năng bảo trì và chất lượng code.

## 2. Cấu Trúc Dự Án

### 2.1. Solution Structure
```
DATMOS_Desktop/
├── DATMOS.Core/          # Class Library - Core entities và interfaces
├── DATMOS.Data/          # Class Library - Data access và EF Core
├── DATMOS.Web/           # ASP.NET Core MVC Library - Web application
└── DATMOS.WinApp/        # WinForms Application - Desktop host
```

### 2.2. Quy Tắc Tổ Chức Thư Mục
- **DATMOS.Core/Entities/**: Chứa tất cả business entities (PascalCase, singular)
- **DATMOS.Core/Interfaces/**: Chứa interfaces (bắt đầu với chữ I)
- **DATMOS.Data/Migrations/**: EF Core migrations (tự động sinh)
- **DATMOS.Web/Areas/**: Tổ chức theo areas (Admin, Customer, Teacher, User)
- **DATMOS.Web/Views/**: Tổ chức theo controller/area
- **DATMOS.Web/wwwroot/**: Static files (css, js, img, lib)

## 3. Coding Conventions (C#)

### 3.1. Đặt Tên (Naming Conventions)
- **Classes**: PascalCase (ví dụ: `Product`, `HomeController`)
- **Interfaces**: Bắt đầu với chữ I + PascalCase (ví dụ: `IProductService`)
- **Methods**: PascalCase (ví dụ: `GetProductById()`)
- **Parameters/Local Variables**: camelCase (ví dụ: `productId`, `userName`)
- **Private Fields**: `_camelCase` (ví dụ: `_dbContext`)
- **Constants**: UPPER_CASE_WITH_UNDERSCORES (ví dụ: `MAX_RETRY_COUNT`)
- **Namespaces**: `DATMOS.{Project}.{SubNamespace}` (ví dụ: `DATMOS.Core.Entities`)

### 3.2. Formatting
- **Indentation**: 4 spaces (không dùng tabs)
- **Braces**: Allman style (dấu ngoặc nhọn trên dòng mới)
- **Line Length**: Tối đa 120 ký tự
- **Using Statements**: Sắp xếp theo alphabetical order, nhóm System.* trước
- **Blank Lines**: 1 dòng trống giữa methods, 2 dòng trống giữa class definitions

### 3.3. Code Style
```csharp
// Good
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    
    public ProductService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

// Avoid
public class productService : IproductService {
    private AppDbContext context;
    public productService(AppDbContext c) { context = c; }
    public async Task<Product> get(int id) {
        return await context.Products.FirstOrDefaultAsync(p=>p.Id==id);
    }
}
```

## 4. ASP.NET Core MVC Conventions

### 4.1. Controllers
- **Tên Controller**: {Resource}Controller (ví dụ: `ProductsController`)
- **Base Class**: Kế thừa từ `Controller` hoặc `ControllerBase`
- **Action Methods**: Trả về `IActionResult` hoặc `Task<IActionResult>`
- **Routing**: Sử dụng attribute routing cho APIs, convention routing cho MVC

### 4.2. Views
- **Thư Mục Views**: `Views/{ControllerName}/{ActionName}.cshtml`
- **Partial Views**: Bắt đầu với `_` (ví dụ: `_ProductForm.cshtml`)
- **Layout Files**: Đặt trong `Views/Shared/` (ví dụ: `_Layout.cshtml`)
- **View Components**: `Views/Shared/Components/{ComponentName}/Default.cshtml`

### 4.3. Areas Structure
```
Areas/
├── Admin/
│   ├── Controllers/
│   ├── Views/
│   └── ViewModels/
├── Customer/
├── Teacher/
└── User/
```

## 5. Entity Framework Core Conventions

### 5.1. Entity Classes
- **Primary Key**: Thuộc tính `Id` (int, Guid, hoặc custom)
- **Navigation Properties**: Virtual cho lazy loading (nếu cần)
- **Timestamps**: `CreatedAt`, `UpdatedAt` cho audit tracking
- **Data Annotations**: Sử dụng cho validation, không dùng cho schema

### 5.2. DbContext
- **DbSet Properties**: PascalCase, plural (ví dụ: `public DbSet<Product> Products`)
- **Configuration**: Sử dụng Fluent API trong `OnModelCreating`
- **Migrations**: Tạo với descriptive names (ví dụ: `AddProductTable`)

## 6. ASP.NET Core Identity Naming Conventions

### 6.1. Identity Entities
- **Custom User Class**: `ApplicationUser` (kế thừa từ `IdentityUser`)
- **Custom Role Class**: `ApplicationRole` (kế thừa từ `IdentityRole`)
- **User Properties**: PascalCase, descriptive names (ví dụ: `FirstName`, `LastName`, `ProfilePictureUrl`)
- **Extended Properties**: Thêm vào `ApplicationUser` class, không sửa trực tiếp `IdentityUser`

### 6.2. Identity DbContext
- **DbContext Name**: `AppDbContext` (kế thừa từ `IdentityDbContext<ApplicationUser, ApplicationRole>`)
- **Generic Parameters**: `IdentityDbContext<ApplicationUser, ApplicationRole, string>` (string cho key type)
- **Configuration**: Sử dụng Fluent API trong `OnModelCreating` để configure identity tables

### 6.3. Database Table Names
- **Default Tables**: Giữ nguyên tên mặc định của ASP.NET Core Identity:
  - `AspNetUsers` (cho users)
  - `AspNetRoles` (cho roles)
  - `AspNetUserRoles` (cho user-role relationships)
  - `AspNetUserClaims`, `AspNetRoleClaims` (cho claims)
  - `AspNetUserLogins`, `AspNetUserTokens` (cho external logins)
- **Custom Tables**: Thêm prefix `AspNet` cho consistency (ví dụ: `AspNetUserProfiles`)

### 6.4. Column Naming Conventions
- **Identity Columns**: Giữ nguyên tên mặc định (ví dụ: `Id`, `UserName`, `Email`, `PasswordHash`)
- **Extended Columns**: Sử dụng PascalCase (ví dụ: `FirstName`, `LastName`, `CreatedAt`)
- **Foreign Keys**: `{TableName}Id` (ví dụ: `UserId`, `RoleId`)
- **Timestamps**: `CreatedAt`, `UpdatedAt`, `LastLoginAt`

### 6.5. Identity Services và Repositories
- **User Manager**: `UserManager<ApplicationUser>` (sử dụng built-in)
- **Role Manager**: `RoleManager<ApplicationRole>` (sử dụng built-in)
- **SignIn Manager**: `SignInManager<ApplicationUser>` (sử dụng built-in)
- **Custom Services**: `I{Feature}Service` (ví dụ: `IUserProfileService`, `IRoleService`)

### 6.6. Identity Controllers và Views
- **Account Controller**: `AccountController` trong Areas/Identity
- **Manage Controller**: `ManageController` cho user profile management
- **View Names**: Standard action names (`Login`, `Register`, `ForgotPassword`, `ResetPassword`)
- **View Models**: `{Action}ViewModel` (ví dụ: `LoginViewModel`, `RegisterViewModel`)

### 6.7. Area Structure cho Identity
```
Areas/Identity/
├── Controllers/
│   ├── AccountController.cs
│   └── ManageController.cs
├── Models/
│   ├── ApplicationUser.cs
│   └── ApplicationRole.cs
├── Services/
│   ├── IUserProfileService.cs
│   └── UserProfileService.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
└── Views/
    ├── Account/
    │   ├── Login.cshtml
    │   ├── Register.cshtml
    │   └── ForgotPassword.cshtml
    └── Manage/
        ├── Index.cshtml
        └── ChangePassword.cshtml
```

### 6.8. Migration Naming cho Identity
- **Initial Identity**: `AddIdentityTables`
- **Extended Properties**: `AddUserProfileFields`
- **Custom Tables**: `Add{TableName}Table` (ví dụ: `AddUserProfileTable`)
- **Relationship Updates**: `Update{Entity}Relationships` (ví dụ: `UpdateUserRoleRelationships`)

### 6.9. Code Examples
```csharp
// ApplicationUser với extended properties
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;
    
    [PersonalData]
    public string LastName { get; set; } = string.Empty;
    
    [PersonalData]
    public string ProfilePictureUrl { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// AppDbContext với Identity configuration
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Custom configuration
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });
    }
}

// Identity service registration
services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

## 7. Frontend Conventions (Razor/HTML/CSS/JS)

### 7.1. Razor Views
- **Model Directive**: `@model Namespace.ViewModel` trên dòng đầu tiên
- **Using Directives**: Nhóm lại ở đầu file
- **HTML Structure**: Semantic HTML5 với proper indentation
- **Bootstrap**: Sử dụng Bootstrap 5 với utility classes

### 7.2. JavaScript
- **File Location**: `wwwroot/js/` hoặc `wwwroot/areas/{area}/js/`
- **Module Pattern**: Sử dụng ES6 modules khi có thể
- **Event Handling**: Sử dụng event delegation khi có thể

### 7.3. CSS/Styling
- **BEM Methodology**: Sử dụng khi viết custom CSS
- **Custom Properties**: Sử dụng CSS variables cho theming
- **Responsive Design**: Mobile-first approach

## 8. Git Workflow

### 8.1. Branch Strategy
- **main**: Production-ready code
- **develop**: Integration branch
- **feature/**: New features (ví dụ: `feature/user-authentication`)
- **bugfix/**: Bug fixes (ví dụ: `bugfix/login-issue`)
- **hotfix/**: Critical production fixes

### 8.2. Commit Messages
Format: `type(scope): description`

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

Example: `feat(auth): add user login functionality`

### 8.3. Pull Requests
- **Title**: Descriptive và concise
- **Description**: Mô tả changes, link đến issue
- **Review**: Tối thiểu 1 reviewer trước khi merge
- **Tests**: Đảm bảo tất cả tests pass

## 9. Testing Conventions

### 9.1. Unit Tests
- **Project Structure**: `{Project}.Tests` (ví dụ: `DATMOS.Core.Tests`)
- **Naming**: `{ClassUnderTest}Tests` (ví dụ: `ProductServiceTests`)
- **Method Naming**: `{MethodUnderTest}_{Scenario}_{ExpectedResult}`

### 9.2. Integration Tests
- **Database**: Sử dụng in-memory database hoặc test database
- **Test Data**: Sử dụng test data builders hoặc factories
- **Cleanup**: Đảm bảo cleanup sau mỗi test

## 10. Performance Guidelines

### 10.1. Database
- **Queries**: Sử dụng `.AsNoTracking()` cho read-only operations
- **Eager Loading**: Sử dụng `.Include()` khi cần related data
- **Pagination**: Luôn paginate large datasets

### 10.2. Web
- **Bundling/Minification**: Sử dụng cho production
- **Caching**: Implement caching cho static content và frequent queries
- **Async/Await**: Sử dụng cho I/O-bound operations

## 11. Security Guidelines

### 11.1. Authentication/Authorization
- **ASP.NET Core Identity**: Sử dụng cho user management
- **Role-based Access**: Implement role checks trong controllers
- **JWT Tokens**: Sử dụng cho API authentication

### 11.2. Data Protection
- **SQL Injection**: Sử dụng parameterized queries (EF Core)
- **XSS Protection**: HTML encode user input trong views
- **CSRF Protection**: Sử dụng anti-forgery tokens

## 12. Documentation

### 12.1. Code Documentation
- **XML Comments**: Sử dụng cho public APIs
- **README Files**: Mỗi project có README.md
- **Architecture Decisions**: Document trong `memory-bank/`

### 12.2. API Documentation
- **Swagger/OpenAPI**: Sử dụng cho Web APIs
- **API Versioning**: Implement versioning cho public APIs

## 13. Development Environment

### 13.1. Required Tools
- .NET 8 SDK
- Visual Studio 2022 hoặc VS Code
- PostgreSQL Database
- WebView2 Runtime (cho WinForms)
- Git

### 13.2. Local Setup
1. Clone repository
2. Restore packages: `dotnet restore`
3. Setup database connection string
4. Run migrations: `dotnet ef database update`
5. Build solution: `dotnet build`
6. Run application: `dotnet run --project DATMOS.WinApp`

## 14. Code Review Checklist

### 14.1. Functional Requirements
- [ ] Code implements requirements correctly
- [ ] Edge cases are handled
- [ ] Error handling is appropriate
- [ ] Performance considerations addressed

### 14.2. Code Quality
- [ ] Follows coding conventions
- [ ] No code duplication
- [ ] Proper separation of concerns
- [ ] Unit tests included/updated

### 14.3. Security
- [ ] Input validation implemented
- [ ] Authentication/authorization checks
- [ ] No sensitive data exposure
- [ ] Secure configuration management

## 15. Exception Handling

### 15.1. General Principles
- **Specific Exceptions**: Catch specific exceptions, không dùng `catch (Exception)`
- **Logging**: Log exceptions với context information
- **User Messages**: Friendly error messages cho end users
- **Global Exception Handler**: Sử dụng middleware cho unhandled exceptions

### 15.2. Logging
- **Structured Logging**: Sử dụng Serilog hoặc built-in logging
- **Log Levels**: 
  - `Error`: Application errors
  - `Warning`: Unexpected but recoverable
  - `Information`: General application flow
  - `Debug`: Detailed debugging information

## 16. Maintenance và Refactoring

### 16.1. Technical Debt
- **Regular Reviews**: Code reviews để identify technical debt
- **Refactoring Sprints**: Dành thời gian cho refactoring
- **Documentation Updates**: Cập nhật documentation với code changes

### 16.2. Deprecation Policy
- **Deprecation Notice**: Thông báo trước khi remove features
- **Migration Path**: Cung cấp migration guide
- **Versioning**: Sử dụng semantic versioning

---

*Tài liệu này được cập nhật lần cuối: 27/12/2025*
*Áp dụng cho tất cả các thành viên tham gia phát triển dự án DATMOS_Desktop*
