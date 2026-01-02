# Kế hoạch Modular hóa DATMOS.Web thành Theme + Plugin Architecture

**Mục tiêu:** Tái cấu trúc project `DATMOS.Web` theo kiến trúc nopCommerce với Theme System và Plugin Modules (Razor Class Libraries). Tăng tính module, dễ bảo trì, phát triển và tái sử dụng.

## KIẾN TRÚC TỔNG QUAN

### 1. Theme System (DATMOS.Themes.Default)
- Chứa tất cả layouts, shared components, global styles
- Cung cấp base layout cho tất cả modules
- Dễ dàng thay thế/upgrade theme

### 2. Plugin Modules (Razor Class Libraries)
- `DATMOS.Modules.Admin` - Quản trị hệ thống
- `DATMOS.Modules.Customer` - Học viên/khách hàng  
- `DATMOS.Modules.Identity` - Xác thực & phân quyền
- `DATMOS.Modules.Teacher` - Giáo viên/giảng viên

### 3. Core Infrastructure
- `DATMOS.Core` - Entities, interfaces
- `DATMOS.Data` - Database context, migrations
- `DATMOS.Web` - Host application

---

## KẾ HOẠCH THỰC HIỆN CHI TIẾT VỚI CHECKLIST

### 📋 GIAI ĐOẠN 0: PHÂN TÍCH & CHUẨN BỊ (Ước tính: 3 giờ)

- [ ] **Phân tích chi tiết cấu trúc hiện tại**
  - [ ] Liệt kê tất cả Controllers, Views, ViewModels, Services theo từng Area
  - [ ] Xác định shared components và dependencies chéo
  - [ ] Phân tích layouts hiện có và xác định base layout

- [ ] **Thiết kế kiến trúc Theme + Plugin**
  - [ ] Thiết kế cấu trúc project cho Theme
  - [ ] Thiết kế interface giữa Theme và Modules
  - [ ] Xác định shared infrastructure requirements

- [ ] **Chuẩn bị development environment**
  - [ ] Backup toàn bộ code hiện tại
  - [ ] Tạo branch mới cho modularization
  - [ ] Setup development tools và templates

### 🎨 GIAI ĐOẠN 1: TẠO THEME SYSTEM (Ước tính: 6 giờ)

- [ ] **Tạo project DATMOS.Themes.Default**
  ```bash
  dotnet new rcl -n DATMOS.Themes.Default
  ```

- [ ] **Cấu trúc Theme project**
  - [ ] Tạo thư mục `Views/Shared/` với base layouts
  - [ ] Tạo thư mục `Views/Components/` cho shared components
  - [ ] Tạo thư mục `wwwroot/` cho global assets
  - [ ] Cấu hình `.csproj` cho static web assets

- [ ] **Di chuyển shared layouts và components**
  - [ ] Di chuyển `_AdminLayout.cshtml`, `_CustomerLayout.cshtml` vào Theme
  - [ ] Di chuyển Navbar, Menu, Footer ViewComponents vào Theme
  - [ ] Di chuyển global CSS/JS files vào Theme
  - [ ] Tạo base layout với placeholders cho modules

- [ ] **Cập nhật các Areas hiện tại sử dụng Theme**
  - [ ] Cập nhật `_ViewImports.cshtml` trong các Areas
  - [ ] Cập nhật layout references trong Views
  - [ ] Testing cross-module consistency

### 🔌 GIAI ĐOẠN 2: TẠO PLUGIN TEMPLATE & INFRASTRUCTURE (Ước tính: 4 giờ)

- [ ] **Tạo plugin project template**
  ```xml
  <!-- DATMOS.Modules.Template.csproj -->
  <Project Sdk="Microsoft.NET.Sdk.Razor">
    <PropertyGroup>
      <TargetFramework>net9.0</TargetFramework>
      <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
    </PropertyGroup>
  </Project>
  ```

- [ ] **Tạo shared infrastructure cho plugins**
  - [ ] `IPlugin` interface và `BasePlugin` class
  - [ ] `PluginManager` service cho dynamic loading
  - [ ] Dependency injection extension methods
  - [ ] Route registration utilities

- [ ] **Tạo build automation scripts**
  - [ ] Script tạo plugin mới từ template
  - [ ] Script build và test tất cả plugins
  - [ ] Script deploy plugins độc lập

### 🏗️ GIAI ĐOẠN 3: TRIỂN KHAI TỪNG MODULE (Ước tính: 18 giờ)

