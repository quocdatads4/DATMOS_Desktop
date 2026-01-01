# Admin Area - Controllers

## Tổng quan
Admin Area controllers xử lý tất cả các request liên quan đến quản trị hệ thống. Các controller này được đặt trong `Areas/Admin/Controllers/` và sử dụng `_AdminLayout.cshtml`.

## Danh sách Controllers

### 1. HomeController (`Areas/Admin/Controllers/HomeController.cs`)
**Mục đích**: Xử lý trang chủ và dashboard của admin area.

**Actions**:
- `Index()` - Hiển thị admin dashboard
- `Error()` - Xử lý lỗi (nếu có)

**Routes**:
- `GET /Admin` → Redirect to `/Admin/Home`
- `GET /Admin/Home` → Home/Index
- `GET /Admin/Home/Index` → Dashboard view

**View**: `Areas/Admin/Views/Home/Index.cshtml`

### 2. CoursesController (`Areas/Admin/Controllers/CoursesController.cs`)
**Mục đích**: Quản lý khóa học trong hệ thống.

**Actions**:
- `Index()` - Danh sách khóa học
- `Create()` - Tạo khóa học mới
- `Edit(int id)` - Chỉnh sửa khóa học
- `Delete(int id)` - Xóa khóa học
- `Details(int id)` - Chi tiết khóa học

**Routes**:
- `GET /Admin/Courses` → Courses/Index
- `GET /Admin/Courses/Create` → Form tạo mới
- `POST /Admin/Courses/Create` → Xử lý tạo mới
- `GET /Admin/Courses/Edit/{id}` → Form chỉnh sửa
- `POST /Admin/Courses/Edit/{id}` → Xử lý chỉnh sửa
- `GET /Admin/Courses/Delete/{id}` → Xác nhận xóa
- `POST /Admin/Courses/Delete/{id}` → Xử lý xóa
- `GET /Admin/Courses/Details/{id}` → Chi tiết khóa học

**View**: `Areas/Admin/Views/Courses/Index.cshtml`

### 3. LessonsController (`Areas/Admin/Controllers/LessonsController.cs`)
**Mục đích**: Quản lý bài học trong các khóa học.

**Actions**:
- `Index()` - Danh sách bài học
- `Create()` - Tạo bài học mới
- `Edit(int id)` - Chỉnh sửa bài học
- `Delete(int id)` - Xóa bài học

**Routes**:
- `GET /Admin/Lessons` → Lessons/Index
- `GET /Admin/Lessons/Create` → Form tạo mới
- `POST /Admin/Lessons/Create` → Xử lý tạo mới
- `GET /Admin/Lessons/Edit/{id}` → Form chỉnh sửa
- `POST /Admin/Lessons/Edit/{id}` → Xử lý chỉnh sửa
- `GET /Admin/Lessons/Delete/{id}` → Xác nhận xóa
- `POST /Admin/Lessons/Delete/{id}` → Xử lý xóa

**Views**:
- `Areas/Admin/Views/Lessons/Index.cshtml` - Danh sách
- `Areas/Admin/Views/Lessons/_CreateEditModal.cshtml` - Modal form

### 4. UsersController (`Areas/Admin/Controllers/UsersController.cs`)
**Mục đích**: Quản lý người dùng hệ thống.

**Actions**:
- `Index()` - Danh sách người dùng
- `Create()` - Tạo người dùng mới
- `Edit(string id)` - Chỉnh sửa người dùng
- `Delete(string id)` - Xóa người dùng
- `Details(string id)` - Chi tiết người dùng

**Routes**:
- `GET /Admin/Users` → Users/Index
- `GET /Admin/Users/Create` → Form tạo mới
- `POST /Admin/Users/Create` → Xử lý tạo mới
- `GET /Admin/Users/Edit/{id}` → Form chỉnh sửa
- `POST /Admin/Users/Edit/{id}` → Xử lý chỉnh sửa
- `GET /Admin/Users/Delete/{id}` → Xác nhận xóa
- `POST /Admin/Users/Delete/{id}` → Xử lý xóa
- `GET /Admin/Users/Details/{id}` → Chi tiết người dùng

**View**: `Areas/Admin/Views/Users/Index.cshtml`

