# Kiến trúc Footer động tập trung DATMOS

## Tổng quan
Hệ thống footer động sử dụng một bảng dữ liệu duy nhất với Entity Framework, hỗ trợ 4 loại footer: Admin, Customer, Teacher, LandingPage. Tất cả footer được quản lý tập trung qua một service chung và hiển thị thông qua ViewComponent duy nhất.

## Mục tiêu
1. **Tập trung hóa**: Một bảng `Footer` duy nhất thay thế cho nhiều file footer riêng biệt
2. **Linh hoạt**: Phân loại footer bằng trường `FooterType` (Admin, Customer, Teacher, Landing)
3. **Dễ bảo trì**: Thay đổi footer không cần deploy code, chỉ cần update database
4. **Hiệu suất**: Caching và query tối ưu cho từng loại footer
5. **Tái sử dụng**: Cùng codebase cho tất cả Areas

## Phân tích hiện tại

### 1. Customer Footer (`CustomerFooterItems.cshtml`)
- **Cấu trúc**: 4 sections chính:
  1. **Brand & Description**: Logo, mô tả, social links
  2. **Quick Links**: Links đến các khóa học
  3. **Support**: Links hỗ trợ
  4. **Contact**: Thông tin liên hệ
- **Độ phức tạp**: Cao, nhiều HTML và logic hiển thị
- **Vị trí**: `DATMOS.Web/Areas/Customer/Views/Shared/Sections/Footer/`

### 2. Admin Footer (`_AdminFooter.cshtml`)
- **Cấu trúc**: Đơn giản với copyright và vài links
- **Độ phức tạp**: Thấp
- **Vị trí**: `DATMOS.Web/Areas/Admin/Views/Shared/Sections/Footer/`

### 3. Layout sử dụng
- **Customer Layout**: `@await Html.PartialAsync("~/Areas/Customer/Views/Shared/Sections/Footer/CustomerFooterItems.cshtml")`
- **Admin Layout**: Cần kiểm tra nhưng có thể tương tự

## Kiến trúc hệ thống

### 1. Entity Design
```csharp
// DATMOS.Core/Entities/Footer.cs
public class Footer
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; set; } = string.Empty;

    [StringLength(500)]
    public string Content { get; set; } = string.Empty;

    [StringLength(200)]
    public string Url { get; set; } = string.Empty;

    // Phân loại section: Brand, Links, Support, Contact, Copyright, Social
    [Required]
    [StringLength(50)]
    public string Section { get; set; } = "Links";

    public int Order { get; set; } = 0;
    
    public bool IsVisible { get; set; } = true;

    // Phân loại footer: Admin, Customer, Teacher, Landing
    [Required]
    [StringLength(20)]
    public string FooterType { get; set; } = "Customer";

    // Phân cấp (cho nested items trong section)
    [StringLength(50)]
    public string? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public Footer? Parent { get; set; }

    public ICollection<Footer> Children { get; set; } = new List<Footer>();

    // Additional fields for specific types
    [StringLength(50)]
    public string? Area { get; set; }

    [StringLength(50)]
    public string? Controller { get; set; }

    [StringLength(50)]
    public string? Action { get; set; }

    [StringLength(50)]
    public string? Target { get; set; } // _blank, _self, etc.
}
```

### 2. Cấu trúc thư mục
```
DATMOS.Web/
├── Services/
│   └── Navigation/
│       ├── IFooterService.cs              # Interface chung
│       └── FooterService.cs               # Implementation chung
├── ViewComponents/
│   └── FooterViewComponent.cs             # ViewComponent duy nhất
├── Models/
│   └── ViewModels/
│       └── Navigation/
│           ├── FooterItemViewModel.cs     # ViewModel cho footer item
│           └── FooterDataViewModel.cs     # ViewModel cho footer data
└── Views/
    └── Shared/
        └── Components/
            └── Footer/
                ├── Default.cshtml         # View mặc định
                ├── RenderFooter.cshtml    # Partial render theo section
                └── Sections/              # Các partial cho từng section type
                    ├── _BrandSection.cshtml
                    ├── _LinksSection.cshtml
                    ├── _SupportSection.cshtml
                    ├── _ContactSection.cshtml
                    └── _CopyrightSection.cshtml
```

### 3. Service Layer
**IFooterService.cs** - Interface chung:
```csharp
public interface IFooterService
{
    Task<List<FooterItemViewModel>> GetFooterAsync(string footerType, string? area = null);
    Task<FooterItemViewModel?> GetFooterItemByIdAsync(string id);
    Task<Dictionary<string, List<FooterItemViewModel>>> GetFooterBySectionsAsync(string footerType, string? area = null);
    Task<FooterDataViewModel> GetFooterDataAsync(string footerType, string? area = null);
    Task ClearCacheAsync(string footerType);
}
```

**FooterService.cs** - Implementation:
- Lấy footer từ database theo `FooterType` và `Area`
- Nhóm items theo `Section` để render
- Hỗ trợ caching với MemoryCache
- Xử lý phân cấp footer đệ quy