#### Module 1: DATMOS.Modules.Identity (Ưu tiên - 5 giờ)
- [ ] **Tạo project và cấu trúc**
  ```bash
  dotnet new rcl -n DATMOS.Modules.Identity
  ```

- [ ] **Di chuyển Identity components**
  - [ ] Di chuyển `Areas/Identity/Controllers/` → `DATMOS.Modules.Identity/Controllers/`
  - [ ] Di chuyển `Areas/Identity/Views/` → `DATMOS.Modules.Identity/Views/`
  - [ ] Di chuyển `Areas/Identity/Services/` → `DATMOS.Modules.Identity/Services/`
  - [ ] Di chuyển `wwwroot/areas/identity/` → `DATMOS.Modules.Identity/wwwroot/`

- [ ] **Cập nhật dependencies và namespaces**
  - [ ] Cập nhật namespace cho tất cả files
  - [ ] Cấu hình dependency injection
  - [ ] Đăng ký routes và services

- [ ] **Testing và validation**
  - [ ] Unit tests cho Identity module
  - [ ] Integration tests với Theme
  - [ ] End-to-end testing authentication flow

#### Module 2: DATMOS.Modules.Admin (4 giờ)
- [ ] **Tạo project và cấu trúc**
- [ ] **Di chuyển Admin components**
  - [ ] Controllers: AdminProduct, Courses, Lessons, Users, etc.
  - [ ] Views và ViewModels
  - [ ] Admin-specific services
  - [ ] Admin assets từ `wwwroot/areas/admin/`

- [ ] **Cập nhật dependencies**
- [ ] **Testing admin functionality**

#### Module 3: DATMOS.Modules.Customer (5 giờ)
- [ ] **Tạo project và cấu trúc**
- [ ] **Di chuyển Customer components**
  - [ ] Controllers: Courses, ExamList, ExamSubject, Product, etc.
  - [ ] Views và ViewModels (phức tạp nhất)
  - [ ] Customer-specific services
  - [ ] Customer assets từ `wwwroot/areas/customer/`

- [ ] **Xử lý phức tạp**
  - [ ] Exam system integration
  - [ ] Product/practice/test flows
  - [ ] Grading system dependencies

- [ ] **Testing customer flows**

#### Module 4: DATMOS.Modules.Teacher (4 giờ)
- [ ] **Tạo project và cấu trúc**
- [ ] **Di chuyển Teacher components**
- [ ] **Cập nhật dependencies**
- [ ] **Testing teacher functionality**

### 🔗 GIAI ĐOẠN 4: TÍCH HỢP VÀ CONFIGURATION (Ước tính: 5 giờ)

