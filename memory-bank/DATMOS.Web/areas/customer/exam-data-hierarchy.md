# Kế hoạch Thực hiện Cấu trúc Dữ liệu Thi (Exam Data Hierarchy)

## 1. Tổng quan
Tài liệu này mô tả kế hoạch và cấu trúc dữ liệu cho hệ thống thi của DATMOS. Hệ thống sử dụng **Entity Framework** làm nền tảng lưu trữ chính. Các file JSON chỉ được sử dụng làm nguồn dữ liệu mẫu để nạp (Seed) vào database ban đầu.

## 2. Cấu trúc Phân cấp
Hệ thống tuân theo mô hình phân cấp 4 cấp:
1.  **ExamSubject (Môn thi):** Word, Excel, PowerPoint.
2.  **ExamList (Đề thi):** Mỗi môn có 3 đề.
3.  **ExamProject (Dự án):** Mỗi đề có ít nhất 6 dự án.
4.  **ExamTask (Nhiệm vụ):** Mỗi dự án có ít nhất 6 nhiệm vụ.

## 3. Phân tích & Kế hoạch Thực hiện

### 3.1. ExamSubject
- **Yêu cầu:** Tồn tại các môn thi chính.
- **Hiện trạng:** Đã có Seeder (`ExamSubjectSeeder`) tạo đủ 3 môn: Word, Excel, PowerPoint.
- **Hành động:** Không cần thay đổi.

### 3.2. ExamList
- **Yêu cầu:** 3 ExamList cho mỗi ExamSubject.
- **Hiện trạng:**
    - Word: 3 đề.
    - Excel: 3 đề.
    - PowerPoint: Đã cập nhật lên 3 đề (thêm `PPT_SR_01` trong `ExamListSeeder`).
- **Hành động:** Đảm bảo `ExamListSeeder` chạy chính xác.

### 3.3. ExamProject
- **Yêu cầu:** 6 ExamProject trở lên cho mỗi ExamList.
- **Hiện trạng:** `ExamProjectSeeder` đang tạo 6 dự án cho mỗi đề thi dựa trên danh sách ExamList hiện có.
- **Hành động:** Giữ nguyên logic seeding hiện tại.

### 3.4. ExamTask
- **Yêu cầu:** 6 ExamTask trở lên cho mỗi ExamProject.
- **Hiện trạng:**
    - Entity `ExamTask`: Đã được thiết kế và thêm vào Core.
    - `AppDbContext`: Đã cấu hình DbSet và quan hệ (Foreign Key với ExamProject).
    - Seeder: `ExamTaskSeeder` đã được tạo để sinh 6 task mẫu cho mỗi project.
- **Hành động:**
    - Chạy Migration để cập nhật database.
    - Chạy Seeder để nạp dữ liệu.

## 4. Chi tiết Kỹ thuật

### Entity: ExamTask
```csharp
public class ExamTask
{
    public int Id { get; set; }
    public int ExamProjectId { get; set; } // FK
    public string Name { get; set; }
    public string Description { get; set; }
    public string Instructions { get; set; }
    public int OrderIndex { get; set; }
    public double MaxScore { get; set; }
    public string TaskType { get; set; }
    public bool IsActive { get; set; }
    // ...
}
```

### Quy trình Seeding (Program.cs)
Thứ tự chạy seeders để đảm bảo toàn vẹn dữ liệu:
1.  `ExamSubjectSeeder`
2.  `ExamListSeeder` (phụ thuộc Subject)
3.  `ExamProjectSeeder` (phụ thuộc ExamList)
4.  `ExamTaskSeeder` (phụ thuộc ExamProject)

## 5. Trạng thái
- [x] Thiết kế Database Schema
- [x] Cập nhật DbContext & Entity
- [x] Viết Seeder cho ExamList, ExamProject, ExamTask
- [ ] Chạy Migration (`AddExamTaskEntity`)
- [ ] Refactor Seeder để sử dụng dữ liệu JSON thật

## 6. Chiến lược Chuyển đổi Dữ liệu (Data Migration Strategy)

### 6.1. Nguyên tắc
- **Nguồn (Source):** Các file JSON trong thư mục `wwwroot/areas/customer/json/`.
- **Đích (Destination):** Database PostgreSQL thông qua Entity Framework Core.
- **Công cụ (Tool):** Các class `Seeder` trong project `DATMOS.Data`.
- **Quy tắc:** Ứng dụng Web chỉ đọc dữ liệu từ Database, không phụ thuộc vào file JSON khi vận hành.

### 6.2. Xử lý Khoảng trống Dữ liệu (Gap Handling)

1.  **Nội dung Task:**
    - Dữ liệu từ `exam-tasks.json` (105 tasks) sẽ được map vào entity `ExamTask`.
    - Các trường thiếu trong JSON (Instructions, TaskType, MaxScore) sẽ được sinh tự động hoặc gán giá trị mặc định hợp lý trong quá trình Seed.

2.  **Số lượng Task/Project:**
    - **Vấn đề:** JSON chỉ cung cấp 5 task/project. Yêu cầu hệ thống là tối thiểu 6 task.
    - **Giải pháp:** `ExamTaskSeeder` sẽ tự động chèn thêm một task thứ 6 là "Save & Close" (Lưu và đóng) cho mọi dự án để đảm bảo tính toàn vẹn của cấu trúc bài thi.

3.  **Liên kết Project - Exam:**
    - Đảm bảo `ExamProjectSeeder` tạo ra các Project có ID khớp với `projectId` trong file `exam-tasks.json` (ID 1-21).

## 7. Kế hoạch Cập nhật (Next Steps)
1.  **Cập nhật ExamTaskSeeder:**
    - Đọc file `exam-tasks.json`.
    - Deserialize thành object.
    - Loop qua từng ProjectID.
    - Insert 5 task từ JSON.
    - Insert thêm task thứ 6 (Save File).
    - Lưu vào Database.