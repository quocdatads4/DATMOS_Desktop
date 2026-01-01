# Kế hoạch Triển khai Teacher Area với Identity - Phiên bản 2.0

## 1. Tổng quan
Tài liệu này mô tả chi tiết việc triển khai khu vực dành cho Giáo viên (Teacher Area) trong DATMOS.Web và tích hợp với hệ thống quản lý Admin. Hệ thống sử dụng **ASP.NET Core Identity** để xác thực và phân quyền.

**Nguyên tắc cốt lõi:**
1.  **Xác thực:** Giáo viên đăng nhập bằng tài khoản Identity có Role = "Teacher".
2.  **Phân quyền:**
    *   **Admin:** Tạo tài khoản giáo viên, Quản lý danh sách giáo viên (qua `TeacherListController`), Tạo khóa học, Phân công giáo viên vào khóa học.
    *   **Teacher:** Chỉ xem được các khóa học mình được phân công, Xem danh sách học viên, Theo dõi tiến độ. Không được tạo/xóa khóa học.
3.  **Dữ liệu:** Dựa trên Entity `Course` đã có, trường `Instructor` lưu `UserName` của giáo viên.

## 2. Hiện trạng & Phân tích

### 2.1. Đã triển khai:
- **Area Teacher** đã có cấu trúc cơ bản với `CoursesController` và các view
- **Area Admin** đã có `UsersController` và `CoursesController`
- **Identity System** đã tích hợp với UserManager và RoleManager
- **View `Users/Index.cshtml`** đã có thiết kế hoàn chỉnh cho quản lý người dùng

### 2.2. Vấn đề cần giải quyết:
1. Teacher Area chưa có security check và lọc theo giáo viên
2. Chưa có trang danh sách giáo viên trong Admin (`TeacherListController`)
3. Form Create/Edit Course chưa có dropdown chọn giáo viên

## 3. Kiến trúc & Luồng dữ liệu

### 3.1. Identity & Security
- **Area Teacher:** `[Area("Teacher")]`, `[Authorize(Roles = "Teacher")]`
- **Area Admin:** `[Area("Admin")]`, `[Authorize(Roles = "Admin")]`
- **User Mapping:** `Course.Instructor` = `User.UserName` của giáo viên

### 3.2. Service Layer Updates
```csharp
// DATMOS.Web/Services/ICoursesService.cs
Task<List<CourseViewModel>> GetCoursesByInstructorAsync(string instructorName);
Task<CourseViewModel?> GetCourseByInstructorAsync(int courseId, string instructorName);
```

### 3.3. Controller Layer
**Teacher/CoursesController:**
- `Index()`: Lấy `User.Identity.Name` → Gọi `GetCoursesByInstructorAsync`
- `Details(int id)`: Security check + hiển thị dashboard
- `Students(int id)`: Danh sách học viên
- `Analytics(int id)`: Thống kê khóa học

**Admin/TeacherListController (MỚI):**
- `Index()`: Danh sách giáo viên (chỉ role "Teacher")

## 4. Chi tiết triển khai ViewModels

### 4.1. Teacher Area ViewModels
```csharp
public class TeacherCourseViewModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Icon { get; set; }
    public string ColorClass { get; set; }
    public int EnrolledStudents { get; set; }
    public int TotalLessons { get; set; }
    public double Progress { get; set; }
    public int AssignmentCount { get; set; }
    public int CompletedAssignments { get; set; }
    public DateTime LastActivity { get; set; }
    public string Instructor { get; set; }
}
```

### 4.2. Admin Teacher Management
```csharp
public class AdminTeacherViewModel : AdminUserViewModel
{
    public int AssignedCourses { get; set; }
    public int TotalStudents { get; set; }
    public double AverageRating { get; set; }
    public DateTime? LastLogin { get; set; }
}
```

## 5. Kế hoạch thực hiện từng bước

### Phase 1: Hoàn thiện Teacher Area (Backend)
1. **Cập nhật Service Layer:**
   - Thêm `GetCoursesByInstructorAsync` vào `ICoursesService` và `CoursesService`
   - Logic: `_context.Courses.Where(c => c.Instructor == instructorName)`

2. **Cải thiện Teacher/CoursesController:**
   - Thêm `[Authorize(Roles = "Teacher")]`
   - Cập nhật `Index()`: Lọc theo giáo viên đăng nhập
   - Thêm security check trong `Details(int id)`

3. **Tổ chức ViewModel:**
   - Di chuyển ViewModel từ controller sang `Areas/Teacher/ViewModels/`

### Phase 2: Admin - TeacherList (Danh sách Giáo viên)
1. **Tạo `TeacherListController`:**
   - Location: `Areas/Admin/Controllers/TeacherListController.cs`
   - Attribute: `[Area("Admin")]`, `[Authorize(Roles = "Admin")]`
   - Action `Index()`: Lấy danh sách người dùng có role "Teacher"