### 5. AdminProductController (`Areas/Admin/Controllers/AdminProductController.cs`)
**Mục đích**: Quản lý sản phẩm (có thể là sách, tài liệu, v.v.)

**Actions**:
- `Index()` - Danh sách sản phẩm
- `Create()` - Tạo sản phẩm mới
- `Edit(int id)` - Chỉnh sửa sản phẩm
- `Delete(int id)` - Xóa sản phẩm

**Routes**:
- `GET /Admin/AdminProduct` → AdminProduct/Index
- `GET /Admin/AdminProduct/Create` → Form tạo mới
- `POST /Admin/AdminProduct/Create` → Xử lý tạo mới
- `GET /Admin/AdminProduct/Edit/{id}` → Form chỉnh sửa
- `POST /Admin/AdminProduct/Edit/{id}` → Xử lý chỉnh sửa
- `GET /Admin/AdminProduct/Delete/{id}` → Xác nhận xóa
- `POST /Admin/AdminProduct/Delete/{id}` → Xử lý xóa

**View**: `Areas/Admin/Views/Product/` (cần kiểm tra)

## Dependency Injection

Các controllers sử dụng các services thông qua constructor injection:

```csharp
public class CoursesController : Controller
{
    private readonly ICoursesService _coursesService;
    
    public CoursesController(ICoursesService coursesService)
    {
        _coursesService = coursesService;
    }
    
    // Actions...
}
```

**Services được inject**:
- `ICoursesService` - Cho CoursesController
- `ILessonService` - Cho LessonsController
- `IMenuService` - Cho tất cả controllers (qua layout)

## Authorization & Security

### 1. Area-based Authorization
- Tất cả controllers trong Admin Area yêu cầu admin privileges
- Route: `/Admin/*` chỉ accessible bởi admin users

### 2. Input Validation
- Model validation với DataAnnotations
- Anti-forgery tokens cho POST requests
- Parameter sanitization

### 3. Error Handling
- Try-catch blocks trong actions
- Logging errors
- User-friendly error messages

## View Models

Các controllers sử dụng ViewModels để truyền data đến views:

### 1. UserViewModel
```csharp
public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    // ... other properties
}
```

### 2. CourseViewModel
- Chứa thông tin khóa học
- Danh sách bài học
- Thống kê người học

### 3. LessonViewModel
- Chi tiết bài học
- Nội dung, tài liệu đính kèm
- Thứ tự trong khóa học

## Routing Configuration

### 1. Area Route Registration
```csharp
endpoints.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
```

### 2. Admin-specific Route
```csharp
endpoints.MapControllerRoute(
    name: "adminRoot",
    pattern: "Admin",
    defaults: new { area = "Admin", controller = "Home", action = "Index" });
```

## Best Practices

### 1. Code Organization
- Mỗi controller tập trung vào một resource
- Actions được nhóm theo CRUD operations
- Shared logic được đưa vào services

### 2. Performance
- Async actions cho I/O operations
- Caching dữ liệu thường xuyên đọc
- Pagination cho danh sách lớn

### 3. Maintainability
- Consistent naming conventions
- XML documentation comments
- Separation of concerns

## Testing Guidelines

### 1. Unit Tests
- Test business logic trong services
- Test controller actions với mock services
- Test model validation

### 2. Integration Tests
- Test routing đến admin area
- Test authentication/authorization
- Test database operations

### 3. UI Tests
- Test form submissions
- Test navigation flows
- Test responsive design

## Common Patterns

### 1. CRUD Pattern
```csharp
// Index - List
// Create - GET/POST
// Edit - GET/POST  
// Delete - GET/POST
// Details - GET
```

### 2. Service Pattern
```csharp
public async Task<IActionResult> Index()
{
    var courses = await _coursesService.GetAllAsync();
    return View(courses);
}
```

### 3. ViewModel Pattern
```csharp
public IActionResult Create()
{
    var viewModel = new CourseViewModel();
    return View(viewModel);
}
```

## Future Enhancements

### 1. API Endpoints
- REST API cho mobile/app integration
- Web API controllers
- Swagger/OpenAPI documentation

### 2. Advanced Features
- Bulk operations
- Import/export functionality
- Advanced search và filtering

### 3. Monitoring
- Action performance tracking
- User activity logging
- Audit trails

---
*Document version: 1.0 - Last updated: 2025-12-23*
