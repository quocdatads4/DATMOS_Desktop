# Kế hoạch Modular hóa DATMOS.Web theo kiến trúc Plugin của NopCommerce

**Mục tiêu:** Tái cấu trúc project `DATMOS.Web` theo kiến trúc plugin linh hoạt và có khả năng mở rộng, tương tự như NopCommerce. Mỗi tính năng lớn (Area) sẽ trở thành một "Plugin" độc lập, có thể được cài đặt, gỡ bỏ và cập nhật mà không cần biên dịch lại toàn bộ ứng dụng.

---

## Phần 1: Xây dựng Nền tảng Plugin (Core Infrastructure) (Ước tính: 12 giờ)

*Đây là giai đoạn quan trọng nhất, tạo ra bộ khung cho toàn bộ hệ thống plugin.*

1.  **Định nghĩa Giao diện Plugin cơ sở (`IPlugin`) trong `DATMOS.Core`:**
    *   Tạo interface `IPlugin` với các phương thức chính:
        *   `Install()`: Xử lý logic khi cài đặt plugin (tạo bảng DB, thêm setting, đăng ký menu...).
        *   `Uninstall()`: Dọn dẹp khi gỡ bỏ plugin.
    *   Tạo một lớp `BasePlugin` trừu tượng implement `IPlugin` để cung cấp các hành vi mặc định, giảm mã lặp lại cho các plugin sau này.

2.  **Tạo file mô tả `plugin.json`:**
    *   Chuẩn hóa cấu trúc file `plugin.json` sẽ nằm ở thư mục gốc của mỗi plugin.
    *   **Cấu trúc:**
        ```json
        {
          "SystemName": "DATMOS.Customer",
          "FriendlyName": "Customer Area",
          "Group": "Official",
          "Version": "1.0",
          "Author": "DATMOS Team",
          "DisplayOrder": 1,
          "Description": "Provides customer-facing features like exam lists and exam taking.",
          "AssemblyFileName": "DATMOS.Modules.Customer.dll"
        }
        ```
    *   Tạo một class C# (`PluginDescriptor`) để deserialize nội dung file này.

3.  **Xây dựng `PluginManagerService` trong `DATMOS.Web`:**
    *   **Nhiệm vụ:**
        *   **Khám phá Plugin (Discovery):** Quét thư mục `Plugins` (ví dụ: `DATMOS.Web/Plugins`) khi ứng dụng khởi động để tìm các thư mục plugin.
        *   **Đọc Metadata:** Đọc file `plugin.json` từ mỗi plugin tìm thấy.
        *   **Tải Assembly:** Tải file DLL chính của plugin (`AssemblyFileName`) vào `ApplicationPart` của ASP.NET Core để MVC có thể nhận diện được Controllers, Views của plugin.
        *   **Quản lý Vòng đời:** Cung cấp các phương thức để `Install` và `Uninstall` plugin (gọi đến phương thức của `IPlugin`).
    *   Service này sẽ giữ một danh sách các `PluginDescriptor` đã được tải.

4.  **Cập nhật `Program.cs` của `DATMOS.Web`:**
    *   Đăng ký `PluginManagerService`.
    *   Khi khởi động, gọi `PluginManagerService` để khám phá và tải các assembly plugin vào `IMvcBuilder`.
        ```csharp
        var mvcBuilder = builder.Services.AddControllersWithViews();
        // ...
        var pluginManager = //... resolve service
        pluginManager.LoadAllPluginAssemblies(mvcBuilder);
        ```
    *   **Quan trọng:** `DATMOS.Web` sẽ **KHÔNG** có project reference trực tiếp đến các plugin. Chúng được nạp động.

---

## Phần 2: Chuyển đổi Area thành Plugin (Ước tính: 6 giờ / Plugin)

*Lặp lại quy trình này cho mỗi Area: `Customer`, `Admin`, `Identity`, `Teacher`.*

1.  **Tạo Project Plugin (Class Library):**
    *   Tạo project mới, ví dụ `DATMOS.Modules.Customer`.
    *   **Không** dùng Razor Class Library, mà dùng Class Library (.NET) thông thường để kiểm soát hoàn toàn.

2.  **Thêm `plugin.json`:**
    *   Tạo file `plugin.json` ở thư mục gốc và cấu hình thuộc tính "Copy to Output Directory" = `Always`.

3.  **Di chuyển mã nguồn:**
    *   Chuyển Controllers, Models, Services, Components từ `DATMOS.Web/Areas/Customer` vào project `DATMOS.Modules.Customer`.
    *   Cập nhật namespaces.
    *   Tạo thư mục `Views` và di chuyển các file `.cshtml` vào đó.
    *   Tạo thư mục `wwwroot` và di chuyển các file JS, CSS... vào đó.

4.  **Implement `IPlugin`:**
    *   Tạo một class, ví dụ `CustomerPlugin.cs`, kế thừa từ `BasePlugin`.
    *   **`Install()` method:**
        *   Logic để thêm menu "Customer" vào hệ thống (nếu cần).
        *   Logic để đăng ký các route đặc thù (nếu `MapControllerRoute` mặc định không đủ).
    *   **`Uninstall()` method:**
        *   Logic để xóa menu, dọn dẹp.

5.  **Cấu hình View và Static Files:**
    *   Views cần được cấu hình là `EmbeddedResource`.
    *   Cần một `IViewLocationExpander` tùy chỉnh để hệ thống có thể tìm thấy view được nhúng trong assembly của plugin.
    *   Sử dụng `StaticFileProvider` để phục vụ các file từ thư mục `wwwroot` của plugin. `PluginManagerService` sẽ giúp đăng ký các provider này.

---

## Phần 3: Tích hợp và Quản lý (Ước tính: 8 giờ)

1.  **Xây dựng trang quản lý Plugin trong Area Admin:**
    *   Tạo một trang trong `DATMOS.Modules.Admin` (plugin Admin) để liệt kê tất cả các plugin đã khám phá.
    *   Hiển thị thông tin từ `plugin.json` (Tên, phiên bản, tác giả...).
    *   Cung cấp các nút "Install", "Uninstall", "Reload Plugins" để gọi các phương thức tương ứng của `PluginManagerService`.

2.  **Cấu hình Build Process:**
    *   Thiết lập MSBuild để sau khi build, các project plugin sẽ tự động được copy vào thư mục `DATMOS.Web/Plugins/[PluginSystemName]/`.

3.  **Kiểm thử toàn diện:**
    *   Build toàn bộ solution.
    *   Chạy `DATMOS.Web`.
    *   Vào trang quản lý plugin, cài đặt từng plugin một.
    *   Kiểm tra xem menu, trang, chức năng của plugin vừa cài có hoạt động không.
    *   Kiểm tra gỡ cài đặt.

---

## Tổng thời gian ước tính: ~44 giờ (5-6 ngày làm việc)

-   **Nền tảng Plugin:** 12 giờ
-   **Chuyển đổi 4 Area (6 giờ/cái):** 24 giờ
-   **Tích hợp và Quản lý:** 8 giờ

Đây là một thay đổi kiến trúc lớn, đòi hỏi sự cẩn thận trong việc xây dựng nền tảng ban đầu. Tuy nhiên, nó sẽ mang lại sự linh hoạt cực lớn cho việc phát triển và bảo trì trong tương lai.
