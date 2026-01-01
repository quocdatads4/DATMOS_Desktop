# Memory Bank: Exam Subject Management (Quản lý Môn thi)

## Tổng quan

Trang danh sách môn thi (`ExamSubject/Index.cshtml`) đóng vai trò là cổng thông tin để học viên lựa chọn môn học và luyện thi chứng chỉ MOS (Word, Excel, PowerPoint). Giao diện cần hiển thị rõ ràng các thông tin về khóa học, thời lượng, số lượng bài học/bài thi và trạng thái.

## Phân tích dữ liệu (JSON Source)

Dựa trên file `exam-subject.json`, cấu trúc dữ liệu của một môn thi bao gồm:

```json
{
  "id": 1,
  "code": "CRS001",
  "name": "Microsoft Word 2019",
  "shortName": "Word 2019",
  "title": "Word 2019 Specialist",
  "description": "...",
  "icon": "ti-file-text",
  "colorClass": "primary",
  "duration": "8 giờ",
  "totalLessons": 12,
  "totalExams": 2,
  "badge": {
    "text": "Phổ biến",
    "icon": "ti-trending-up",
    "colorClass": "bg-label-primary"
  }
}
```

## Thiết kế Entity Framework & ViewModel

Để ánh xạ dữ liệu JSON này vào hệ thống Entity Framework và sử dụng trong View, chúng ta cần định nghĩa các class sau:

### Entity: `ExamSubject`

```csharp
public class ExamSubject
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } // e.g., CRS001

    [Required]
    [StringLength(100)]
    public string Name { get; set; } // e.g., Microsoft Word 2019

    [StringLength(50)]
    public string ShortName { get; set; } // e.g., Word 2019

    [StringLength(200)]
    public string Title { get; set; } // e.g., Word 2019 Specialist

    public string Description { get; set; }

    [StringLength(50)]
    public string Icon { get; set; } // e.g., ti-file-text

    [StringLength(20)]
    public string ColorClass { get; set; } // e.g., primary, success, warning

    [StringLength(50)]
    public string Duration { get; set; } // e.g., 8 giờ

    public int TotalLessons { get; set; }
    public int TotalExams { get; set; }

    // Badge Properties (Flattened for simplicity or Owned Type)
    [StringLength(50)]
    public string BadgeText { get; set; }
    
    [StringLength(50)]
    public string BadgeIcon { get; set; }
    
    [StringLength(50)]
    public string BadgeColorClass { get; set; }
}
```

### ViewModel: `ExamSubjectViewModel`

ViewModel sẽ tương tự như Entity nhưng có thể bao gồm thêm các thuộc tính tính toán hoặc định dạng để hiển thị trên View (ví dụ: `IsActive`, `UserProgress`).

## Cấu trúc View (Razor)

File `Areas/Customer/Views/ExamSubject/Index.cshtml` sẽ được thiết kế với layout:

1.  **Header Section**: Tiêu đề trang và mô tả ngắn gọn.
2.  **Grid System**: Sử dụng Bootstrap Grid (`row`, `col-xxl-4`, `col-md-6`) để hiển thị danh sách môn thi.
3.  **Card Component**: Mỗi môn thi là một Card với:
    *   **Header**: Tên môn thi và Badge (Phổ biến, Mới, v.v.).
    *   **Body**: Icon lớn đại diện, Tiêu đề, Mô tả, Thông số (Thời gian, Bài học).
    *   **Footer/Actions**: Nút "Ôn luyện" và "Thi thử".