- [ ] **Cập nhật DATMOS.Web.csproj**
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\DATMOS.Themes.Default\DATMOS.Themes.Default.csproj" />
    <ProjectReference Include="..\DATMOS.Modules.Admin\DATMOS.Modules.Admin.csproj" />
    <ProjectReference Include="..\DATMOS.Modules.Customer\DATMOS.Modules.Customer.csproj" />
    <ProjectReference Include="..\DATMOS.Modules.Identity\DATMOS.Modules.Identity.csproj" />
    <ProjectReference Include="..\DATMOS.Modules.Teacher\DATMOS.Modules.Teacher.csproj" />
  </ItemGroup>
  ```

- [ ] **Cập nhật Program.cs**
  - [ ] Đăng ký Theme services và middleware
  - [ ] Đăng ký Plugin modules dynamic loading
  - [ ] Cấu hình static files từ plugins
  - [ ] Cập nhật routing configuration

- [ ] **Cấu hình build pipeline**
  - [ ] Multi-project build configuration
  - [ ] Asset bundling và minification
  - [ ] Deployment configuration

### 🧪 GIAI ĐOẠN 5: TESTING COMPREHENSIVE (Ước tính: 8 giờ)

- [ ] **Unit Testing**
  - [ ] Test tất cả controllers trong mỗi module
  - [ ] Test services và business logic
  - [ ] Test ViewModels và validation

- [ ] **Integration Testing**
  - [ ] Test module-to-module communication
  - [ ] Test Theme với tất cả modules
  - [ ] Test database interactions

- [ ] **End-to-End Testing**
  - [ ] Test toàn bộ user flows
  - [ ] Test authentication/authorization
  - [ ] Test exam system từ đầu đến cuối

- [ ] **Performance Testing**
  - [ ] Test load time với module architecture
  - [ ] Test memory usage
  - [ ] Test startup time

- [ ] **Cross-browser Testing**
  - [ ] Test trên Chrome, Firefox, Edge
  - [ ] Test responsive design
  - [ ] Test mobile compatibility

### 📚 GIAI ĐOẠN 6: DOCUMENTATION & OPTIMIZATION (Ước tính: 4 giờ)

- [ ] **Tạo documentation**
  - [ ] Module development guide
  - [ ] Theme customization guide
  - [ ] API documentation cho plugins
  - [ ] Deployment guide

- [ ] **Tối ưu hóa**
  - [ ] Code cleanup và refactoring
  - [ ] Performance optimization
  - [ ] Bundle size optimization

- [ ] **Final validation**
  - [ ] Code review tất cả modules
  - [ ] Security audit
  - [ ] Accessibility testing

---

## 📊 TỔNG THỜI GIAN ƯỚC TÍNH

| Giai đoạn | Thời gian | Tổng tích lũy |
|-----------|-----------|---------------|
| Giai đoạn 0: Phân tích & Chuẩn bị | 3 giờ | 3 giờ |
| Giai đoạn 1: Tạo Theme System | 6 giờ | 9 giờ |
| Giai đoạn 2: Plugin Template & Infrastructure | 4 giờ | 13 giờ |
| Giai đoạn 3: Triển khai từng Module | 18 giờ | 31 giờ |
| Giai đoạn 4: Tích hợp & Configuration | 5 giờ | 36 giờ |
| Giai đoạn 5: Testing Comprehensive | 8 giờ | 44 giờ |
| Giai đoạn 6: Documentation & Optimization | 4 giờ | 48 giờ |

**Tổng: 48 giờ (6 ngày làm việc)**

---

## ⚠️ RỦI RO VÀ GIẢI PHÁP

### Rủi ro chính:
1. **Complex dependencies between modules**
   - **Giải pháp**: Use interface-based design, dependency inversion
   
2. **Shared state management**
   - **Giải pháp**: Implement event bus pattern, use MediatR
   
3. **Build time increase**
   - **Giải pháp**: Incremental builds, parallel compilation
   
4. **Database schema conflicts**
   - **Giải pháp**: Module-specific migrations, versioning

### Mitigation strategies:
- **Phased rollout**: Deploy modules one by one
- **Feature flags**: Enable/disable modules at runtime
- **Rollback plan**: Quick revert to monolithic if needed
- **Monitoring**: Real-time performance monitoring

---

## 🎯 TIÊU CHÍ THÀNH CÔNG

### Technical Success Criteria:
- [ ] Tất cả modules compile độc lập
- [ ] Theme có thể thay thế mà không ảnh hưởng modules
- [ ] Modules có thể enable/disable tại runtime
- [ ] Performance không giảm quá 10%
- [ ] Tất cả tests pass

### Business Success Criteria:
- [ ] Development velocity tăng 30%
- [ ] Bug rate giảm 20%
- [ ] Feature deployment time giảm 50%
- [ ] Team collaboration improved

---

## 📅 KẾ HOẠCH TRIỂN KHAI THEO TUẦN

**Tuần 1**: Giai đoạn 0-2 (Phân tích, Theme, Infrastructure)
**Tuần 2**: Giai đoạn 3 (Identity & Admin modules)
**Tuần 3**: Giai đoạn 3 (Customer & Teacher modules)
**Tuần 4**: Giai đoạn 4-5 (Tích hợp & Testing)
**Tuần 5**: Giai đoạn 6 (Documentation & Optimization)

---

## 🔄 MAINTENANCE CHECKLIST SAU KHI HOÀN THÀNH

### Hàng ngày:
- [ ] Monitor module performance
- [ ] Review error logs
- [ ] Check build status

### Hàng tuần:
- [ ] Update dependencies
- [ ] Run full test suite
- [ ] Review security patches

### Hàng tháng:
- [ ] Performance audit
- [ ] Code quality review
- [ ] Documentation update

---

*Kế hoạch này sẽ được cập nhật khi có thay đổi requirements hoặc phát hiện issues mới trong quá trình thực hiện.*

*Cập nhật lần cuối: 02/01/2026*
*Người tạo: Cline AI Assistant*
*Trạng thái: Đang chờ phê duyệt*