### 4. ViewComponent
**FooterViewComponent.cs**:
```csharp
public class FooterViewComponent : ViewComponent
{
    private readonly IFooterService _footerService;
    
    public FooterViewComponent(IFooterService footerService)
    {
        _footerService = footerService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(string footerType, string? area = null)
    {
        var footerData = await _footerService.GetFooterDataAsync(footerType, area);
        return View(footerData);
    }
}
```

### 5. View Templates
**Default.cshtml** - Main view:
```html
@model FooterDataViewModel
@if (Model?.FooterBySections != null && Model.FooterBySections.Any())
{
    <footer class="content-footer footer bg-footer-theme @(Model.FooterType)-footer" id="footer">
        <div class="container-xxl">
            <div class="row py-5">
                @foreach (var section in Model.FooterBySections)
                {
                    @await Html.PartialAsync($"Components/Footer/Sections/_{section.Key}Section", section.Value)
                }
            </div>
            
            @if (Model.FooterBySections.ContainsKey("Copyright"))
            {
                <div class="border-top py-4">
                    @await Html.PartialAsync("Components/Footer/Sections/_CopyrightSection", Model.FooterBySections["Copyright"])
                </div>
            }
        </div>
    </footer>
}
```

**RenderFooter.cshtml** - Generic partial:
```html
@model List<FooterItemViewModel>
@foreach (var item in Model.OrderBy(m => m.Order))
{
    @if (!item.IsVisible) continue;
    
    <li class="@item.CssClass">
        @if (!string.IsNullOrEmpty(item.Url))
        {
            <a href="@item.Url" target="@item.Target" class="footer-link">
                @if (!string.IsNullOrEmpty(item.Icon))
                {
                    <i class="@item.Icon"></i>
                }
                @item.Title
            </a>
        }
        else if (!string.IsNullOrEmpty(item.Controller) && !string.IsNullOrEmpty(item.Action))
        {
            <a asp-area="@item.Area" asp-controller="@item.Controller" asp-action="@item.Action" class="footer-link">
                @if (!string.IsNullOrEmpty(item.Icon))
                {
                    <i class="@item.Icon"></i>
                }
                @item.Title
            </a>
        }
        else
        {
            <span>
                @if (!string.IsNullOrEmpty(item.Icon))
                {
                    <i class="@item.Icon"></i>
                }
                @item.Title
            </span>
        }
    </li>
}
```

## Kế hoạch triển khai chi tiết

### Phase 1: Database & Entity (Ngày 1-2)
1. **Tạo Entity `Footer`** trong `DATMOS.Core/Entities/`
2. **Tạo migration**: `AddFooterEntity`
3. **Update database** và test migration
4. **Tạo seed data** cho Customer và Admin footer

### Phase 2: Service Layer (Ngày 3-4)
1. **Tạo `IFooterService`** interface
2. **Implement `FooterService`** với caching
3. **Tạo ViewModels**: `FooterItemViewModel`, `FooterDataViewModel`
4. **Đăng ký service** trong `Program.cs`

### Phase 3: ViewComponent (Ngày 5-6)
1. **Tạo `FooterViewComponent`**
2. **Tạo view templates**: `Default.cshtml`, `RenderFooter.cshtml`
3. **Tạo section partials**: `_BrandSection.cshtml`, `_LinksSection.cshtml`, etc.
4. **Test ViewComponent** với các loại footer khác nhau

### Phase 4: Tích hợp vào Areas (Ngày 7-8)
1. **Cập nhật `_CustomerLayout.cshtml`**:
   ```html
   @await Component.InvokeAsync("Footer", new { footerType = "Customer", area = "Customer" })
   ```
2. **Cập nhật `_AdminLayout.cshtml`**:
   ```html
   @await Component.InvokeAsync("Footer", new { footerType = "Admin", area = "Admin" })
   ```
3. **Cập nhật các layout khác** nếu cần (Teacher, Landing)

### Phase 5: Xóa file cũ & Testing (Ngày 9-10)
1. **Xóa file footer cũ**:
   - `CustomerFooterItems.cshtml`
   - `_AdminFooter.cshtml`
2. **Testing**:
   - Functional testing
   - Performance testing với caching
   - Cross-browser testing
   - Mobile responsiveness testing

## Danh sách file cần tạo/sửa

### File mới cần tạo:
1. `DATMOS.Core/Entities/Footer.cs`
2. `DATMOS.Web/Services/Navigation/IFooterService.cs`
3. `DATMOS.Web/Services/Navigation/FooterService.cs`
4. `DATMOS.Web/ViewComponents/FooterViewComponent.cs`
5. `DATMOS.Web/Models/ViewModels/Navigation/FooterItemViewModel.cs`
6. `DATMOS.Web/Models/ViewModels/Navigation/FooterDataViewModel.cs`
7. `DATMOS.Web/Views/Shared/Components/Footer/Default.cshtml`
8. `DATMOS.Web/Views/Shared/Components/Footer/RenderFooter.cshtml`
9. `DATMOS.Web/Views/Shared/Components/Footer/Sections/_BrandSection.cshtml`
10. `DATMOS.Web/Views/Shared/Components/Footer/Sections/_LinksSection.cshtml`
11. `DATMOS.Web/Views/Shared/Components/Footer/Sections/_SupportSection.cshtml`
12. `DATMOS.Web/Views/Shared/Components/Footer/Sections/_ContactSection.cshtml`
13. `DATMOS.Web/Views/Shared/Components/Footer/Sections/_CopyrightSection.cshtml`

