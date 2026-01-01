# Profile System - DATMOS

## Overview
Hệ thống Profile được thiết kế để quản lý thông tin tài khoản cho 3 loại người dùng:
- **Admin** - Quản trị viên
- **Customer** - Học viên  
- **Teacher** - Giáo viên

Hệ thống sử dụng mô hình MVC Core với thiết kế tái sử dụng cao, cho phép chia sẻ code giữa các areas.

## Architecture

### 1. Database Layer
- **Entity**: `AddUsers` trong `DATMOS.Core.Entities.Identity`
- **Fields**: Đã được mở rộng với các trường thông tin cá nhân, địa chỉ, tùy chọn ngôn ngữ, múi giờ, tiền tệ
- **Migration**: Cần chạy migration để cập nhật database với các fields mới

### 2. Service Layer
- **Interface**: `IProfileService` trong `DATMOS.Core.Interfaces`
- **Implementation**: `ProfileService` trong `DATMOS.Web.Areas.Identity.Services`
- **Features**:
  - Quản lý thông tin người dùng
  - Upload và xóa ảnh đại diện
  - Thay đổi mật khẩu
  - Cache để tối ưu hiệu suất
  - Validation file upload

### 3. Controller Layer
- **Base Controller**: `BaseProfileController` trong `DATMOS.Web.Areas.Identity.Controllers`
- **Area Controllers**:
  - `Admin/ProfileController`
  - `Customer/ProfileController` 
  - `Teacher/ProfileController`
- **Design Pattern**: Sử dụng inheritance để chia sẻ logic chung

### 4. View Layer
- **Shared Views**: Trong `Areas/Identity/Views/Shared/Profile/`
  - `_ProfileTabs.cshtml` - Navigation tabs
  - `_AccountTab.cshtml` - Tab thông tin tài khoản
  - `_SecurityTab.cshtml` - Tab bảo mật
- **Main View**: `Index.cshtml` trong `Areas/Identity/Views/Profile/`

### 5. ViewModel
- **UserProfileViewModel**: Chứa tất cả fields từ `AddUsers` với validation attributes

### 6. Utilities
- **FileValidationHelper**: Validation và xử lý file upload an toàn

## Features

### 1. Thông tin tài khoản
- Thông tin cá nhân (họ, tên, tên hiển thị)
- Thông tin liên hệ (email, số điện thoại)
- Địa chỉ (địa chỉ, thành phố, tỉnh, mã bưu điện, quốc gia)
- Tùy chọn (ngôn ngữ, múi giờ, tiền tệ)
- Thông tin nghề nghiệp (tổ chức, chức vụ, phòng ban)
- Tiểu sử và liên kết mạng xã hội

### 2. Ảnh đại diện
- Upload ảnh (JPG, PNG, GIF, BMP, WEBP)
- Kích thước tối đa: 5MB
- Validation an toàn (kiểm tra header file)
- Tự động xóa ảnh cũ khi upload ảnh mới
- Xóa ảnh đại diện

### 3. Bảo mật
- Thay đổi mật khẩu
- Validation mật khẩu mạnh
- Hiển thị/ẩn mật khẩu
- Mẹo bảo mật

### 4. Tabs Navigation
- Tài khoản
- Bảo mật
- Thanh toán (placeholder)
- Thông báo (placeholder)
- Kết nối (placeholder)

## Setup & Configuration

### 1. Database Migration
```bash
dotnet ef migrations add UpdateUserProfileFields
dotnet ef database update
```

### 2. File Upload Configuration
- Thư mục uploads: `wwwroot/uploads/profiles/`
- Static files được phục vụ tự động qua `app.UseStaticFiles()`
- File `.gitignore` để tránh commit file upload

### 3. Dependency Injection
Profile service đã được đăng ký trong `WebEntryPoint.cs`:
```csharp
services.AddScoped<DATMOS.Core.Interfaces.IProfileService, DATMOS.Web.Areas.Identity.Services.ProfileService>();
```

## Security Considerations

### 1. File Upload Security
- Validation file extension và MIME type
- Kiểm tra kích thước file (max 5MB)
- Kiểm tra header file để đảm bảo là ảnh hợp lệ
- Tạo tên file an toàn (không chứa path traversal)
- Xóa file cũ khi upload file mới

### 2. Input Validation
- Validation attributes trong ViewModel
- Server-side validation trong controller
- Client-side validation với JavaScript

### 3. Authorization
- Mỗi area có authorization riêng:
  - Admin: `[Authorize(Roles = "Admin")]`
  - Customer: `[Authorize(Roles = "Customer")]`
  - Teacher: `[Authorize(Roles = "Teacher")]`

## Customization per Area

### Admin Area
- Thêm thông tin quản trị
- Thống kê hệ thống
- Quyền quản lý người dùng, khóa học, nội dung

### Customer Area  
- Thông tin khóa học đã đăng ký
- Tiến độ học tập
- Kết quả thi

### Teacher Area
- Thông tin khóa học đã tạo
- Số lượng học viên
- Giờ giảng dạy

## Testing

### 1. Unit Tests (TODO)
- Test ProfileService methods
- Test FileValidationHelper
- Test Controller actions

### 2. Integration Tests (TODO)
- Test file upload flow
- Test profile update flow
- Test password change flow

### 3. Manual Testing
1. Đăng nhập với từng loại người dùng
2. Truy cập trang profile
3. Cập nhật thông tin tài khoản
4. Upload ảnh đại diện
5. Thay đổi mật khẩu
6. Kiểm tra navigation giữa các tabs

## Future Enhancements

### 1. Two-Factor Authentication
- Thêm support cho 2FA
- QR code generation
- Backup codes

### 2. Session Management
- Xem và quản lý các phiên đăng nhập
- Đăng xuất từ xa

### 3. Email & Phone Verification
- Xác thực email
- Xác thực số điện thoại

### 4. Social Media Integration
- Kết nối với Facebook, Google, GitHub
- Import thông tin từ social media

### 5. Advanced Preferences
- Theme customization
- Notification preferences
- Privacy settings

## Troubleshooting

### 1. File Upload Issues
- Kiểm tra quyền ghi thư mục `wwwroot/uploads/profiles/`
- Kiểm tra kích thước file (max 5MB)
- Kiểm tra định dạng file (chỉ ảnh)

### 2. Database Issues
- Chạy migration để cập nhật fields
- Kiểm tra connection string
- Kiểm tra PostgreSQL service

### 3. Cache Issues
- Clear cache nếu thông tin không cập nhật
- Kiểm tra cache duration (30 phút)

## Code Examples

### Update User Profile
```csharp
var model = new UserProfileViewModel
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    // ... other properties
};

var success = await _profileService.UpdateUserProfileAsync(userId, model);
```

### Upload Profile Picture
```csharp
var imageUrl = await _profileService.UploadProfilePictureAsync(userId, file);
```

### Change Password
```csharp
var success = await _profileService.ChangePasswordAsync(userId, currentPassword, newPassword);
```

## Conclusion
Hệ thống Profile được thiết kế với kiến trúc modular, dễ bảo trì và mở rộng. Sử dụng inheritance và shared views để tối ưu code reuse giữa các areas. Hỗ trợ đầy đủ các tính năng quản lý thông tin người dùng với bảo mật cao.
