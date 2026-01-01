# Memory Bank: Cấu trúc tích hợp Entity Framework cho Course

Tài liệu này mô tả cấu trúc hoàn chỉnh của module **Course** (Khóa học) trong ứng dụng DATMOS, được tích hợp với Entity Framework Core theo pattern của Menu.

## Tổng quan cấu trúc

Courses được tổ chức theo mô hình phân tầng rõ ràng:

```
Entity Layer (DATMOS.Core/Entities/Course.cs)
        ↓
Data Layer (DATMOS.Data/)
  ├── AppDbContext (DbSet<Course>)
  └── CourseSeeder (Dữ liệu mẫu)
        ↓
Service Layer (DATMOS.Web/Services/)
  └── CoursesService (Business logic + Caching)
        ↓
Controller Layer (Areas/)
  ├── Admin/Controllers/CoursesController (CRUD)
  ├── Teacher/Controllers/CoursesController (Quản lý lớp)
  └── Customer/Controllers/ (Hiển thị công khai)
        ↓
View Layer (Areas/*/Views/Courses/)
  ├── Admin/ (Giao diện quản trị)
  ├── Teacher/ (Giao diện giáo viên)
  └── Customer/ (Giao diện người học)
```

## 1. Entity Layer - Định nghĩa Course

### **File: `DATMOS.Core/Entities/Course.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace DATMOS.Core.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShortName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(50)]
        public string ColorClass { get; set; } = string.Empty;

        [StringLength(50)]
        public string Level { get; set; } = string.Empty;

        [StringLength(50)]
        public string Duration { get; set; } = string.Empty;

        public int TotalLessons { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }

        [StringLength(100)]
        public string Instructor { get; set; } = string.Empty;

        public double Rating { get; set; }
        public int EnrolledStudents { get; set; }

        // Để tương thích với hệ thống hiện tại
        public int SubjectId { get; set; }
    }
}
```

## 2. Data Layer - Database Integration

### **2.1 AppDbContext Integration**

**File: `DATMOS.Data/AppDbContext.cs`**
```csharp
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Course> Courses { get; set; }
    // ... other DbSets
}
```

### **2.2 Course Seeder**

**File: `DATMOS.Data/CourseSeeder.cs`**
- Tạo 3 khóa học MOS mẫu: Word 2019, Excel 2019, PowerPoint 2019
- Xử lý lỗi chi tiết và logging
- Sử dụng mã Code ngắn (6 ký tự) để phù hợp với database constraints

## 3. Service Layer - Business Logic

### **File: `DATMOS.Web/Services/CoursesService.cs`**

CoursesService cung cấp các chức năng:
- **Đọc từ database** thay vì file JSON
- **Caching** với IMemoryCache (5 phút)
- **Mapping** từ Course entity sang CourseViewModel
- **Business logic**: Tạo badge, syllabus dựa trên thông tin khóa học
- **Search và filtering**: Theo subject, level, keyword

**Key Features:**
- `GetAllCoursesAsync()`: Lấy tất cả khóa học với caching
- `GetCourseByIdAsync(id)`: Lấy chi tiết khóa học
- `GetCourseDetailsAsync(id)`: Lấy thông tin đầy đủ + syllabus + related courses
- `SearchCoursesAsync(keyword)`: Tìm kiếm khóa học
- `GetCourseStatisticsAsync()`: Thống kê tổng quan

## 4. Controller Layer - Phân chia theo Areas

### **4.1 Admin Area - Quản trị toàn hệ thống**

**File: `DATMOS.Web/Areas/Admin/Controllers/CoursesController.cs`**

**Chức năng:**
- **CRUD đầy đủ**: Create, Read, Update, Delete
- **ViewModels riêng**: `AdminCourseViewModel`, `AdminCourseCreateEditViewModel`
- **Database operations**: Sử dụng AppDbContext trực tiếp
- **Cache management**: Xóa cache sau khi thay đổi dữ liệu

**Routes:**
- `GET /Admin/Courses` - Danh sách khóa học
- `GET /Admin/Courses/Create` - Tạo mới
- `POST /Admin/Courses/Create` - Xử lý tạo mới
- `GET /Admin/Courses/Edit/{id}` - Chỉnh sửa
- `POST /Admin/Courses/Edit/{id}` - Xử lý chỉnh sửa
- `GET /Admin/Courses/Delete/{id}` - Xác nhận xóa
- `POST /Admin/Courses/Delete/{id}` - Xử lý xóa

### **4.2 Teacher Area - Quản lý lớp học**

**File: `DATMOS.Web/Areas/Teacher/Controllers/CoursesController.cs`**

**Chức năng:**
- **Xem khóa học đang dạy**: Filter theo giáo viên (cần mở rộng)
- **Quản lý học viên**: Danh sách, tiến độ, điểm số
- **Thống kê lớp học**: Enrollment trend, progress distribution
- **Theo dõi hoạt động**: Recent activities, student engagement

**ViewModels:**
- `TeacherCourseViewModel`: Thông tin cơ bản + tiến độ
- `TeacherCourseDetailsViewModel`: Chi tiết + học viên + thống kê
- `TeacherCourseAnalyticsViewModel`: Phân tích dữ liệu

**Routes:**
- `GET /Teacher/Courses` - Danh sách khóa học của giáo viên
- `GET /Teacher/Courses/Details/{id}` - Chi tiết khóa học
- `GET /Teacher/Courses/Students/{id}` - Danh sách học viên
- `GET /Teacher/Courses/Analytics/{id}` - Phân tích dữ liệu

### **4.3 Customer Area - Hiển thị công khai**

**Đã có sẵn trong hệ thống:**
- Sử dụng `CoursesService` hiện có
- ViewModels trong `DATMOS.Web/Areas/Customer/ViewModels/`
- Hiển thị danh sách khóa học, chi tiết, tìm kiếm

## 5. View Layer - Giao diện người dùng

### **5.1 Teacher Views**

**File: `DATMOS.Web/Areas/Teacher/Views/Courses/Index.cshtml`**
- Card-based layout hiển thị danh sách khóa học
- Progress bars, student counts, ratings
- Dropdown actions: Xem chi tiết, học viên, phân tích

**File: `DATMOS.Web/Areas/Teacher/Views/Courses/Details.cshtml`**
- Thông tin khóa học chi tiết
- Syllabus với trạng thái hoàn thành
- Thống kê: Tổng học viên, tiến độ TB, điểm TB
- Hoạt động gần đây
- Danh sách học viên tiêu biểu

### **5.2 Admin Views**

**Cần tạo (theo pattern của Menu):**
- `Index.cshtml`: Danh sách với CRUD actions
- `Create.cshtml`: Form tạo mới khóa học
- `Edit.cshtml`: Form chỉnh sửa
- `Details.cshtml`: Xem chi tiết
- `Delete.cshtml`: Xác nhận xóa

## 6. Tổ chức theo Pattern của Menu

Courses áp dụng pattern tương tự như Menu:

| Component | Menu Pattern | Course Implementation |
|-----------|--------------|----------------------|
| **Entity** | `Menu.cs` | `Course.cs` |
| **DbContext** | `DbSet<Menu>` | `DbSet<Course>` |
| **Seeder** | `MenuSeeder.cs` | `CourseSeeder.cs` |
| **Service** | `MenuService` | `CoursesService` |
| **Interface** | `IMenuService` | `ICoursesService` |
| **Admin Controller** | `Admin/MenusController` | `Admin/CoursesController` |
| **View Components** | `MenuViewComponent` | (Có thể thêm `CourseViewComponent`) |

## 7. Bài học kinh nghiệm

### **7.1 Database Design**
- **String length constraints**: Luôn sử dụng `[StringLength]` attribute
- **Code optimization**: Mã khóa học nên ngắn gọn (6-10 ký tự)
- **Migration strategy**: Chạy migration qua project Data khi solution phức tạp

### **7.2 Architecture Patterns**
- **Separation of concerns**: Tách biệt Entity, Service, Controller, View
- **Area-based organization**: Phân chia chức năng theo vai trò (Admin, Teacher, Customer)
- **Caching strategy**: Sử dụng IMemoryCache cho dữ liệu ít thay đổi
- **ViewModel mapping**: Tạo ViewModels riêng cho từng use case

### **7.3 Code Organization**
- **Service layer**: Đóng gói business logic và data access
- **Controller thin**: Controllers chỉ xử lý HTTP, gọi services
- **ViewModels specific**: Mỗi area có ViewModels phù hợp với nhu cầu
- **Consistent patterns**: Áp dụng pattern thống nhất trong toàn hệ thống

## 8. Hướng phát triển

### **Ngắn hạn:**
1. **Hoàn thiện Admin Views**: Tạo đầy đủ CRUD views cho Admin area
2. **Teacher authentication**: Filter courses theo giáo viên đăng nhập
3. **Student management**: Thực hiện chức năng quản lý học viên thực tế

### **Trung hạn:**
1. **Course enrollment**: Hệ thống đăng ký khóa học
2. **Progress tracking**: Theo dõi tiến độ học tập thực tế
3. **Assignment system**: Hệ thống bài tập và chấm điểm

### **Dài hạn:**
1. **Payment integration**: Hệ thống thanh toán cho khóa học trả phí
2. **Certificate generation**: Tự động tạo chứng chỉ hoàn thành
3. **Analytics dashboard**: Bảng điều khiển phân tích nâng cao

## 9. Tình trạng hiện tại

✅ **Đã hoàn thành:**
- Entity Course với đầy đủ attributes
- Database integration (DbContext, Seeder, Migration)
- CoursesService với caching và business logic
- Admin CoursesController với CRUD
- Teacher CoursesController với basic functions
- Teacher Views (Index, Details)

⚠️ **Cần hoàn thiện:**
- Admin Views (CRUD interfaces)
- Teacher authentication integration
- Student management real data
- Customer area updates (nếu cần)

🚀 **Sẵn sàng sử dụng:**
- Database có 3 khóa học MOS mẫu
- CoursesService đọc từ database
- Admin có thể quản lý khóa học qua database
- Teacher có giao diện quản lý lớp cơ bản
- Customer area tiếp tục hoạt động với dữ liệu mới
