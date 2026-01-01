# DATMOS.Web - Services Layer

## Tổng quan
Services layer chứa business logic của ứng dụng, tách biệt khỏi presentation layer (controllers) và data access layer (DbContext). Các services được đăng ký trong DI container và inject vào controllers.

## Service Categories

### 1. Navigation Services
Quản lý menu và navigation trong ứng dụng.

#### IMenuService (`Services/Navigation/IMenuService.cs`)
**Interface**:
```csharp
public interface IMenuService
{
    Task<List<MenuItemViewModel>> GetMenuAsync(string menuType, string? area = null);
    Task<MenuItemViewModel?> GetMenuItemByIdAsync(string id);
    Task<List<MenuItemViewModel>> GetActiveMenuAsync(string menuType, string? area = null);
    Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null);
    Task ClearCacheAsync(string menuType);
}
```

#### MenuService (`Services/Navigation/MenuService.cs`)
**Implementation**:
- **Caching**: Sử dụng MemoryCache để cache menu data
- **Hierarchy building**: Xây dựng menu hierarchy từ flat list
- **Active state**: Xác định active menu item dựa trên current route
- **Multi-type support**: Hỗ trợ 4 loại menu (Admin, Customer, Teacher, Landing)

**Key Methods**:
- `GetMenuAsync()`: Lấy menu với caching
- `BuildMenuHierarchy()`: Xây dựng cây menu từ flat list
- `IsMenuItemActive()`: Kiểm tra active state

### 2. Course Management Services
Quản lý khóa học và bài học.

#### ICoursesService (`Services/ICoursesService.cs`)
**Interface**:
```csharp
public interface ICoursesService
{
    Task<List<CourseViewModel>> GetAllCoursesAsync();
    Task<CourseViewModel?> GetCourseByIdAsync(int id);
    Task<CourseViewModel> CreateCourseAsync(CourseViewModel course);
    Task<CourseViewModel> UpdateCourseAsync(int id, CourseViewModel course);
    Task<bool> DeleteCourseAsync(int id);
    Task<List<LessonViewModel>> GetCourseLessonsAsync(int courseId);
}
```

#### CoursesService (`Services/CoursesService.cs`)
**Implementation**:
- **Course CRUD**: Quản lý khóa học
- **Lesson management**: Quản lý bài học trong khóa học
- **Progress tracking**: Theo dõi tiến độ học tập

### 3. Exam Management Services
Quản lý đề thi và câu hỏi.

#### IExamSubjectService (`Services/IExamSubjectService.cs`)
**Interface**:
```csharp
public interface IExamSubjectService
{
    Task<List<ExamSubjectViewModel>> GetAllExamSubjectsAsync();
    Task<ExamSubjectViewModel?> GetExamSubjectByIdAsync(int id);
    Task<ExamSubjectViewModel> CreateExamSubjectAsync(ExamSubjectViewModel examSubject);
    Task<ExamSubjectViewModel> UpdateExamSubjectAsync(int id, ExamSubjectViewModel examSubject);
    Task<bool> DeleteExamSubjectAsync(int id);
    Task<List<QuestionViewModel>> GetExamQuestionsAsync(int examSubjectId);
}
```

#### ExamSubjectService (`Services/ExamSubjectService.cs`)
**Implementation**:
- **Exam management**: Quản lý môn thi
- **Question bank**: Ngân hàng câu hỏi
- **Scoring**: Tính điểm và đánh giá

### 4. Lesson Management Services
Quản lý bài học và nội dung học tập.

#### ILessonService (`Services/ILessonService.cs`)
**Interface**:
```csharp
public interface ILessonService
{
    Task<List<LessonViewModel>> GetAllLessonsAsync();
    Task<LessonViewModel?> GetLessonByIdAsync(int id);
    Task<LessonViewModel> CreateLessonAsync(LessonViewModel lesson);
    Task<LessonViewModel> UpdateLessonAsync(int id, LessonViewModel lesson);
    Task<bool> DeleteLessonAsync(int id);
    Task<List<ContentViewModel>> GetLessonContentsAsync(int lessonId);
}
```

