# PHÂN TÍCH CẤU TRÚC HIỆN TẠI DATMOS.Web
*Ngày phân tích: 02/01/2026*
*Phục vụ cho Modularization Project*

## TỔNG QUAN

### Số lượng file trong Areas: 138 files

### Các Areas hiện có:
1. **Admin** - Quản trị hệ thống
2. **Customer** - Học viên/Khách hàng
3. **Identity** - Xác thực & Phân quyền
4. **Teacher** - Giáo viên/Giảng viên

## PHÂN TÍCH CHI TIẾT TỪNG AREA

### 1. AREA: ADMIN

#### Controllers:
- `AdminProductController.cs` - Quản lý sản phẩm
- `CoursesController.cs` - Quản lý khóa học
- `HomeController.cs` - Dashboard admin
- `LessonsController.cs` - Quản lý bài học
- `ProfileController.cs` - Profile admin
- `TeacherListController.cs` - Quản lý giáo viên
- `UsersController.cs` - Quản lý người dùng

#### Models:
- `InMemoryUserStore.cs` - Lưu trữ user trong memory

#### ViewModels:
- `AdminCourseViewModel.cs`
- `AdminUserViewModel.cs`
- `UserViewModel.cs`

#### Views Structure:
- `Views/Courses/` - Quản lý khóa học
- `Views/Dashboard/` - Dashboard
- `Views/Home/` - Trang chủ admin
- `Views/Lessons/` - Quản lý bài học
- `Views/Product/` - Quản lý sản phẩm
- `Views/Profile/` - Profile
- `Views/TeacherList/` - Danh sách giáo viên
- `Views/Users/` - Quản lý người dùng

#### Layouts:
- `_AdminLayout.cshtml` - Layout chính
- `_AdminBlankLayout.cshtml` - Layout trống

#### Shared Components:
- `_Partials/_AdminMacros.cshtml` - Macros admin
- `Sections/Footer/` - Footer admin
- `Sections/Menu/` - Menu admin

### 2. AREA: CUSTOMER

#### Controllers:
- `CoursesController.cs` - Khóa học cho học viên
- `ExamListController.cs` - Danh sách bài thi
- `ExamSubjectController.cs` - Môn thi
- `HomeController.cs` - Trang chủ học viên
- `LessonController.cs` - Bài học
- `ProductController.cs` - Sản phẩm
- `ProfileController.cs` - Profile học viên
- `Word2019ResultsController.cs` - Kết quả Word 2019

#### Models:
- `TrainingResult.cs` - Kết quả training

#### ViewModels (Phức tạp nhất):
- `CourseViewModel.cs`
- `CustomerMenuItemViewModel.cs`
- `ExamHierarchyViewModel.cs`
- `ExamListViewModel.cs`
- `ExamProjectViewModel.cs`
- `ExamSubjectDetailsViewModel.cs`
- `ExamSubjectViewModel.cs`
- `ExamTakingViewModel.cs`
- `ExamTaskViewModel.cs`
- `LessonViewModel.cs`
- `SubjectExamSummaryViewModel.cs`
- `UserProgressViewModel.cs`

#### Views Structure:
- `Views/Courses/` - Khóa học
- `Views/ExamList/` - Danh sách bài thi
- `Views/ExamSubject/` - Môn thi
- `Views/Home/` - Trang chủ
- `Views/Lesson/` - Bài học
- `Views/Product/` - Sản phẩm (Details, Index, Practice, Project, Test)
- `Views/Profile/` - Profile
- `Views/Word2019Results/` - Kết quả Word 2019

#### Layouts:
- `_CustomerLayout.cshtml` - Layout chính
- `_GMetrixLayout.cshtml.backup` - Layout GMetrix (backup)
- `_Word2019ResultsLayout.cshtml` - Layout kết quả
- `_Word2019TrainingLayout.cshtml` - Layout training

### 3. AREA: IDENTITY

#### Controllers:
- `AccountController.cs` - Tài khoản (Login/Register)
- `BaseProfileController.cs` - Base profile controller

#### Services:
- `ProfileService.cs` - Service xử lý profile

#### ViewModels:
- `LoginViewModel.cs`
- `RegisterViewModel.cs`

#### Views Structure:
- `Views/Account/` - Đăng nhập/Đăng ký
- `Views/Profile/` - Profile
- `Views/Shared/Profile/` - Shared profile components

