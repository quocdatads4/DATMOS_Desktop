# Prompt khởi tạo dự án .NET 8 Hybrid (WinForms + ASP.NET Core)

**Role:** Bạn là một chuyên gia về .NET Architect và Full-stack Developer.

**Task:** Hãy xây dựng cấu trúc mã nguồn cho một giải pháp .NET 8 Hybrid Desktop Application. Ứng dụng này sẽ sử dụng WinForms làm host container, nhưng toàn bộ giao diện người dùng (UI) sẽ được render bởi ASP.NET Core MVC thông qua WebView2 control.

**Yêu cầu cấu trúc Solution:**
Tạo Solution tên là `[PROJECT_NAME]` với 4 project thành phần:
1.  `[PROJECT_NAME].Core` (Class Library): Chứa Entities, Interfaces và DTOs. Không phụ thuộc vào Web hay Data.
2.  `[PROJECT_NAME].Data` (Class Library): Chứa EF Core DbContext (sử dụng PostgreSQL/Npgsql) và Migrations.
3.  `[PROJECT_NAME].Web` (ASP.NET Core Web App - MVC): 
    - Chứa Controllers, Views, wwwroot.
    - **Quan trọng:** Project này cần được thiết kế như một Library để WinForms có thể tham chiếu và khởi chạy.
    - Tạo class `WebEntryPoint` để cấu hình Kestrel.
4.  `[PROJECT_NAME].WinApp` (WinForms App): 
    - Project khởi chạy chính (Startup project).
    - Tham chiếu đến `.Web` và `.Data`.

**Yêu cầu kỹ thuật chi tiết:**

1.  **Web Project (`.Web`):**
    *   Tạo class `WebEntryPoint` với phương thức tĩnh `CreateHostBuilder(string[] args, int port)`.
    *   Cấu hình Kestrel: `options.ListenLocalhost(port)`.
    *   Đăng ký dịch vụ: `AddControllersWithViews`, `AddDbContext<AppDbContext>`.
    *   Cấu hình Auto-Migration: Trong `Configure`, kiểm tra và chạy `dbContext.Database.Migrate()` để đảm bảo DB luôn sẵn sàng.

2.  **WinForms Project (`.WinApp`):**
    *   **Program.cs:**
        *   Khởi tạo `CancellationTokenSource`.
        *   Sử dụng `Task.Run` để khởi chạy `WebEntryPoint.CreateHostBuilder(...).Build().Run()` trong background thread.
        *   Đảm bảo Web Server tắt an toàn khi WinForms app đóng (`cancellationToken.Cancel()`).
    *   **MainForm.cs:**
        *   Tích hợp `Microsoft.Web.WebView2.WinForms`.
        *   Khởi tạo WebView2 bất đồng bộ: `await webView.EnsureCoreWebView2Async()`.
        *   Điều hướng (`Source`) tới `http://localhost:[PORT]`.
        *   **Resilience:** Thêm logic trong sự kiện `NavigationCompleted`: Nếu thất bại (do server chưa lên kịp), đợi 1-2s rồi `Reload()`.

3.  **Database:**
    *   Sử dụng PostgreSQL.
    *   Connection String mặc định trỏ tới `localhost`.

**Code Output:**
Hãy cung cấp mã nguồn đầy đủ và chi tiết cho các file quan trọng sau:
- `[PROJECT_NAME].WinApp/Program.cs` (Xử lý Threading và Lifecycle).
- `[PROJECT_NAME].WinApp/MainForm.cs` (Xử lý WebView2 và Retry logic).
- `[PROJECT_NAME].Web/WebEntryPoint.cs` (Cấu hình Kestrel và DI).
- `[PROJECT_NAME].Data/AppDbContext.cs` (Cấu hình EF Core).

**Lưu ý:**
- Xử lý Exception handling cẩn thận (ví dụ: Port bị chiếm dụng, WebView2 Runtime thiếu).
- Viết code rõ ràng, tuân thủ SOLID.
- Thêm comment giải thích các đoạn logic phức tạp (đặc biệt là phần giao tiếp giữa WinForms và Web Host).