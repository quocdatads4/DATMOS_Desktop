# Active Context

## Trạng thái hiện tại
- **Skeleton Complete:** Cấu trúc 4 project (`Core`, `Data`, `Web`, `WinApp`) đã hoàn thiện và liên kết chính xác.
- **Hosting Stable:** WinForms đã host thành công Kestrel Server, quản lý vòng đời Start/Stop ổn định.
- **UI Integration:** WebView2 đã hiển thị được trang chủ từ `localhost:5000` và có cơ chế Retry khi server chưa sẵn sàng.
- **Database:** EF Core đã được cấu hình với PostgreSQL và có cơ chế Auto-Migration khi khởi động.
- **Documentation:** Hệ thống Memory Bank đã được thiết lập đầy đủ để hỗ trợ phát triển.
- **Code Cleanup:** Đã xóa folder không dùng `DATMOS.Web/Areas/Admin/Views/Shared/Components` (chứa placeholder AdminMenu) và `DATMOS.Web/Areas/Admin/Components` (chứa AdminMenuViewComponent) để đơn giản hóa cấu trúc. Đã cập nhật `_AdminLayout.cshtml` để sử dụng partial menu thay vì ViewComponent.

## Các bước tiếp theo (Next Steps)
1. **Business Logic:** Phát triển các Controller và View cụ thể cho nghiệp vụ (ví dụ: Quản lý sản phẩm, Đơn hàng).
   - Đã tạo **CoursesController** (đổi tên từ CourseController) với dữ liệu mẫu MOS Word 2019, Excel 2019, PowerPoint 2019
   - Đã tạo View **Courses/Index.cshtml** với giao diện quản lý khóa học đầy đủ
   - Đã tạo CSS tùy chỉnh **admin-courses.css** và JavaScript **admin-courses.js**
   - Đã cập nhật menu admin để sử dụng controller "Courses"
2. **Native Interop:** Thiết lập cơ chế giao tiếp 2 chiều:
   - *C# -> JS:* `ExecuteScriptAsync`
   - *JS -> C#:* `WebMessageReceived` (để gọi các tính năng native như in ấn, đọc file hệ thống).
3. **Deployment:** Cấu hình Publish profile để đóng gói thành Single File `.exe` và ẩn cửa sổ Console (sử dụng `Output Type: Windows Application` thay vì Console).
