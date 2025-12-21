# Project Brief: DATMOS_Desktop

## Tổng quan
DATMOS_Desktop là giải pháp **Hybrid Desktop Application** hiệu năng cao xây dựng trên nền tảng .NET 8. Dự án kết hợp khả năng kiểm soát hệ thống mạnh mẽ của **WinForms** với trải nghiệm người dùng (UX) hiện đại, linh hoạt của **ASP.NET Core MVC** thông qua **WebView2** (Edge Chromium).

## Mục tiêu cốt lõi
1. **Modern UI/UX:** Thoát khỏi sự cứng nhắc của WinForms Controls truyền thống, sử dụng HTML5/CSS3/JS để xây dựng giao diện responsive và đẹp mắt.
2. **Seamless Integration:** Nhúng hoàn toàn Kestrel Web Server vào trong WinForms process, mang lại trải nghiệm "như native app" mà không cần cài đặt IIS hay Web Server rời.
3. **Unified Data:** Sử dụng Entity Framework Core và PostgreSQL làm lớp dữ liệu chung cho cả logic nền (Background Services) và giao diện Web.
4. **Reliability:** Đảm bảo vòng đời (Lifecycle) của Web Server gắn liền chặt chẽ với ứng dụng Desktop, tự động dọn dẹp tài nguyên khi đóng ứng dụng.

## Phạm vi
- **WinApp (Host):** 
  - Entry point của ứng dụng.
  - Quản lý cửa sổ chính (MainForm), System Tray.
  - Hosting Kestrel Server trong Background Thread.
  - Điều khiển WebView2 Control.
- **Web (UI & Logic):** Xử lý nghiệp vụ, Controllers, Views, API endpoints.
- **Data (Persistence):** EF Core DbContext, Migrations, Repository Pattern.
- **Core (Domain):** Entities, Interfaces, DTOs, Constants dùng chung.