# Kế hoạch Tái cấu trúc Navbar dùng chung

**Người yêu cầu:** QuocDat-PC
**Ngày tạo:** 2025-12-24
**Mục tiêu:** Tạo một `Navbar` có thể tái sử dụng trên toàn bộ ứng dụng web, bao gồm các `Area` `Admin`, `Customer` và `Tearch` mới. Việc triển khai sẽ học theo mô hình của `MenuViewComponent` đã có sẵn để đảm bảo tính nhất quán trong kiến trúc.

---

## 1. Phân tích & Hướng tiếp cận

Yêu cầu là tạo một `Navbar` dùng chung, tương tự như cách `Menu` đang được hiển thị. Dựa trên cấu trúc dự án, `Menu` được triển khai dưới dạng một **View Component**. Đây là một pattern chuẩn trong ASP.NET Core để đóng gói logic và UI của một phần giao diện có thể tái sử dụng.

Chúng ta sẽ áp dụng cùng một pattern:
1.  **Tạo `NavbarViewComponent.cs`**: Một class logic để xử lý việc hiển thị component.
2.  **Tạo View cho View Component**: Một file `.cshtml` chứa mã HTML cho `Navbar`. View này sẽ gọi đến `MenuViewComponent` đã có để hiển thị các mục menu.
3.  **Tích hợp vào các Layout**: Cập nhật các file `_Layout.cshtml` của các `Area` để gọi `NavbarViewComponent` thay vì mã HTML trực tiếp, đảm bảo tính nhất quán và dễ bảo trì.
4.  **Tạo cấu trúc cho `Area` mới (`Tearch`)**: Vì `Area` này chưa tồn tại, chúng ta cần tạo cấu trúc thư mục và file layout cơ bản cho nó.

## 2. Các bước thực hiện chi tiết

### Bước 1: Tạo `NavbarViewComponent` Class

Tạo một file C# mới để định nghĩa logic cho View Component.

- **Vị trí file:** `DATMOS.Web/ViewComponents/NavbarViewComponent.cs`
- **Nội dung:**

```csharp
using Microsoft.AspNetCore.Mvc;

namespace DATMOS.Web.ViewComponents
{
    /// <summary>
    /// View Component này chịu trách nhiệm hiển thị thanh điều hướng (Navbar) chung của trang web.
    /// Nó đóng vai trò là một container chứa các thành phần khác như Menu, ô tìm kiếm, thông tin người dùng, v.v.
    /// </summary>
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Trả về view mặc định cho component này.
            // View này nằm tại /Views/Shared/Components/Navbar/Default.cshtml
            return View();
        }
    }
}
```

### Bước 2: Tạo View cho `NavbarViewComponent`

View này sẽ chứa mã HTML của `Navbar` và quan trọng là nó sẽ gọi `MenuViewComponent` ở bên trong.

1.  Tạo thư mục `Navbar` theo đường dẫn sau: `DATMOS.Web/Views/Shared/Components/Navbar`
2.  Tạo file `Default.cshtml` bên trong thư mục đó.

- **Vị trí file:** `DATMOS.Web/Views/Shared/Components/Navbar/Default.cshtml`
- **Nội dung:**

```html
@*
    Đây là view cho NavbarViewComponent.
    Nó định nghĩa cấu trúc HTML của thanh điều hướng.
    Component Menu hiện tại được gọi bên trong để hiển thị các liên kết điều hướng động.
*@
<nav class="navbar navbar-expand-lg navbar-light bg-white shadow-sm">
    <div class="container">
        <a class="navbar-brand" asp-area="Customer" asp-controller="Home" asp-action="Index">
            <strong>DATMOS</strong>
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarMain" aria-controls="navbarMain" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarMain">
            @* Gọi MenuViewComponent đã có để hiển thị menu động *@
            @await Component.InvokeAsync("Menu")

            @* Có thể thêm các thành phần khác của Navbar ở đây (ví dụ: tìm kiếm, hồ sơ người dùng) *@
            <ul class="navbar-nav ms-auto">
                <li class="nav-item">
                    <a class="nav-link" href="#">
                        <i class="fas fa-user"></i> Profile
                    </a>
                </li>
                 <li class="nav-item">
                    <a class="nav-link" href="#">
                        <i class="fas fa-shopping-cart"></i> Cart
                    </a>
                </li>
            </ul>
        </div>
    </div>
</nav>
```

