# Kế Hoạch Triển Khai Module Identity cho DATMOS

---

| Trạng Thái | Chủ sở hữu | Cập nhật lần cuối |
| :--- | :--- | :--- |
| ✅ **Hoàn thành** | Nhóm Phát triển | 2025-12-25 |

---

## 1. Tóm Tắt

Tài liệu này trình bày kế hoạch chiến lược để tích hợp framework **ASP.NET Core Identity** vào ứng dụng DATMOS. Mục tiêu chính là thiết lập một hệ thống mạnh mẽ, an toàn và có khả năng mở rộng để xác thực và phân quyền người dùng. Việc triển khai này bao gồm chức năng đăng ký, đăng nhập, quản lý vai trò và bảo vệ các khu vực cụ thể của ứng dụng. Giao diện người dùng cho việc xác thực sẽ được thiết kế tùy chỉnh dựa trên một mẫu HTML được cung cấp để đảm bảo trải nghiệm thương hiệu nhất quán.

## 2. Yêu Cầu Cốt Lõi

- **Xác thực người dùng:** Cung cấp chức năng cho người dùng đăng ký, đăng nhập và đăng xuất.
- **Phân quyền dựa trên vai trò (Role-Based):** Triển khai hệ thống quản lý vai trò để kiểm soát quyền truy cập vào các phần khác nhau của ứng dụng.
- **Khu vực được bảo vệ:** Bảo mật các khu vực `Admin`, `Customer` (`User`), và `Teacher`, đảm bảo chỉ những người dùng đã xác thực và được cấp quyền mới có thể truy cập.
- **Model Người dùng Tùy chỉnh:** Tạo một model `AppUser` linh hoạt có thể được mở rộng với các thuộc tính bổ sung trong tương lai.
- **Giao diện người dùng tùy chỉnh:** Thay thế giao diện Identity mặc định bằng một giao diện tùy chỉnh, được thiết kế chuyên nghiệp.
- **Dữ liệu mẫu (Data Seeding):** Khởi tạo cơ sở dữ liệu với các vai trò và tài khoản người dùng mặc định cho mục đích phát triển và thử nghiệm.

## 3. Kiến Trúc Hệ Thống & Các Thành Phần Chính

Module Identity tích hợp với một số project chính trong giải pháp DATMOS.

| Thành phần | Đường dẫn / Vị trí | Mô tả |
| :--- | :--- | :--- |
| **Thực thể User** | `DATMOS.Core/Entities/AppUser.cs` | Lớp người dùng tùy chỉnh kế thừa từ `IdentityUser`. Cho phép mở rộng thuộc tính người dùng trong tương lai (ví dụ: `FullName`). |
| **Database Context**| `DATMOS.Data/AppDbContext.cs` | Kế thừa từ `IdentityDbContext<AppUser>` để quản lý tất cả các bảng liên quan đến Identity trong cơ sở dữ liệu. |
| **Cấu hình Service** | `DATMOS.Web/Program.cs` | Đăng ký các dịch vụ Identity, cấu hình chính sách mật khẩu và thiết lập hành vi cookie ứng dụng để xác thực. |
| **Middleware** | `DATMOS.Web/Program.cs` | Middleware `UseAuthentication()` và `UseAuthorization()` được cấu hình trong pipeline request HTTP để thực thi bảo mật. |
| **Giao diện Identity**| `DATMOS.Web/Areas/Identity/` | Các trang Razor Pages/Views cho Đăng nhập, Đăng ký, v.v. Chúng được tạo tự động (scaffold) và sau đó tùy chỉnh. |
| **Seeder Dữ liệu** | `DATMOS.Data/IdentitySeeder.cs` | Một lớp chuyên dụng chịu trách nhiệm tạo các vai trò và người dùng ban đầu trong cơ sở dữ liệu. |
| **Trình chạy Seeder**| `DATMOS.Data/Program.cs` | Điểm vào của ứng dụng console được sử dụng để thực thi quá trình khởi tạo dữ liệu. |
| **Account Controller**|`DATMOS.Web/Areas/Identity/Controllers/AccountController.cs`| Quản lý logic đăng ký người dùng, bao gồm việc gán vai trò mặc định cho người dùng mới. |

## 4. Các Nhiệm Vụ Triển Khai

Quá trình triển khai được chia thành bốn giai đoạn riêng biệt.

### Giai đoạn 1: Cài đặt & Cấu hình

- [x] **1.1. Cài đặt các Gói NuGet:** Thêm các gói Identity cần thiết vào project `DATMOS.Web`.
  ```powershell
  dotnet add DATMOS.Web/DATMOS.Web.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
  dotnet add DATMOS.Web/DATMOS.Web.csproj package Microsoft.AspNetCore.Identity.UI
  ```
- [x] **1.2. Định nghĩa `AppUser` Tùy chỉnh:** Tạo lớp `DATMOS.Core/Entities/AppUser.cs`.
- [x] **1.3. Cập nhật `AppDbContext`:** Sửa đổi `DATMOS.Data/AppDbContext.cs` để kế thừa từ `IdentityDbContext<AppUser>`.
- [x] **1.4. Cấu hình Dịch vụ Identity:** Trong `DATMOS.Web/Program.cs`, đăng ký các dịch vụ Identity bằng `builder.Services.AddIdentity<...>()` và cấu hình cookie với `ConfigureApplicationCookie`.
- [x] **1.5. Cấu hình Middleware:** Thêm `app.UseAuthentication()` và `app.UseAuthorization()` vào pipeline request theo đúng thứ tự.

