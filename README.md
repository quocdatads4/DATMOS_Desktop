# DATMOS_Desktop - .NET 8 Hybrid Desktop Solution

Một giải pháp Hybrid Desktop Application kết hợp WinForms host và ASP.NET Core MVC Web App nhúng.

## Kiến Trúc Dự Án

```
DATMOS_Desktop/
├── DATMOS.Core/ (Class Library)
│   ├── Entities/ (Business objects: Product)
│   └── Interfaces/
├── DATMOS.Data/ (Class Library)
│   ├── AppDbContext (EF Core với PostgreSQL)
│   └── Migrations/
├── DATMOS.Web/ (ASP.NET Core MVC Library)
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   └── WebEntryPoint.cs (Custom hosting entry point)
└── DATMOS.WinApp/ (WinForms Application)
    ├── Program.cs (App entry với web server hosting)
    ├── MainForm.cs (Main window với WebView2)
    └── Services/
```

## Tính Năng Chính

1. **Hybrid Architecture**: Kết hợp WinForms desktop app với ASP.NET Core MVC web app
2. **WebView2 Integration**: Hiển thị web app trong WinForms control
3. **PostgreSQL Database**: Sử dụng EF Core với PostgreSQL cho data persistence
4. **Kestrel Web Server**: Embedded web server chạy trên localhost:5000
5. **Modern UI**: Web-based UI với responsive design trong desktop window

## Yêu Cầu Hệ Thống

- .NET 8 SDK
- PostgreSQL Database (cho DATMOS.Data và DATMOS.Web)
- WebView2 Runtime (có sẵn trên Windows 10/11)
- Visual Studio 2022 hoặc VS Code

## Cấu Hình Database

### PostgreSQL (cho DATMOS.Data và DATMOS.Web)
```sql
-- Tạo database
CREATE DATABASE datmos_db;

-- Tạo user (nếu cần)
CREATE USER datmos_user WITH PASSWORD 'password';
GRANT ALL PRIVILEGES ON DATABASE datmos_db TO datmos_user;
```

Connection string mặc định: `Host=localhost;Database=datmos_db;Username=postgres;Password=password`

## Cách Chạy Ứng Dụng

### 1. Build Solution
```bash
dotnet build DATMOS_Desktop.sln
```

### 2. Chạy WinForms Application
```bash
dotnet run --project DATMOS.WinApp/DATMOS.WinApp.csproj
```

### 3. Ứng dụng sẽ:
- Khởi động Kestrel web server trên port 5000
- Tạo/ensure database PostgreSQL
- Mở WinForms window với WebView2
- Tự động navigate đến http://localhost:5000

## Cấu Trúc Code

### DATMOS.Core
- `Entities/Product.cs`: Product entity với Id, Name, Price, timestamps

### DATMOS.Data
- `AppDbContext.cs`: DbContext với PostgreSQL configuration

### DATMOS.Web
- `WebEntryPoint.cs`: Static class với `CreateHostBuilder()` method để tạo IHost

### DATMOS.WinApp
- `Program.cs`: Application entry point với background web server
- `MainForm.cs`: Main form với WebView2 control và UI controls

## Database Migrations

### Tạo migration
```bash
cd DATMOS.Data
dotnet ef migrations add InitialCreate
```

### Áp dụng migration
```bash
dotnet ef database update
```

## Mở Rộng

### Thêm Entities
1. Thêm class trong `DATMOS.Core/Entities/`
2. Thêm DbSet trong `AppDbContext.cs`
3. Tạo migration mới

### Thêm Controllers/Views
1. Thêm Controller trong `DATMOS.Web/Controllers/`
2. Thêm View trong `DATMOS.Web/Views/`
3. Web app sẽ tự động nhận diện

### Cấu hình Port
Thay đổi port trong `DATMOS.WinApp/Program.cs`:
```csharp
_webHost = WebEntryPoint.CreateHostBuilder(new string[] { }, 5000); // Thay 5000 bằng port khác
```

## Troubleshooting

### WebView2 không khởi động
- Đảm bảo WebView2 Runtime đã được cài đặt
- Kiểm tra internet connection (cho lần đầu khởi động)

### Database connection failed
- Kiểm tra PostgreSQL service đang chạy
- Verify connection string trong `AppDbContext.cs`

### Port 5000 đang được sử dụng
- Thay đổi port trong `Program.cs`
- Hoặc kill process đang sử dụng port 5000

## License

MIT License

## Tác Giả

DATMOS Desktop Solution - Hybrid .NET 8 Application
