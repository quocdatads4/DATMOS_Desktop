# Product Context

## Bối cảnh & Vấn đề
Các ứng dụng doanh nghiệp truyền thống trên Windows Forms (WinForms) thường gặp hạn chế về khả năng tùy biến giao diện (UI) và trải nghiệm người dùng (UX) so với các ứng dụng Web hiện đại. Tuy nhiên, ứng dụng Web thuần túy lại thiếu khả năng tương tác sâu với hệ thống (File System, Hardware) và yêu cầu kết nối mạng hoặc cấu hình Server phức tạp.

## Giải pháp: DATMOS_Desktop
DATMOS_Desktop là giải pháp lai (Hybrid), mang lại những ưu điểm tốt nhất của cả hai thế giới:
1.  **Sức mạnh của Desktop:** Khả năng truy cập hệ thống, chạy offline, chạy nền (System Tray), hiệu năng cao của .NET 8.
2.  **Vẻ đẹp của Web:** Giao diện linh hoạt, responsive, dễ dàng phát triển với HTML/CSS/JS thông qua ASP.NET Core MVC.

## Trải nghiệm người dùng (User Experience)
- **Khởi động:** Người dùng mở ứng dụng như một file `.exe` bình thường. Không cần cài đặt IIS hay cấu hình Database phức tạp.
- **Sử dụng:** Giao diện mượt mà, hiện đại như Web App nhưng phản hồi tức thì (do Server chạy local in-process).
- **Đóng ứng dụng:** Khi tắt cửa sổ, ứng dụng dọn dẹp tài nguyên sạch sẽ hoặc thu nhỏ xuống khay hệ thống tùy theo cấu hình.