2. **Tạo View `TeacherList/Index.cshtml`:**
   - Copy từ `Users/Index.cshtml`, chỉnh sửa tiêu đề và thống kê
   - Chỉ hiển thị người dùng có role "Teacher"

3. **Cập nhật Navigation:**
   - Thêm menu "Giáo viên" trong Admin sidebar

### Phase 3: Admin - Phân công Giáo viên vào Khóa học
1. **Cập nhật Admin/CoursesController:**
   - `Create()` và `Edit()`: Truyền `ViewBag.Teachers` từ `userManager.GetUsersInRoleAsync("Teacher")`
   
2. **Cập nhật View `Create.cshtml` và `Edit.cshtml`:**
   - Thay `@Html.TextBoxFor(m => m.Instructor)` bằng `@Html.DropDownListFor(m => m.Instructor, ViewBag.Teachers, "Chọn giáo viên...")`

### Phase 4: Kiểm tra & Hoàn thiện
1. **Kiểm tra Security:** Đảm bảo chỉ đúng role truy cập đúng area
2. **Kiểm tra Data Filtering:** Giáo viên chỉ thấy khóa học của mình
3. **Kiểm tra UI/UX:** Đảm bảo giao diện thân thiện, responsive

## 6. Phần Admin: TeacherList (Danh sách Giáo viên)

### 6.1. Yêu cầu
- Chỉ Admin role mới truy cập được
- Thiết kế giống hệt `Users/Index.cshtml`
- Chỉ hiển thị người dùng có role "Teacher"
- URL: `/Admin/TeacherList`
- Controller: `TeacherListController` (trong Area Admin)
- View: `Areas/Admin/Views/TeacherList/Index.cshtml`

### 6.2. Implementation Details
**Controller `TeacherListController`:**
```csharp
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TeacherListController : Controller
{
    private readonly UserManager<AddUsers> _userManager;
    
    public TeacherListController(UserManager<AddUsers> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
        var teacherViewModels = teachers.Select(t => 
            AdminUserViewModel.FromUser(t, new List<string> { "Teacher" })).ToList();
        return View(teacherViewModels);
    }
}
```

**View `Index.cshtml`:**
- Copy từ `Index.cshtml` trong `Views/Users/`
- Sửa `ViewData["Title"] = "Danh sách Giáo viên"`
- Sửa tiêu đề: "DANH SÁCH GIÁO VIÊN"
- Cập nhật thống kê chỉ tính trên danh sách giáo viên

## 7. Prompt cho Cline (Visual Code)

### Prompt 1: TeacherList Controller & View
> "Tạo trang danh sách giáo viên trong Area Admin với:
> 1. Tạo `TeacherListController.cs` trong `Areas/Admin/Controllers/` với action `Index()` lấy danh sách người dùng có role 'Teacher'
> 2. Tạo thư mục `Views/TeacherList/` và file `Index.cshtml` với thiết kế giống `Users/Index.cshtml`
> 3. Thêm `[Authorize(Roles = "Admin")]` trên controller
> 4. Cập nhật navigation menu để thêm link 'Giáo viên'"

### Prompt 2: Teacher Area Security
> "Cập nhật Teacher Area với:
> 1. Thêm `[Authorize(Roles = "Teacher")]` trên `Teacher/CoursesController`
> 2. Cập nhật action `Index()` để lọc khóa học theo `User.Identity.Name`
> 3. Thêm security check trong `Details(int id)` để đảm bảo giáo viên chỉ xem được khóa học của mình"

### Prompt 3: Courses Dropdown Teachers
> "Cập nhật Admin/CoursesController và views:
> 1. Trong `Create()` và `Edit()`, truyền `ViewBag.Teachers` từ `userManager.GetUsersInRoleAsync("Teacher")`
> 2. Trong `Create.cshtml` và `Edit.cshtml`, thay textbox Instructor bằng dropdown list
> 3. Đảm bảo giá trị được lưu đúng vào `Course.Instructor`"

## 8. Phụ lục: Code Samples

### 8.1. GetCoursesByInstructorAsync Implementation
```csharp
public async Task<List<CourseViewModel>> GetCoursesByInstructorAsync(string instructorName)
{
    var courses = await _context.Courses
        .Where(c => c.Instructor == instructorName)
        .ToListAsync();
    
    return courses.Select(MapToCourseViewModel).ToList();
}
```

### 8.2. Security Check in Details Action
```csharp
public async Task<IActionResult> Details(int id)
{
    var course = await _coursesService.GetCourseByIdAsync(id);
    if (course == null) return NotFound();
    
    // Security check
    if (course.Instructor != User.Identity.Name)
        return Forbid();
    
    // ... rest of the code ...
}
```

---

**Document version: 2.0**  
**Created: 2025-12-24**  
**Last Updated: 2025-12-25**  
**Status: Đang triển khai**  
**Priority: High**
