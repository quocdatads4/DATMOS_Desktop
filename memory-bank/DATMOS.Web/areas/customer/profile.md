# Memory Bank: Profile Management

## Tổng quan

Hệ thống quản lý profile người dùng đã được cải thiện đáng kể với các sửa đổi tập trung vào:
1. Loại bỏ redundancy (AvatarUrl) và chuẩn hóa sử dụng ProfilePictureUrl
2. Sửa lỗi không lưu được Address, PhoneNumber và các custom properties vào database
3. Cải thiện độ tin cậy của việc cập nhật thông tin người dùng

## Vấn đề 1: Loại bỏ AvatarUrl - Chuẩn hóa sử dụng ProfilePictureUrl (Đã xử lý)

### Mô tả
Hệ thống có 2 property cho ảnh đại diện: `AvatarUrl` (giá trị mặc định) và `ProfilePictureUrl` (URL thực tế từ upload). Điều này gây redundancy và khó bảo trì.

### Giải pháp đã thực hiện
1. **Xóa property AvatarUrl từ UserProfileViewModel.cs**
   - Property `public string AvatarUrl { get; set; } = "/img/avatars/1.png";` đã bị xóa
   - Chỉ giữ lại `public string? ProfilePictureUrl { get; set; }`

2. **Cập nhật file view sử dụng AvatarUrl:**
   - `DATMOS.Web/Views/Shared/Components/Navbar/Partials/_UserDropdown.cshtml`:
     - Thay `@Model.AvatarUrl` bằng `@(Model.ProfilePictureUrl ?? "/img/avatars/1.png")`
     - Đảm bảo giá trị mặc định khi ProfilePictureUrl null

3. **Cập nhật NavbarViewComponent.cs:**
   - Thay `AvatarUrl = user.ProfilePictureUrl ?? "/img/avatars/1.png"`
   - Bằng `ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatars/1.png"`

### Kết quả
- Codebase chỉ sử dụng một nguồn duy nhất cho ảnh đại diện: `ProfilePictureUrl`
- Giảm redundancy, dễ bảo trì và nhất quán
- UI vẫn hiển thị ảnh đại diện đúng với giá trị mặc định khi cần

## Vấn đề 2: Address, PhoneNumber không được cập nhật database (Đã xử lý)

### Mô tả
Khi người dùng nhấn nút Lưu trong form cập nhật profile, các field như Address, PhoneNumber, City, State, ZipCode, Country và các custom properties khác không được lưu vào database.

### Nguyên nhân
- **Method `UpdateUserProfileAsync` trong `ProfileService.cs`** sử dụng `_userManager.UpdateAsync(user)`
- **`UserManager.UpdateAsync` có vấn đề với custom properties:** Không detect được thay đổi trên các property custom như Address, City, State, ZipCode, Country, Organization, JobTitle, v.v.
- Có thể không detect được thay đổi trên PhoneNumber (mặc dù là property của IdentityUser)

### Giải pháp đã thực hiện
1. **Sửa method `UpdateUserProfileAsync` trong `ProfileService.cs`:**
   - Thay thế `_userManager.UpdateAsync(user)` bằng logic sử dụng DbContext
   - Sử dụng pattern: check tracked entity, attach nếu cần, mark as Modified, gọi `SaveChangesAsync`

2. **Sửa các method khác sử dụng `_userManager.UpdateAsync`:**
   - `UpdateUserPreferencesAsync` - cập nhật Language, Timezone, Currency
   - `UpdateLastLoginAsync` - cập nhật thời gian đăng nhập cuối
   - `UpdateLastProfileUpdateAsync` - cập nhật thời gian cập nhật profile
   - `UpdateUserRoleAsync` - cập nhật vai trò người dùng

### Code mẫu (ProfileService.cs - UpdateUserProfileAsync)
```csharp
// Use DbContext to save changes for custom properties
// UserManager.UpdateAsync may not detect changes to custom properties
try
{
    // Check if the entity is already being tracked
    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
    if (trackedUser != null)
    {
        // If already tracked, update the tracked entity
        _context.Entry(trackedUser).CurrentValues.SetValues(user);
    }
    else
    {
        // If not tracked, attach and mark as modified
        _context.Attach(user);
        _context.Entry(user).State = EntityState.Modified;
    }
    
    // Save changes to database
    var rowsAffected = await _context.SaveChangesAsync();
    
    if (rowsAffected > 0)
    {
        ClearUserCache(userId);
        _logger.LogInformation("User profile updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
            userId, rowsAffected);
        return true;
    }
    else
    {
        _logger.LogWarning("No rows affected when updating user profile for user {UserId}", userId);
        return false;
    }
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error saving user profile to database for user {UserId}", userId);
    return false;
}
```

### Kết quả
- Address, PhoneNumber và tất cả các field trong user profile được cập nhật đúng vào database
- Code nhất quán: tất cả các method update đều sử dụng cùng pattern với DbContext
- Không có breaking changes, chỉ thay đổi logic lưu database

## Vấn đề 3: Mất ảnh đại diện khi cập nhật thông tin tài khoản (Đã xử lý)

### Mô tả
Khi người dùng cập nhật thông tin cá nhân (ví dụ: Tên, Số điện thoại) mà không tải lên ảnh đại diện mới, đường dẫn ảnh đại diện (`ProfilePictureUrl`) trong cơ sở dữ liệu bị cập nhật thành `null`.

### Giải pháp
Trong `BaseProfileController.UpdateAccount`:
- Khi không có file ảnh mới: giữ lại đường dẫn ảnh cũ bằng cách lấy thông tin từ đối tượng `currentUser` đã được truy vấn từ database
- Code: `model.ProfilePictureUrl = currentUser.ProfilePictureUrl;`

## Tổng kết các thay đổi

### Files đã sửa:
1. **`DATMOS.Web/ViewModels/UserProfileViewModel.cs`**
   - Xóa property `AvatarUrl`

2. **`DATMOS.Web/Views/Shared/Components/Navbar/Partials/_UserDropdown.cshtml`**
   - Thay `@Model.AvatarUrl` bằng `@(Model.ProfilePictureUrl ?? "/img/avatars/1.png")`

3. **`DATMOS.Web/ViewComponents/NavbarViewComponent.cs`**
   - Thay `AvatarUrl` bằng `ProfilePictureUrl`

4. **`DATMOS.Web/Areas/Identity/Services/ProfileService.cs`**
   - Sửa `UpdateUserProfileAsync` - dùng DbContext thay vì UserManager
   - Sửa `UpdateUserPreferencesAsync` - dùng DbContext
   - Sửa `UpdateLastLoginAsync` - dùng DbContext
   - Sửa `UpdateLastProfileUpdateAsync` - dùng DbContext
   - Sửa `UpdateUserRoleAsync` - dùng DbContext
   - `UploadProfilePictureAsync` và `DeleteProfilePictureAsync` đã sửa trước đó

5. **`DATMOS.Web/Areas/Identity/Controllers/BaseProfileController.cs`**
   - Giữ lại đường dẫn ảnh cũ khi không có upload mới

### Lợi ích:
- **Giảm redundancy:** Chỉ một property cho ảnh đại diện
- **Độ tin cậy cao:** Tất cả thay đổi được lưu đúng vào database
- **Nhất quán:** Tất cả method update sử dụng cùng pattern
- **Dễ bảo trì:** Code rõ ràng, dễ hiểu

### Kiểm tra:
- Không còn references nào đến `AvatarUrl` trong toàn bộ codebase
- Các field Address, PhoneNumber, và custom properties được lưu đúng vào database
- Ảnh đại diện hiển thị đúng với giá trị mặc định khi cần
