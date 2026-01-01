# DATMOS.Web - Tổng quan ứng dụng Web

## Giới thiệu
DATMOS.Web là ứng dụng web ASP.NET Core MVC được thiết kế để chạy trong môi trường hybrid desktop application. Ứng dụng này cung cấp giao diện người dùng hiện đại, responsive và được host bên trong WinForms application thông qua WebView2.

## Kiến trúc tổng quan
```
DATMOS.Web/
├── Areas/                          # Phân chia theo module chức năng
│   ├── Admin/                      # Khu vực quản trị
│   └── Customer/                   # Khu vực người dùng
├── Controllers/                    # Controller gốc
├── Services/                       # Business logic services
├── Models/                         # Models và ViewModels
├── Views/                          # Views và layouts
├── Pages/                          # Razor Pages
├── wwwroot/                        # Static assets
└── Program.cs + WebEntryPoint.cs   # Entry points
```

## Công nghệ sử dụng
- **Framework**: ASP.NET Core MVC 8.0
- **UI Framework**: Bootstrap 5, jQuery, Font Awesome
- **Database**: PostgreSQL với Entity Framework Core
- **Authentication**: ASP.NET Core Identity (có thể mở rộng)
- **Caching**: MemoryCache cho menu và dữ liệu thường xuyên truy cập
- **JSON Serialization**: Newtonsoft.Json

## Cấu hình chính
### Ports
- **Development**: Port 5243 (theo launchSettings.json)
- **Production**: Port 5000 (theo WebEntryPoint.cs)

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=datmos_db;Username=postgres;Password=password"
  }
}
```

### Dependency Injection
Các services được đăng ký trong `WebEntryPoint.cs`:
- `AppDbContext` - Database context
- `IMenuService` - Menu navigation service
- `ICoursesService` - Quản lý khóa học
- `IExamSubjectService` - Quản lý môn thi
- `ILessonService` - Quản lý bài học

## Routing Configuration
```csharp
// Area routing
endpoints.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default routing
endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

## Areas Structure

### 1. Admin Area
- **Mục đích**: Quản lý hệ thống, người dùng, khóa học
- **Controllers**: HomeController, CoursesController, UsersController, LessonsController
- **Layout**: `_AdminLayout.cshtml` với sidebar navigation
- **Features**: Dashboard, CRUD operations, user management

### 2. Customer Area
- **Mục đích**: Giao diện người dùng, học tập, thực hành
- **Controllers**: HomeController, CoursesController, ProductController, LessonController
- **Layout**: `_CustomerLayout.cshtml` với horizontal menu
- **Features**: Course browsing, practice tests, exam simulation

## Services Layer

### Menu Service
- **Interface**: `IMenuService`
- **Implementation**: `MenuService`
- **Chức năng**: Quản lý menu động với caching, hỗ trợ 4 loại menu (Admin, Customer, Teacher, Landing)

### Course Service
- **Interface**: `ICoursesService`
- **Implementation**: `CoursesService`
- **Chức năng**: Quản lý khóa học, bài học, progress tracking

### Exam Service
- **Interface**: `IExamSubjectService`
- **Implementation**: `ExamSubjectService`
- **Chức năng**: Quản lý đề thi, câu hỏi, kết quả

## Static Assets (wwwroot)

### CSS Structure
- `site.css` - Custom styles
- `demo.css` - Theme demo styles
- Bootstrap 5 + custom theming

### JavaScript Structure
- `site.js` - Global scripts
- `main.js` - Application entry point
- Area-specific scripts (app-academy-*.js, app-ecommerce-*.js)

### Images và Media
- `img/` - Hình ảnh, icons, illustrations
- `audio/` - Audio files cho multimedia content
- `svg/` - SVG graphics

## Development Workflow

### 1. Local Development
```bash
dotnet run --project DATMOS.Web
# Hoặc chạy qua DATMOS.WinApp để test integration
```

### 2. Database Migrations
```bash
dotnet ef migrations add [MigrationName] --project DATMOS.Data
dotnet ef database update --project DATMOS.Data
```

### 3. Seed Data
```bash
# Chạy MenuSeeder để tạo dữ liệu menu mẫu
dotnet run --project DATMOS.Data
```

## Integration với WinApp
DATMOS.Web được host bên trong DATMOS.WinApp thông qua:
1. **Kestrel Web Server** chạy trên background thread
2. **WebView2 Control** hiển thị giao diện web
3. **Shared Database** sử dụng cùng AppDbContext

## Performance Considerations
1. **Caching**: Menu data được cache để giảm database calls
2. **Bundling & Minification**: Static assets được optimized
3. **Lazy Loading**: Images và heavy resources load on demand
4. **Database Indexing**: Optimized indexes cho frequent queries

## Security Features
1. **Input Validation**: Model validation với DataAnnotations
2. **XSS Protection**: Auto-escaping trong Razor views
3. **CSRF Protection**: Anti-forgery tokens
4. **Secure Headers**: HSTS, X-Content-Type-Options

## Monitoring & Logging
1. **Application Logs**: Serilog integration (có thể cấu hình)
2. **Error Handling**: Global exception handler
3. **Performance Metrics**: Response time monitoring

## Deployment
Ứng dụng được deploy như một phần của DATMOS_Desktop solution:
1. Build tất cả projects
2. Copy output đến thư mục bin
3. Chạy DATMOS.WinApp.exe

---
*Document version: 1.0 - Last updated: 2025-12-23*