#### LessonService (`Services/LessonService.cs`)
**Implementation**:
- **Lesson CRUD**: Quản lý bài học
- **Content management**: Quản lý nội dung (video, text, quiz)
- **Sequence management**: Thứ tự bài học trong khóa học

## Service Registration

### DI Configuration (`WebEntryPoint.cs`)
```csharp
services.AddScoped<DATMOS.Web.Services.IExamSubjectService, DATMOS.Web.Services.ExamSubjectService>();
services.AddScoped<DATMOS.Web.Services.ICoursesService, DATMOS.Web.Services.CoursesService>();
services.AddScoped<DATMOS.Web.Services.ILessonService, DATMOS.Web.Services.LessonService>();
services.AddScoped<DATMOS.Web.Services.Navigation.IMenuService, DATMOS.Web.Services.Navigation.MenuService>();
```

### Lifetime Management
- **Scoped**: Mỗi request có instance riêng (phù hợp cho DbContext và services)
- **Singleton**: Một instance cho toàn bộ application (cẩn thận với state)
- **Transient**: Mới mỗi lần resolve (ít dùng cho services)

## Design Patterns trong Services

### 1. Repository Pattern
Services sử dụng DbContext như repository:
```csharp
public class CoursesService : ICoursesService
{
    private readonly AppDbContext _context;
    
    public CoursesService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<CourseViewModel>> GetAllCoursesAsync()
    {
        var courses = await _context.Courses.ToListAsync();
        return MapToViewModels(courses);
    }
}
```

### 2. Caching Pattern
MenuService sử dụng caching để cải thiện performance:
```csharp
public class MenuService : IMenuService
{
    private readonly IMemoryCache _cache;
    
    public async Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null)
    {
        var cacheKey = $"Menu_{menuType}_{area}";
        
        if (!_cache.TryGetValue(cacheKey, out MenuDataViewModel menuData))
        {
            menuData = await BuildMenuDataAsync(menuType, area);
            _cache.Set(cacheKey, menuData, TimeSpan.FromMinutes(30));
        }
        
        return menuData;
    }
}
```

### 3. Mapping Pattern
Map giữa Entities và ViewModels:
```csharp
private CourseViewModel MapToViewModel(Course entity)
{
    return new CourseViewModel
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        // ... other properties
    };
}
```

### 4. Strategy Pattern
Xử lý different menu types:
```csharp
private async Task<List<Menu>> GetMenusByTypeAsync(string menuType, string? area)
{
    var query = _context.Menus
        .Where(m => m.MenuType == menuType && m.IsVisible);
    
    if (!string.IsNullOrEmpty(area))
    {
        query = query.Where(m => m.Area == area || string.IsNullOrEmpty(m.Area));
    }
    
    return await query.OrderBy(m => m.Order).ToListAsync();
}
```

## Error Handling trong Services

### 1. Exception Types
- `NotFoundException`: Khi resource không tồn tại
- `ValidationException`: Khi input validation fails
- `BusinessRuleException`: Khi vi phạm business rules

### 2. Error Propagation
```csharp
public async Task<CourseViewModel> GetCourseByIdAsync(int id)
{
    var course = await _context.Courses.FindAsync(id);
    
    if (course == null)
    {
        throw new NotFoundException($"Course with ID {id} not found.");
    }
    
    return MapToViewModel(course);
}
```

### 3. Logging
```csharp
public class MenuService : IMenuService
{
    private readonly ILogger<MenuService> _logger;
    
    public async Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null)
    {
        try
        {
            _logger.LogInformation("Getting menu data for type: {MenuType}, area: {Area}", menuType, area);
            // ... implementation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting menu data for type: {MenuType}", menuType);
            throw;
        }
    }
}
```

## Testing Services

