# DATMOS.Web Memory Bank

## Giới thiệu
Memory bank này chứa documentation chi tiết về DATMOS.Web - ứng dụng web ASP.NET Core MVC trong DATMOS_Desktop solution. Documentation được tổ chức theo cấu trúc mirror của project thực tế.

## Cấu trúc Documentation

### 1. Tổng quan
- **[overview.md](overview.md)** - Tổng quan ứng dụng, kiến trúc, công nghệ
- **[architecture.md](architecture.md)** - Kiến trúc hệ thống, design patterns, flow
- **[dependencies.md](dependencies.md)** - Dependencies, packages, compatibility

### 2. Kiến trúc Menu
- **[menu.md](menu.md)** - Kiến trúc menu động tập trung (đã có)

### 3. Areas
- **Admin Area**
  - [areas/admin/controllers.md](areas/admin/controllers.md) - Admin controllers documentation
  - *Thêm documentation cho views, models khi cần*

- **Customer Area**
  - *Documentation cho Customer area (cần thêm)*

### 4. Core Components
- **[services.md](services.md)** - Services layer, business logic
- *Thêm documentation cho controllers, models, views khi cần*

## Quick Links

### Kiến trúc chính
- [Tổng quan ứng dụng](overview.md)
- [Kiến trúc hệ thống](architecture.md)
- [Services Layer](services.md)

### Menu System
- [Kiến trúc Menu động](menu.md)

### Admin Area
- [Admin Controllers](areas/admin/controllers.md)

### Dependencies
- [Dependencies & Packages](dependencies.md)

## Cập nhật Documentation

### 1. Khi thay đổi code
1. Cập nhật documentation liên quan
2. Update version và last updated date
3. Kiểm tra tính chính xác

### 2. Khi thêm tính năng mới
1. Tạo documentation mới
2. Update index này
3. Link đến documentation liên quan

### 3. Quy ước versioning
- **Major version**: Thay đổi lớn, breaking changes
- **Minor version**: Thêm tính năng mới
- **Patch version**: Sửa lỗi, cập nhật nhỏ

## Sử dụng Memory Bank

### 1. Cho developer mới
1. Đọc [overview.md](overview.md) để hiểu tổng quan
2. Xem [architecture.md](architecture.md) để hiểu kiến trúc
3. Tham khảo [menu.md](menu.md) cho navigation system

### 2. Cho maintenance
1. Sử dụng [dependencies.md](dependencies.md) để quản lý packages
2. Tham khảo [services.md](services.md) để hiểu business logic
3. Xem area-specific docs cho module cụ thể

### 3. Cho development
1. Follow patterns documented trong [architecture.md](architecture.md)
2. Sử dụng services structure từ [services.md](services.md)
3. Tuân thủ conventions từ các docs

## Liên kết với Projects khác

### DATMOS.Core
- Domain entities và interfaces
- Shared business objects

### DATMOS.Data
- Data access layer
- Database context và migrations

### DATMOS.WinApp
- Desktop host application
- WebView2 integration

## Contributing

### 1. Documentation Guidelines
- Sử dụng tiếng Việt cho documentation
- Bao gồm code examples khi cần
- Cập nhật last updated date
- Sử dụng markdown formatting

### 2. Structure Guidelines
- Mirror cấu trúc project thực tế
- Tạo index cho mỗi thư mục
- Link related documents

### 3. Maintenance
- Regular review và updates
- Remove outdated information
- Keep consistent với codebase

---
*Memory Bank version: 1.0 - Last updated: 2025-12-23*