### File cần sửa đổi:
1. `DATMOS.Data/AppDbContext.cs` - Thêm DbSet<Footer>
2. `DATMOS.Data/Migrations/` - Tạo migration mới
3. `DATMOS.Web/Program.cs` - Đăng ký IFooterService
4. `DATMOS.Web/Areas/Customer/Views/Shared/_CustomerLayout.cshtml`
5. `DATMOS.Web/Areas/Admin/Views/Shared/_AdminLayout.cshtml`
6. Các layout khác nếu có (Teacher, Landing)

### File cần xóa:
1. `DATMOS.Web/Areas/Customer/Views/Shared/Sections/Footer/CustomerFooterItems.cshtml`
2. `DATMOS.Web/Areas/Admin/Views/Shared/Sections/Footer/_AdminFooter.cshtml`

## Seed Data mẫu

### Customer Footer Seed:
```json
// Brand Section
{ Id: "customer-brand", Title: "DATMOS Learning", Content: "Hệ thống học tập và thi thử MOS 2019 chuyên nghiệp...", Section: "Brand", FooterType: "Customer", Order: 1 }

// Quick Links Section
{ Id: "customer-courses-all", Title: "Tất cả khóa học", Area: "Customer", Controller: "Course", Action: "Index", Section: "Links", FooterType: "Customer", Order: 1 }
{ Id: "customer-word", Title: "Word 2019", Url: "#", Section: "Links", FooterType: "Customer", Order: 2 }
{ Id: "customer-excel", Title: "Excel 2019", Url: "#", Section: "Links", FooterType: "Customer", Order: 3 }

// Support Section
{ Id: "customer-home", Title: "Trang chủ", Area: "Customer", Controller: "Home", Action: "Index", Section: "Support", FooterType: "Customer", Order: 1 }
{ Id: "customer-progress", Title: "Tiến độ học tập", Area: "Customer", Controller: "Home", Action: "Progress", Section: "Support", FooterType: "Customer", Order: 2 }

// Contact Section
{ Id: "customer-address", Title: "123 Đường ABC, Quận XYZ, TP. Hồ Chí Minh", Icon: "ti ti-map-pin", Section: "Contact", FooterType: "Customer", Order: 1 }
{ Id: "customer-email", Title: "info@datmos.edu.vn", Icon: "ti ti-mail", Section: "Contact", FooterType: "Customer", Order: 2 }

// Social Links (Children of Brand)
{ Id: "customer-facebook", Title: "Facebook", Icon: "ti ti-brand-facebook", Url: "https://facebook.com", Section: "Brand", FooterType: "Customer", ParentId: "customer-brand", Order: 1 }
{ Id: "customer-youtube", Title: "YouTube", Icon: "ti ti-brand-youtube", Url: "https://youtube.com", Section: "Brand", FooterType: "Customer", ParentId: "customer-brand", Order: 2 }

// Copyright
{ Id: "customer-copyright", Title: "© 2025 DATMOS Learning Platform. All rights reserved.", Section: "Copyright", FooterType: "Customer", Order: 1 }
{ Id: "customer-terms", Title: "Điều khoản sử dụng", Url: "#", Section: "Copyright", FooterType: "Customer", Order: 2 }
{ Id: "customer-privacy", Title: "Chính sách bảo mật", Url: "#", Section: "Copyright", FooterType: "Customer", Order: 3 }
```

### Admin Footer Seed:
```json
// Copyright Section
{ Id: "admin-copyright", Title: "© 2025 DATMOS Admin. All rights reserved.", Section: "Copyright", FooterType: "Admin", Order: 1 }
{ Id: "admin-license", Title: "Bản quyền", Url: "#", Section: "Copyright", FooterType: "Admin", Order: 2 }
{ Id: "admin-support", Title: "Hỗ trợ", Url: "#", Section: "Copyright", FooterType: "Admin", Order: 3 }
```

## Trạng thái hiện tại
- [ ] **Phase 1**: Database & Entity - Chưa bắt đầu
- [ ] **Phase 2**: Service Layer - Chưa bắt đầu
- [ ] **Phase 3**: ViewComponent - Chưa bắt đầu
- [ ] **Phase 4**: Tích hợp - Chưa bắt đầu
- [ ] **Phase 5**: Xóa file cũ & Testing - Chưa bắt đầu

## Ưu điểm của kiến trúc mới
1. **Quản lý tập trung**: Một bảng duy nhất cho tất cả footer
2. **Linh hoạt**: Thêm loại footer mới chỉ cần thêm data, không cần code
3. **Dễ bảo trì**: Thay đổi footer content từ database
4. **Hiệu suất**: Caching theo loại footer
5. **Consistency**: Đồng bộ footer design across all areas

---
*Document version: 1.0 - Created: 2025-12-23*
*Kiến trúc footer động tham khảo từ kiến trúc menu động đã triển khai*