### 1. Unit Testing
```csharp
[Test]
public async Task GetMenuAsync_ShouldReturnMenuItems()
{
    // Arrange
    var mockContext = new Mock<AppDbContext>();
    var mockCache = new Mock<IMemoryCache>();
    var service = new MenuService(mockContext.Object, mockCache.Object);
    
    // Act
    var result = await service.GetMenuAsync("Admin");
    
    // Assert
    Assert.NotNull(result);
    Assert.IsInstanceOf<List<MenuItemViewModel>>(result);
}
```

### 2. Integration Testing
```csharp
[Test]
public async Task CoursesService_ShouldCreateAndRetrieveCourse()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;
    
    using var context = new AppDbContext(options);
    var service = new CoursesService(context);
    
    // Act & Assert
    var course = await service.CreateCourseAsync(new CourseViewModel { Name = "Test Course" });
    Assert.NotNull(course);
    Assert.IsTrue(course.Id > 0);
}
```

## Performance Considerations

### 1. Database Optimization
- **Eager loading**: `Include()` cho related data
- **Select projection**: Chỉ select columns cần thiết
- **Pagination**: Cho danh sách lớn

### 2. Caching Strategy
- **Cache duration**: 30 phút cho menu data
- **Cache invalidation**: Khi data thay đổi
- **Cache keys**: Structured cache keys

### 3. Async Operations
- **Async/await**: Cho tất cả I/O operations
- **ConfigureAwait(false)**: Tránh deadlocks
- **Cancellation tokens**: Hỗ trợ cancellation

## Security Considerations

### 1. Input Validation
```csharp
public async Task<CourseViewModel> CreateCourseAsync(CourseViewModel course)
{
    if (course == null)
        throw new ArgumentNullException(nameof(course));
    
    if (string.IsNullOrWhiteSpace(course.Name))
        throw new ValidationException("Course name is required.");
    
    // ... implementation
}
```

### 2. Authorization Checks
```csharp
public async Task DeleteCourseAsync(int id, string userId)
{
    var course = await _context.Courses.FindAsync(id);
    
    // Check if user has permission to delete
    if (!await _authorizationService.CanDeleteCourseAsync(userId, course))
    {
        throw new UnauthorizedAccessException("User not authorized to delete this course.");
    }
    
    // ... delete implementation
}
```

### 3. SQL Injection Prevention
- Sử dụng Entity Framework Core parameterized queries
- Không dùng string concatenation cho SQL queries
- Input sanitization

## Monitoring & Logging

### 1. Performance Metrics
```csharp
public async Task<MenuDataViewModel> GetMenuDataAsync(string menuType, string? area = null)
{
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        // ... implementation
    }
    finally
    {
        stopwatch.Stop();
        _logger.LogInformation("GetMenuDataAsync completed in {ElapsedMilliseconds}ms", 
            stopwatch.ElapsedMilliseconds);
    }
}
```

### 2. Business Metrics
- Số lượng courses được tạo
- Số lượng users active
- Menu cache hit rate

### 3. Health Checks
```csharp
public async Task<bool> IsHealthyAsync()
{
    try
    {
        // Test database connection
        await _context.Database.CanConnectAsync();
        return true;
    }
    catch
    {
        return false;
    }
}
```

## Extension Points

### 1. Plugin Architecture
Services có thể được mở rộng thông qua:
- Interface inheritance
- Dependency injection
- Strategy pattern

### 2. Event System
```csharp
public class CoursesService : ICoursesService
{
    private readonly IEventPublisher _eventPublisher;
    
    public async Task<CourseViewModel> CreateCourseAsync(CourseViewModel course)
    {
        // ... create course
        
        await _eventPublisher.PublishAsync(new CourseCreatedEvent(course.Id));
        
        return course;
    }
}
```

### 3. Custom Providers
- Caching providers (Redis, SQL Server, etc.)
- Storage providers (Azure Blob, AWS S3, etc.)
- Notification providers (Email, SMS, Push)

---
*Document version: 1.0 - Last updated: 2025-12-23*