### 4. AREA: TEACHER

#### Controllers:
- `CoursesController.cs` - Khóa học giáo viên
- `ProfileController.cs` - Profile giáo viên

#### Views Structure:
- `Views/Courses/` - Khóa học
- `Views/Profile/` - Profile
- `Views/Shared/Sections/Navbar/` - Navbar giáo viên

#### Layouts:
- `_Layout.cshtml` - Layout giáo viên

## TÀI NGUYÊN TĨNH (wwwroot/areas)

### Admin Assets:
- `admin/css/` - 6 file CSS
  - `admin-content-components.css`
  - `admin-courses.css`
  - `admin-data-table.css`
  - `admin-lessons.css`
  - `admin-users.css`
  - `blank-layout.css`
- `admin/js/` - 5 file JS
  - `admin-content-components.js`
  - `admin-courses.js`
  - `admin-lessons.js`
  - `admin-users.js`
  - `blank-layout.js`

### Customer Assets:
- `customer/css/` - 4 file CSS
  - `product-details.css`
  - `product-practice.css`
  - `word2019-results.css`
  - `word2019-training.css`
- `customer/js/` - 5 file JS
  - `product-common.js`
  - `product-practice.js`
  - `product-test.js`
  - `word2019-results.js`
  - `word2019-training.js`
- `customer/json/` - 15 file JSON
  - Các file dữ liệu: courses, menu, students, grading solutions, exam definitions, etc.

### Identity Assets:
- `identity/css/` - 2 file CSS
  - `login.css`
  - `register.css`
- `identity/js/` - 2 file JS
  - `login.js`
  - `register.js`

## SHARED COMPONENTS HIỆN TẠI

### ViewComponents (trong DATMOS.Web):
- `FooterViewComponent.cs` - Footer toàn hệ thống
- `MenuViewComponent.cs` - Menu toàn hệ thống
- `NavbarViewComponent.cs` - Navbar toàn hệ thống

### Services (có thể shared):
- Các services trong `DATMOS.Web/Services/`
- Các interfaces trong `DATMOS.Web/Interfaces/`

## DEPENDENCIES PHÂN TÍCH

### Cross-Area Dependencies:
1. **Identity → All**: Authentication được sử dụng bởi tất cả Areas
2. **Admin → Customer**: Quản lý users/courses liên quan đến customer data
3. **Customer → Identity**: Authentication cho customer access
4. **Teacher → Customer**: Chia sẻ courses data

### Database Dependencies:
- Tất cả Areas sử dụng `AppDbContext` từ `DATMOS.Data`
- Shared entities từ `DATMOS.Core`

### Service Dependencies:
- Các services được đăng ký trong `Program.cs`
- Một số services được sử dụng bởi nhiều Areas

## KIẾN TRÚC HIỆN TẠI - VẤN ĐỀ

### Ưu điểm:
- Tổ chức theo Areas rõ ràng
- Separation of concerns cơ bản
- Có sẵn shared components

### Nhược điểm:
- Tight coupling giữa các Areas
- Khó tái sử dụng code giữa các projects
- Khó phát triển độc lập
- Build time tăng khi project lớn
- Khó thay thế/tích hợp module mới

## ĐỀ XUẤT CHO MODULARIZATION

### High Priority (Di chuyển đầu tiên):
1. **Identity Area** - Vì được sử dụng bởi tất cả Areas khác
2. **Shared Components** - ViewComponents cần được di chuyển vào Theme

### Medium Priority:
3. **Admin Area** - Business logic độc lập tương đối
4. **Customer Area** - Phức tạp nhất, cần phân tích kỹ

### Low Priority:
5. **Teacher Area** - Đơn giản nhất, có thể tích hợp sau

### Theme Components cần tạo:
1. Base Layout với placeholders
2. Shared ViewComponents (Navbar, Menu, Footer)
3. Global CSS/JS framework
4. Authentication/Authorization infrastructure

## NEXT STEPS CHO GIAI ĐOẠN 1

1. **Tạo DATMOS.Themes.Default project**
2. **Di chuyển shared layouts và components vào Theme**
3. **Cập nhật các Areas sử dụng Theme**
4. **Testing cross-Area consistency**

---

*Phân tích này sẽ được cập nhật khi có thêm thông tin trong quá trình thực hiện.*
