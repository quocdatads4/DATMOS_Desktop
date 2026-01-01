# Kiến trúc Menu động tập trung DATMOS

## Tổng quan
Hệ thống menu động sử dụng một bảng dữ liệu duy nhất với Entity Framework, hỗ trợ 4 loại menu: Admin, Customer, Teacher, LandingPage. Tất cả menu được quản lý tập trung qua một service chung và hiển thị thông qua ViewComponent duy nhất.

## Mục tiêu
1. **Tập trung hóa**: Một bảng `Menu` duy nhất thay thế cho nhiều bảng/cấu trúc riêng biệt
2. **Linh hoạt**: Phân loại menu bằng trường `MenuType` (Admin, Customer, Teacher, Landing)
3. **Dễ bảo trì**: Thay đổi menu không cần deploy code, chỉ cần update database
4. **Hiệu suất**: Caching và query tối ưu cho từng loại menu
5. **Tái sử dụng**: Cùng codebase cho tất cả Areas

## Kiến trúc hệ thống

### 1. Entity Design
```csharp
// DATMOS.Core/Entities/Menu.cs
public class Menu
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Text { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; set; } = string.Empty;

    [StringLength(50)]
    public string Area { get; set; } = string.Empty;

    [StringLength(50)]
    public string Controller { get; set; } = string.Empty;

    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    [StringLength(200)]
    public string Url { get; set; } = string.Empty;

    public int Order { get; set; } = 0;
    
    public bool IsVisible { get; set; } = true;

    // Phân loại menu: Admin, Customer, Teacher, Landing
    [Required]
    [StringLength(20)]
    public string MenuType { get; set; } = "Customer";

    // Phân cấp menu
    [StringLength(50)]
    public string? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public Menu? Parent { get; set; }

    public ICollection<Menu> Children { get; set; } = new List<Menu>();
}
```

### 2. Cấu trúc thư mục
```
DATMOS.Web/
├── Services/
│   └── Navigation/
│       ├── IMenuService.cs              # Interface chung
│       └── MenuService.cs               # Implementation chung
├── ViewComponents/
│   └── MenuViewComponent.cs             # ViewComponent duy nhất
├── Models/
│   └── ViewModels/
│       └── Navigation/
│           ├── MenuItemViewModel.cs     # ViewModel cho menu item
│           └── MenuDataViewModel.cs     # ViewModel cho menu data
└── Views/
    └── Shared/
        └── Components/
            └── Menu/
                ├── Default.cshtml       # View mặc định
                └── RenderMenu.cshtml    # Partial render đệ quy
```

### 3. Service Layer
**IMenuService.cs** - Interface chung:
```csharp
public interface IMenuService
{
    Task<List<MenuItemViewModel>> GetMenuAsync(string menuType, string? area = null);
    Task<MenuItemViewModel?> GetMenuItemByIdAsync(string id);
    Task<List<MenuItemViewModel>> GetActiveMenuAsync(string menuType, string? area = null);
    Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null);
    Task ClearCacheAsync(string menuType);
}
```

**MenuService.cs** - Implementation:
- Lấy menu từ database theo `MenuType` và `Area`
- Hỗ trợ caching với MemoryCache
- Xử lý phân cấp menu đệ quy
- Logic active state dựa trên RouteData

### 4. ViewComponent
**MenuViewComponent.cs**:
```csharp
public class MenuViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;
    
    public MenuViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(string menuType, string? area = null)
    {
        var menuData = await _menuService.GetMenuDataAsync(menuType, area);
        return View(menuData);
    }
}
```

### 5. View Templates
**Default.cshtml** - Main view:
```html
@model MenuDataViewModel
@if (Model?.MenuItems != null && Model.MenuItems.Any())
{
    <aside id="layout-menu" class="layout-menu-horizontal menu-horizontal menu flex-grow-0">
        <div class="container-xxl d-flex h-100">
            <ul class="menu-inner">
                @await Html.PartialAsync("Components/Menu/RenderMenu", Model.MenuItems)
            </ul>
        </div>
    </aside>
}
```

**RenderMenu.cshtml** - Recursive partial:
```html
@model List<MenuItemViewModel>
@foreach (var item in Model.OrderBy(m => m.Order))
{
    @if (!item.IsVisible) continue;
    
    var hasChildren = item.Children != null && item.Children.Any();
    var isActive = IsMenuItemActive(item.Controller, item.Action) || 
                   (hasChildren && IsParentMenuItemActive(item.Children));
    
    <li class="menu-item @(isActive ? "active" : "")">
        <!-- Render menu item với logic link -->
    </li>
}
```

## Trạng thái hiện tại

### Đã hoàn thành:
1. ✅ **ViewComponent**: `MenuViewComponent.cs` đã được tạo
2. ✅ **View Templates**: `Default.cshtml` và `RenderMenu.cshtml` đã được tạo
3. ✅ **Layout Updates**: 
   - `_AdminLayout.cshtml` đã cập nhật để sử dụng `@await Component.InvokeAsync("Menu", new { area = "Admin" })`
   - `_CustomerLayout.cshtml` đã cập nhật để sử dụng `@await Component.InvokeAsync("Menu", new { area = "Customer" })`
   - `_Layout.cshtml` đã cập nhật để sử dụng `@await Component.InvokeAsync("Menu")`
4. ✅ **Xóa file cũ**: 
   - `_AdminMenuItems.cshtml` đã xóa
   - `_CustomerMenuItems.cshtml` đã xóa
5. ✅ **Build & Migration**: 
   - Build thành công
   - Database migration chạy thành công

### Cần triển khai tiếp:
1. **Service Layer**: Cần tạo `IMenuService` và `MenuService` với caching
2. **ViewModels**: Cần tạo `MenuItemViewModel` và `MenuDataViewModel`
3. **Database**: Cần cập nhật Entity `Menu` với trường `MenuType`
4. **Seed Data**: Cần tạo seed data cho các loại menu

## File đã tạo:
1. `DATMOS.Web/ViewComponents/MenuViewComponent.cs`
2. `DATMOS.Web/Views/Shared/Components/Menu/Default.cshtml`
3. `DATMOS.Web/Views/Shared/Components/Menu/RenderMenu.cshtml`

## File đã cập nhật:
1. `DATMOS.Web/Areas/Admin/Views/Shared/_AdminLayout.cshtml`
2. `DATMOS.Web/Areas/Customer/Views/Shared/_CustomerLayout.cshtml`
3. `DATMOS.Web/Views/Shared/_Layout.cshtml`

## File đã xóa:
1. `DATMOS.Web/Areas/Admin/Views/Shared/Sections/Menu/_AdminMenuItems.cshtml`
2. `DATMOS.Web/Areas/Customer/Views/Shared/Sections/Menu/_CustomerMenuItems.cshtml`

---
*Document version: 2.0 - Last updated: 2025-12-23*
*Kiến trúc menu động đã được triển khai phần ViewComponent. Cần tiếp tục triển khai Service Layer và Database updates.*
