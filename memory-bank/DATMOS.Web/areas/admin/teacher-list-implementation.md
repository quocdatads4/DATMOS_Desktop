# TeacherList Implementation Plan - Admin Area

## 1. Tổng quan
Tài liệu này mô tả chi tiết việc triển khai trang **Danh sách Giáo viên** trong khu vực Admin của DATMOS.Web.

**Mục tiêu:** Tạo trang quản lý tập trung cho giáo viên, chỉ dành cho Admin role.

## 2. Yêu cầu chức năng
- [x] Chỉ Admin role mới truy cập được
- [x] Thiết kế giống hệt `Users/Index.cshtml`
- [x] Chỉ hiển thị người dùng có role "Teacher"
- [x] URL: `/Admin/TeacherList`
- [x] Controller: `TeacherListController` (trong Area Admin)
- [x] View: `Areas/Admin/Views/TeacherList/Index.cshtml`

## 3. Kiến trúc

### 3.1. Cấu trúc file
```
DATMOS.Web/Areas/Admin/
├── Controllers/
│   └── TeacherListController.cs (MỚI)
├── Views/
│   └── TeacherList/ (MỚI)
│       └── Index.cshtml (copy từ Users/Index.cshtml)
```

### 3.2. Luồng dữ liệu
1. Admin truy cập `/Admin/TeacherList`
2. `TeacherListController.Index()` được gọi
3. Controller gọi `UserManager.GetUsersInRoleAsync("Teacher")`
4. Chuyển đổi sang `AdminUserViewModel`
5. Trả về view `Index.cshtml` với danh sách giáo viên

## 4. Implementation Steps

### Step 1: Tạo Controller
**File:** `Areas/Admin/Controllers/TeacherListController.cs`
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

### Step 2: Tạo View
**File:** `Areas/Admin/Views/TeacherList/Index.cshtml`
- Copy toàn bộ nội dung từ `Users/Index.cshtml`
- Chỉnh sửa:
  - `ViewData["Title"] = "Danh sách Giáo viên"`
  - Tiêu đề: "DANH SÁCH GIÁO VIÊN"
  - Mô tả: "Quản lý tất cả giáo viên trong hệ thống"
  - Cập nhật thống kê chỉ tính trên danh sách giáo viên

### Step 3: Cập nhật Navigation
Thêm menu item "Giáo viên" trong Admin sidebar:
```html
<li class="menu-item">
    <a href="@Url.Action("Index", "TeacherList", new { area = "Admin" })" class="menu-link">
        <i class="ti ti-users"></i>
        <span>Giáo viên</span>
    </a>
</li>
```

### Step 4: Cập nhật Courses Create/Edit
**File:** `Areas/Admin/Controllers/CoursesController.cs`
```csharp
// Trong Create() và Edit() actions:
var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
ViewBag.Teachers = teachers.Select(t => new SelectListItem 
{ 
    Value = t.UserName, 
    Text = $"{t.UserName} ({t.Email})" 
}).ToList();
```

**File:** `Areas/Admin/Views/Courses/Create.cshtml` và `Edit.cshtml`:
```razor
@Html.DropDownListFor(m => m.Instructor, ViewBag.Teachers as List<SelectListItem>, "Chọn giáo viên...", new { @class = "form-control" })
```

## 5. Testing Checklist
- [ ] Admin có thể truy cập `/Admin/TeacherList`
- [ ] Non-Admin không thể truy cập (redirect/forbidden)
- [ ] Chỉ hiển thị người dùng có role "Teacher"
- [ ] Giao diện giống hệt `Users/Index.cshtml`
- [ ] Thống kê hiển thị đúng
- [ ] Dropdown giáo viên hoạt động trong Courses Create/Edit
- [ ] Navigation menu hoạt động

## 6. Liên kết liên quan
- [Teacher Area Implementation](../teacher-implementation-plan.md)
- [Admin Controllers](./controllers.md)

---

**Created:** 2025-12-25  
**Status:** Đang triển khai  
**Priority:** High  
**Owner:** Admin Team
