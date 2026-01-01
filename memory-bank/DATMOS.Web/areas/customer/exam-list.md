# Chuyển đổi exam-list.json sang Entity Framework

## Tổng quan
- **Ngày:** 30/12/2025
- **Mục tiêu:** Chuyển dữ liệu từ file JSON sang bảng ExamList trong database
- **Vị trí file JSON:** `DATMOS.Web/wwwroot/areas/customer/json/exam-list.json`

## Phân tích dữ liệu JSON
File chứa 8 bài thi với các thuộc tính:
- `id`: 1-8
- `subjectId`: 1 (Word), 2 (Excel), 3 (PowerPoint)
- `code`: Mã bài thi (WORD_PE_01, EXCEL_PE_01, etc.)
- `name`: Tên bài thi
- `description`: Mô tả
- `type`: Practice, SkillReview
- `mode`: Testing, Training
- `totalProjects`: 5-7
- `totalTasks`: 25-35
- `timeLimit`: 0-50 phút
- `passingScore`: 0-700
- `difficulty`: Easy, Medium, Hard
- `isActive`: true

## Thiết kế Entity
### ExamList Entity
```csharp
public class ExamList
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public ExamSubject? Subject { get; set; }
}
```

### Quan hệ
- `ExamList.SubjectId` → `ExamSubject.Id` (khóa ngoại)

## Các bước thực hiện
1. **Tạo entity ExamList** trong `DATMOS.Core/Entities/`
2. **Cập nhật AppDbContext**:
   - Thêm `DbSet<ExamList> Exams`
   - Cấu hình trong `OnModelCreating`
3. **Tạo migration**: `AddExamListEntity`
4. **Cập nhật database**
5. **Tạo seeder** (tùy chọn): `ExamListSeeder.cs`

## Dữ liệu seed
8 bản ghi từ JSON sẽ được chuyển vào database.

## Lợi ích
- Quản lý dữ liệu tập trung qua database
- Dễ dàng truy vấn, cập nhật
- Tương thích với hệ thống hiện có
- Hỗ trợ phân trang, tìm kiếm

## Tiến độ
- [x] Phân tích dữ liệu JSON
- [x] Thiết kế entity
- [x] Lập kế hoạch migration
- [x] Tạo entity ExamList
- [x] Cập nhật AppDbContext
- [x] Tạo migration
- [x] Cập nhật database
- [x] Tạo seeder

## Ghi chú
- Entity sẽ được tạo trong namespace `DATMOS.Core.Entities`
- Sử dụng Data Annotation và Fluent API để cấu hình
- Migration sẽ tạo bảng `ExamLists` trong database PostgreSQL
- Dữ liệu seed sẽ giữ nguyên cấu trúc từ JSON