### Giai đoạn 2: Tích hợp Cơ sở dữ liệu

- [x] **2.1. Tạo Migration:** Tạo một migration mới của Entity Framework để thêm schema cho các bảng Identity.
  ```powershell
  dotnet ef migrations add AddIdentitySchema -p DATMOS.Data/DATMOS.Data.csproj --startup-project DATMOS.Web/DATMOS.Web.csproj
  ```
- [x] **2.2. Áp dụng Migration:** Cập nhật cơ sở dữ liệu với schema mới.
  ```powershell
  dotnet ef database update --startup-project DATMOS.Web/DATMOS.Web.csproj
  ```

### Giai đoạn 3: Triển khai Giao diện Người dùng (UI/UX)

- [x] **3.1. Cài đặt Công cụ Scaffolding:** Đảm bảo `dotnet-aspnet-codegenerator` đã được cài đặt toàn cục.
  ```powershell
  dotnet tool install -g dotnet-aspnet-codegenerator
  ```
- [x] **3.2. Scaffolding các Trang Identity:** Tạo ra logic backend cần thiết cho các trang Account.
  ```powershell
  dotnet aspnet-codegenerator identity -p DATMOS.Web/DATMOS.Web.csproj --files "Account.Login;Account.Register;Account.Logout" --dbContext "DATMOS.Data.AppDbContext" --userClass "DATMOS.Core.Entities.AppUser"
  ```
- [x] **3.3. Tùy chỉnh Giao diện:**
    - Thay thế mã HTML được tạo trong `Login.cshtml` và `Register.cshtml` bằng thiết kế tùy chỉnh từ các file mẫu.
    - Tích hợp các Tag Helper của ASP.NET (`asp-for`, `asp-validation-for`, `asp-page`) vào các form HTML mới.
    - Di chuyển tất cả các tài sản CSS và JS cần thiết vào thư mục `wwwroot` và liên kết chúng một cách chính xác trong các file view.

### Giai đoạn 4: Bảo mật & Phân quyền

- [x] **4.1. Bảo vệ các Khu vực Ứng dụng:** Áp dụng thuộc tính `[Authorize]` cho các controller trong các khu vực an toàn (ví dụ: `Admin`, `Customer`, `Teacher`) để yêu cầu xác thực.
  ```csharp
  [Area("Admin")]
  [Authorize(Roles = "Administrator")]
  public class HomeController : Controller { /* ... */ }
  ```
- [x] **4.2. Xác minh Chuyển hướng:** Xác nhận rằng người dùng chưa xác thực được chuyển hướng chính xác đến trang đăng nhập tùy chỉnh khi cố gắng truy cập một tài nguyên được bảo vệ.

## 5. Chiến Lược Dữ Liệu Mẫu (Seeding)

Để phát triển và thử nghiệm hiệu quả, cơ sở dữ liệu được khởi tạo với dữ liệu ban đầu.

**Lý do:** Việc seeding đảm bảo một tập hợp người dùng và vai trò nhất quán và có thể dự đoán được trên tất cả các môi trường phát triển, loại bỏ nhu cầu tạo tài khoản thủ công trong quá trình thử nghiệm.

#### Các Vai trò được tạo mẫu:
1.  **Administrator:** Quyền truy cập toàn bộ hệ thống.
2.  **Teacher:** Quyền truy cập vào các chức năng dành riêng cho giáo viên.
3.  **User:** Quyền của người dùng/khách hàng tiêu chuẩn (mặc định cho các đăng ký mới).

#### Các Tài khoản người dùng được tạo mẫu:

| Email | Mật khẩu | Vai trò được gán | Trạng thái |
| :--- | :--- | :--- | :--- |
| `admin@datmos.com` | `Admin@123` | `Administrator`| Đã xác thực Email |
| `teacher@datmos.com`| `Teacher@123`| `Teacher` | Đã xác thực Email |
| `user@datmos.com` | `User@123` | `User` | Đã xác thực Email |

**Thực thi:** Quá trình seeding được thực hiện bằng cách chạy ứng dụng console `DATMOS.Data`, được cấu hình để gọi `IdentitySeeder`.

## 6. Lưu Ý Bảo Mật

- **Mật khẩu phát triển:** Các mật khẩu được liệt kê ở trên là yếu và chỉ dành cho **môi trường phát triển**.
- **Môi trường Production:** Trong môi trường triển khai thực tế (production), các tài khoản mặc định này phải được xóa hoặc thay đổi mật khẩu thành các giá trị mạnh, duy nhất.
- **Chính sách Mật khẩu:** Chính sách mật khẩu mặc định của Identity đã được nới lỏng để dễ dàng phát triển. Điều này phải được xem xét và làm chặt chẽ hơn cho môi trường production (trong file `Program.cs`).

## 7. Các Cải Tiến Trong Tương Lai

- **Đăng nhập bằng Mạng xã hội:** Tích hợp các nhà cung cấp bên thứ ba (Google, Facebook) để đơn giản hóa việc đăng ký và đăng nhập của người dùng.
- **Xác thực Hai yếu tố (2FA):** Triển khai 2FA để tăng cường bảo mật, đặc biệt cho các vai trò `Administrator` và `Teacher`.
- **Xác thực Email:** Bật và thực thi xác thực email cho các đăng ký người dùng mới để xác minh danh tính người dùng.
- **Đặt lại Mật khẩu:** Triển khai một quy trình "Quên mật khẩu" đầy đủ tính năng.
