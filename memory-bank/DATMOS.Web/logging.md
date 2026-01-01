# Memory Bank: Hệ thống Logging (Ghi nhật ký)

## 1. Yêu cầu
- Tự động tạo thư mục `Log` trong thư mục gốc của dự án.
- Ghi log ra file, tự động tách file theo ngày.
- Định dạng tên file: `[TênDựÁn]-[Ngày].txt` (Ví dụ: `DATMOS.Web-20241222.txt`).
- Ghi lại các hoạt động của hệ thống để theo dõi và debug.

## 2. Phân tích hiện trạng
- **Mã nguồn hiện tại (`Program.cs`)**: Đang sử dụng phương pháp ghi log thủ công thông qua hàm `LogToFile` và `File.AppendAllText`.
- **Hạn chế**:
  - Không tích hợp với interface `ILogger<T>` chuẩn của ASP.NET Core.
  - Không thu thập được log từ hệ thống (System logs), Entity Framework, hay Identity.
  - Thiếu các tính năng nâng cao như Log Level (Info, Warning, Error), Structured Logging.
  - Gây phân mảnh code trong `Program.cs`.

## 3. Giải pháp kỹ thuật
Sử dụng thư viện **Serilog** kết hợp với **Serilog.Sinks.File**.
- **Serilog**: Thư viện logging mạnh mẽ, hỗ trợ structured data.
- **Serilog.AspNetCore**: Tích hợp sâu vào pipeline của ASP.NET Core, thay thế default logger.
- **Serilog.Sinks.File**: Module hỗ trợ ghi log ra file với khả năng rolling (cuộn) file theo ngày.

## 4. Các bước thực hiện (Implementation Plan)

### Bước 1: Cài đặt NuGet Packages
Chạy lệnh sau trong thư mục dự án `DATMOS.Web`:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

### Bước 2: Cấu hình Serilog trong `Program.cs`
Thay thế đoạn code logging thủ công bằng cấu hình Serilog.

**Code cấu hình:**
```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Mức độ log tối thiểu
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Giảm log rác từ Microsoft
    .Enrich.FromLogContext()
    .WriteTo.Console() // Ghi ra console
    .WriteTo.File(
        path: "Log/DATMOS.Web-.txt", // Đường dẫn file, dấu "-" để nối ngày
        rollingInterval: RollingInterval.Day, // Tạo file mới mỗi ngày
        retainedFileCountLimit: 30, // Giữ log trong 30 ngày
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

// Đăng ký Serilog với Host
builder.Host.UseSerilog();
```

### Bước 3: Dọn dẹp code cũ
- Xóa các hàm `GetDailyLogPath`, `LogToFile`.
- Xóa các dòng gọi `LogToFile(...)`.
- Middleware logging request thủ công (`app.Use(...)`) có thể được thay thế bằng `app.UseSerilogRequestLogging()` nếu cần log HTTP request chi tiết.

## 5. Kết quả mong đợi
- Khi chạy ứng dụng, folder `Log` tự động được tạo.
- File log xuất hiện dạng `DATMOS.Web-yyyyMMdd.txt`.
- Nội dung log chuẩn hóa, bao gồm thời gian, mức độ lỗi, và thông điệp.