# Technology Context

## Stack công nghệ
- **Runtime:** .NET 8 SDK (Long Term Support)
- **Desktop Framework:** Windows Forms (WinForms) - chịu trách nhiệm Native Shell.
- **Web Framework:** ASP.NET Core MVC - chịu trách nhiệm UI và Business Logic.
- **Browser Engine:** Microsoft.Web.WebView2 (Edge Chromium) - Rendering Engine.
- **Database:** PostgreSQL 14+ (Production grade relational database).
- **ORM:** Entity Framework Core 8 (Npgsql Provider).

## Cấu hình môi trường
- **Port:** Mặc định `5000` (HTTP). Có thể cấu hình động.
- **Connection String:** Cấu hình trong `WebEntryPoint.cs` hoặc `appsettings.json`.
  - Default: `Host=localhost;Database=datmos_db;Username=postgres;Password=password`

## Dependencies quan trọng
- **NuGet Packages:**
  - `Microsoft.Web.WebView2`: Điều khiển trình duyệt nhúng.
  - `Npgsql.EntityFrameworkCore.PostgreSQL`: Driver EF Core cho PostgreSQL.
  - `Microsoft.Extensions.Hosting`: Quản lý Generic Host và Dependency Injection.
  - `Microsoft.AspNetCore.App`: Framework reference cho Web components.

## Yêu cầu Runtime
- **OS:** Windows 10/11 (64-bit khuyến nghị).
- **WebView2 Runtime:** Phải được cài đặt trên máy client (thường có sẵn trên Windows 10/11 cập nhật mới).
- **PostgreSQL Service:** Phải đang chạy và truy cập được qua Connection String.