### Bước 3: Tạo cấu trúc cho `Area` "Tearch"

Area `Tearch` chưa tồn tại, cần tạo cấu trúc thư mục và file layout cho nó.

1.  **Tạo cây thư mục** bằng các lệnh sau:
    ```powershell
    mkdir DATMOS.Web/Areas/Tearch
    mkdir DATMOS.Web/Areas/Tearch/Controllers
    mkdir DATMOS.Web/Areas/Tearch/Views
    mkdir DATMOS.Web/Areas/Tearch/Views/Shared
    ```

2.  **Tạo file `_ViewStart.cshtml`** để thiết lập layout mặc định cho Area này.
    - **Vị trí file:** `DATMOS.Web/Areas/Tearch/Views/_ViewStart.cshtml`
    - **Nội dung:**
    ```csharp
    @{
        Layout = "_Layout";
    }
    ```

3.  **Tạo file `_Layout.cshtml`** cho Area.
    - **Vị trí file:** `DATMOS.Web/Areas/Tearch/Views/Shared/_Layout.cshtml`
    - **Nội dung (sao chép từ một `_Layout.cshtml` khác và chỉnh sửa nếu cần):**
    ```html
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-g" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>@ViewData["Title"] - Tearch Area</title>
        @* Thêm các link CSS và script cần thiết *@
        <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
        <link rel="stylesheet" href="~/css/site.css" />
    </head>
    <body>
        <header>
            @* Gọi Navbar View Component dùng chung *@
            @await Component.InvokeAsync("Navbar")
        </header>
        <div class="container">
            <main role="main" class="pb-3">
                @RenderBody()
            </main>
        </div>

        <footer class="border-top footer text-muted">
             @* Giữ lại hoặc thay thế bằng Footer View Component nếu có *@
            <div class="container">
                &copy; 2025 - DATMOS
            </div>
        </footer>
        <script src="~/lib/jquery/dist/jquery.min.js"></script>
        <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
        <script src="~/js/site.js" asp-append-version="true"></script>
        @await RenderSectionAsync("Scripts", required: false)
    </body>
    </html>
    ```

### Bước 4: Tích hợp `NavbarViewComponent` vào các Layout

Bây giờ, thay thế mã HTML của navbar trong các file layout hiện có bằng một dòng lệnh gọi View Component.

1.  **File `DATMOS.Web/Areas/Customer/Views/Shared/_Layout.cshtml`** (Nếu tồn tại)
2.  **File `DATMOS.Web/Areas/Admin/Views/Shared/_Layout.cshtml`** (Nếu tồn tại)


**Hành động:**
Trong mỗi file trên, tìm khối mã HTML định nghĩa `<nav class="navbar ...">...</nav>` và **thay thế toàn bộ khối đó** bằng dòng sau:

```csharp
@await Component.InvokeAsync("Navbar")
```

---

## 3. Tổng kết

Sau khi hoàn thành các bước trên:
- Toàn bộ ứng dụng (bao gồm các Area `Admin`, `Customer`, `Tearch`) sẽ sử dụng chung một `Navbar`.
- Mọi thay đổi về giao diện hoặc cấu trúc `Navbar` chỉ cần thực hiện tại một nơi duy nhất: `DATMOS.Web/Views/Shared/Components/Navbar/Default.cshtml`.
- Kiến trúc dự án trở nên nhất quán và dễ bảo trì hơn, tuân thủ đúng theo các pattern của ASP.NET Core.
