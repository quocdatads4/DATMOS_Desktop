# Master Plan: Exam Data Analysis & Implementation

**Context:** DATMOS.Web / Customer Area / Exam Module
**Version:** 4.0 (Scientific & Actionable)
**Last Updated:** 30/12/2025
**Objective:** Implement a hierarchical exam data system (Subject -> List -> Project -> Task) with full data integrity and UI integration.

## 1. Data Architecture Specification

The system strictly enforces a 4-level hierarchy. All implementations must adhere to these cardinality rules.

| Level | Entity | Cardinality Rule | Total Required | Source Strategy |
| :--- | :--- | :--- | :--- | :--- |
| **1** | `ExamSubject` | Fixed (Word, Excel, PowerPoint) | 3 | Seeder / `exam-subject.json` |
| **2** | `ExamList` | 3 Lists per Subject | 9 | Seeder / `exam-list.json` |
| **3** | `ExamProject` | ≥ 6 Projects per List | 54 | Seeder (Auto-generated structure) |
| **4** | `ExamTask` | ≥ 6 Tasks per Project | **324+** | Hybrid: JSON (Partial) + Generator (Gap fill) |

### 1.1. Entity Relationships (EF Core)
```csharp
ExamSubject (1) <---> (*) ExamList
ExamList (1)    <---> (*) ExamProject
ExamProject (1) <---> (*) ExamTask
```

## 2. Current State & Gap Analysis

### 2.1. Status Overview
- **Models & Migrations:** ✅ Complete (All 4 levels).
- **Seeders:** ⚠️ Partial/Buggy.
    - `ExamListSeeder`: References wrong DbSet (`context.Exams` instead of `context.ExamLists`).
    - `ExamTaskSeeder`: Relies on incomplete JSON.
- **Data Completeness:** ⚠️ Critical Gap in Level 4 (Tasks).
    - Available JSON Tasks: ~105 (Covers ~21 projects).
    - Missing Tasks: ~219 (For remaining 33 projects).

### 2.2. Critical Issues to Resolve
1.  **Seeder Logic Error:** `ExamListSeeder.cs` will fail due to incorrect context reference.
2.  **Data Gap:** Cannot deploy without full task coverage (6 tasks/project).
3.  **Schema Mismatch:** JSON data lacks `Instructions`, `TaskType`, `MaxScore` fields required by the Entity.

## 3. Implementation Roadmap

### Phase 1: Data Layer Standardization (Immediate)
**Goal:** Fix Seeders and validate Database Schema.

- [ ] **Task 1.1: Fix `ExamListSeeder.cs`**
    - Locate: `DATMOS.Data/ExamListSeeder.cs`
    - Action: Change `context.Exams` to `context.ExamLists`.
    - Action: Ensure `SubjectId` mapping matches `ExamSubjects` (Word=1, Excel=2, PPT=3).
- [ ] **Task 1.2: Database Validation**
    - Action: Run `dotnet ef database update`.
    - Action: Verify tables: `ExamSubjects`, `ExamLists`, `ExamProjects`, `ExamTasks`.

### Phase 2: Data Population Strategy (High Priority)
**Goal:** Fill the 219+ missing tasks using an automated generator.

- [ ] **Task 2.1: Create `TaskGenerator` Utility**
    - File: `DATMOS.Data/Utilities/TaskGenerator.cs`
    - Logic: Generate 6 tasks per project based on Subject (Word/Excel/PPT).
    - Requirement: Task #6 must always be "Save & Close".
    - Reference: See Section 4.1 for code.
- [ ] **Task 2.2: Upgrade `ExamTaskSeeder`**
    - File: `DATMOS.Data/ExamTaskSeeder.cs`
    - Logic:
        1. Load all `ExamProjects`.
        2. Attempt to load tasks from `exam-tasks.json`.
        3. If tasks < 6, invoke `TaskGenerator` to fill the gap.
        4. Save to DB.

### Phase 3: Backend Logic (Controllers & Services)
**Goal:** Expose hierarchical data to the frontend.

- [ ] **Task 3.1: Implement Services**
    - File: `DATMOS.Web/Services/ExamListService.cs`
    - Method: `GetExamListWithHierarchyAsync(int id)`
    - Logic: Use `.Include(x => x.ExamProjects).ThenInclude(p => p.ExamTasks)`.
- [ ] **Task 3.2: Implement Controllers**
    - File: `DATMOS.Web/Areas/Customer/Controllers/ExamListController.cs`
    - Actions: `Index` (List by Subject), `TakeExam` (Full hierarchy view).
    - Reference: See Section 4.2 for code.

### Phase 4: Frontend Implementation (UI/UX)
**Goal:** User interface for taking exams.

- [ ] **Task 4.1: Create ViewModels**
    - `ExamSubjectViewModel`
    - `ExamListViewModel`
    - `ExamProjectViewModel`
    - `ExamTaskViewModel`
    - `ExamTakingViewModel` (Aggregate root for the exam view).
- [ ] **Task 4.2: Create Views**
    - `Areas/Customer/Views/ExamList/TakeExam.cshtml`: Main exam interface.
    - `Areas/Customer/Views/Shared/_ExamTaskPanel.cshtml`: Reusable task list component.

## 4. Reference Implementations

### 4.1. TaskGenerator Reference
Use this logic to fill data gaps.

```csharp
public class TaskGenerator
{
    public List<ExamTask> GenerateTasksForProject(int projectId, string subjectType, int count = 6)
    {
        var tasks = new List<ExamTask>();
        for (int i = 1; i < count; i++)
        {
            tasks.Add(new ExamTask {
                ExamProjectId = projectId,
                Name = $"Task {i}: {GetRandomAction(subjectType)}",
                Description = "Perform the action as described.",
                Instructions = "Step 1: Open menu...\nStep 2: Select option...",
                OrderIndex = i,
                MaxScore = 100.0 / count,
                TaskType = "General",
                IsActive = true
            });
        }
        // Mandatory Final Task
        tasks.Add(new ExamTask {
            ExamProjectId = projectId,
            Name = $"Task {count}: Save & Close",
            Description = "Save the document and close the application.",
            OrderIndex = count,
            MaxScore = 100.0 / count,
            TaskType = "File",
            IsActive = true
        });
        return tasks;
    }
    
    private string GetRandomAction(string subject) => subject switch {
        "Word" => "Format Paragraph",
        "Excel" => "Insert Formula",
        _ => "Edit Slide"
    };
}
```

### 3.2. Database Schema Validation

```sql
-- Expected tables
ExamSubjects (Id, Code, Name, ShortName, Title, Description, Icon, ColorClass, ...)
ExamLists (Id, SubjectId, Code, Name, Type, Mode, TotalProjects, TotalTasks, ...)
ExamProjects (Id, ExamListId, Name, Description, TotalTasks, OrderIndex, ...)
ExamTasks (Id, ExamProjectId, Name, Description, Instructions, OrderIndex, MaxScore, ...)
```

## 4. SEEDER IMPLEMENTATION ANALYSIS

### 4.1. Existing Seeders

#### **ExamSubjectSeeder.cs**
- **Status:** ✅ Hoàn thành
- **Data:** 5 môn thi (Word, Excel, PowerPoint, Outlook, Access)
- **Method:** `Seed(AppDbContext context)`

#### **ExamListSeeder.cs** 
- **Status:** ✅ Hoàn thành
- **Data:** 9 lists (3 Word + 3 Excel + 3 PowerPoint)
- **Dependency:** Requires ExamSubjects
- **Issue:** Uses `context.Exams` (should be `context.ExamLists`?)

#### **ExamProjectSeeder.cs**
- **Status:** ⚠️ Cần kiểm tra
- **Data:** 6 projects cho mỗi ExamList (tổng 54)
- **Method:** `SeedAsync(AppDbContext context)`
- **Dependency:** Requires ExamLists

#### **ExamTaskSeeder.cs**
- **Status:** ⚠️ Có vấn đề
- **Data:** Phụ thuộc vào JSON file (105 tasks)
- **Issue:** JSON chỉ có tasks cho 21 projects đầu
- **Gap Handling:** Thêm task thứ 6 "Save & Close" cho mỗi project

### 4.2. Seeder Execution Order

```csharp
// Correct execution order:
1. ExamSubjectSeeder.Seed(context);
2. ExamListSeeder.Seed(context); 
3. await ExamProjectSeeder.SeedAsync(context);
4. await ExamTaskSeeder.SeedAsync(context);
```

## 5. JSON DATA SOURCE ANALYSIS

### 5.1. File Locations
- `DATMOS.Web/wwwroot/areas/customer/json/exam-subject.json` (5 subjects)
- `DATMOS.Web/wwwroot/areas/customer/json/exam-list.json` (8 lists)  
- `DATMOS.Web/wwwroot/areas/customer/json/exam-tasks.json` (105 tasks)

### 5.2. JSON to Entity Mapping Gaps

| JSON Field | Entity Field | Status | Notes |
|------------|--------------|--------|-------|
| id | Id | ✅ | Direct mapping |
| projectId | ExamProjectId | ✅ | Foreign key |
| name | Name | ✅ | Direct mapping |
| description | Description | ✅ | Direct mapping |
| order | OrderIndex | ✅ | Direct mapping |
| - | Instructions | ⚠️ | Generated by seeder |
| - | MaxScore | ⚠️ | Calculated (100/6) |
| - | TaskType | ⚠️ | Inferred from name |
| - | IsActive | ✅ | Default true |
| - | CreatedAt | ✅ | DateTime.UtcNow |

## 6. IMPLEMENTATION PLAN

### Phase 1: Data Validation & Analysis (Ngắn hạn)

#### **Task 1.1: Database Inspection**
```bash
# Check current data in database
dotnet ef database update
# Run query to count records
SELECT COUNT(*) FROM "ExamSubjects";
SELECT COUNT(*) FROM "ExamLists"; 
SELECT COUNT(*) FROM "ExamProjects";
SELECT COUNT(*) FROM "ExamTasks";
```

#### **Task 1.2: Seeder Validation**
- [ ] Fix `ExamListSeeder.cs` (change `context.Exams` to `context.ExamLists`)
- [ ] Test seeder execution order
- [ ] Verify all 54 ExamProjects created
- [ ] Check ExamTaskSeeder JSON path resolution

#### **Task 1.3: Data Completeness Report**
- [ ] Generate report of missing data
- [ ] Calculate exact number of missing tasks
- [ ] Identify projects without tasks

### Phase 2: Data Completion (Trung hạn)

#### **Task 2.1: Extend JSON Data**
**Option A:** Manually extend `exam-tasks.json`
- Add tasks for remaining 33 projects
- Maintain consistent task structure
- Ensure 6 tasks per project

**Option B:** Auto-generation tool
- Create `TaskGenerator.cs` 
- Use patterns from existing 105 tasks
- Generate realistic exam tasks

#### **Task 2.2: Enhance ExamTaskSeeder**
```csharp
// Proposed enhancements:
1. Better error handling for missing JSON
2. Fallback to auto-generation when JSON incomplete  
3. Improved task type inference
4. Dynamic MaxScore calculation
5. Better instructions generation
```

#### **Task 2.3: Data Quality Improvements**
- [ ] Add validation for task instructions
- [ ] Implement task type categorization
- [ ] Add difficulty levels to tasks
- [ ] Include estimated completion time

### Phase 3: UI/UX Integration (Dài hạn)

#### **Task 3.1: Customer Area Controllers**
```csharp
// Required controllers:
1. ExamSubjectController - List subjects
2. ExamListController - List exams by subject  
3. ExamProjectController - Show project details
4. ExamTaskController - Display tasks
5. ExamTakingController - Exam interface
```

#### **Task 3.2: View Models**
```csharp
// Required view models:
1. ExamSubjectViewModel
2. ExamListViewModel
3. ExamProjectViewModel  
4. ExamTaskViewModel
5. ExamHierarchyViewModel (aggregate)
6. ExamProgressViewModel
```

#### **Task 3.3: Views Structure**
```
Areas/Customer/Views/
├── ExamSubject/
│   ├── Index.cshtml          # All subjects
│   └── Details.cshtml        # Subject details + exam lists
├── ExamList/
│   ├── Index.cshtml          # Exams for a subject
│   ├── Details.cshtml        # Exam details + projects
│   └── TakeExam.cshtml       # Exam interface
├── ExamProject/
│   └── Details.cshtml        # Project details + tasks
└── Shared/
    └── _ExamTaskPanel.cshtml # Reusable task component
```

## 7. TECHNICAL SPECIFICATIONS

### 7.1. Database Requirements

#### **PostgreSQL Configuration**
```json
// appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=datmos_db;Username=postgres;Password=password"
}
```

#### **Indexing Strategy**
```sql
-- Recommended indexes
CREATE INDEX idx_examlists_subjectid ON "ExamLists" ("SubjectId");
CREATE INDEX idx_examprojects_examlistid ON "ExamProjects" ("ExamListId");
CREATE INDEX idx_examtasks_examprojectid ON "ExamTasks" ("ExamProjectId");
CREATE INDEX idx_examtasks_orderindex ON "ExamTasks" ("OrderIndex");
```

### 7.2. API Endpoints Design

#### **RESTful API Structure**
```
GET    /api/exam-subjects                    # List all subjects
GET    /api/exam-subjects/{id}              # Get subject details
GET    /api/exam-subjects/{id}/exam-lists   # Get lists for subject

GET    /api/exam-lists                      # List all exam lists  
GET    /api/exam-lists/{id}                 # Get exam list details
GET    /api/exam-lists/{id}/exam-projects   # Get projects for exam

GET    /api/exam-projects/{id}              # Get project details
GET    /api/exam-projects/{id}/exam-tasks   # Get tasks for project

GET    /api/exam-tasks/{id}                 # Get task details
```

### 7.3. Performance Considerations

#### **Eager Loading Strategy**
```csharp
// Recommended query patterns
var subjectWithHierarchy = await _context.ExamSubjects
    .Include(s => s.ExamLists)
        .ThenInclude(l => l.ExamProjects)
            .ThenInclude(p => p.ExamTasks)
    .FirstOrDefaultAsync(s => s.Id == subjectId);
```

#### **Pagination**
- Exam lists: 10-20 per page
- Exam projects: 6-12 per page  
- Exam tasks: 6 per project (no pagination needed)

## 8. RISK ANALYSIS & MITIGATION

### 8.1. Technical Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| JSON data incomplete | High | High | Auto-generation fallback |
| Seeder dependency issues | Medium | Medium | Better error handling |
| Database performance | Low | Medium | Indexing, eager loading |
| Data consistency | Medium | High | Transaction scope, validation |

### 8.2. Business Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Insufficient exam content | High | High | Content generation plan |
| Poor user experience | Medium | High | UX prototyping, user testing |
| Scalability issues | Low | Medium | Architecture review |

## 9. SUCCESS METRICS

### 9.1. Data Completeness Metrics
- [ ] 100% ExamSubjects (3/3)
- [ ] 100% ExamLists (9/9)  
- [ ] 100% ExamProjects (54/54)
- [ ] 100% ExamTasks (324/324)
- [ ] All tasks have instructions
- [ ] All tasks have appropriate task types
- [ ] Score distribution is logical

### 9.2. System Performance Metrics
- [ ] Page load time < 2 seconds
- [ ] API response time < 500ms
- [ ] Database queries optimized
- [ ] No N+1 query problems

### 9.3. User Experience Metrics
- [ ] Intuitive navigation hierarchy
- [ ] Clear exam progression
- [ ] Responsive task interface
- [ ] Progress tracking visible

## 10. NEXT STEPS IMMEDIATE

### Priority 1: Data Validation
1. Run database inspection queries
2. Fix ExamListSeeder DbSet reference
3. Execute all seeders in correct order
4. Generate data completeness report

### Priority 2: Gap Analysis
1. Identify exact missing task count
2. Analyze patterns in existing 105 tasks
3. Design task generation algorithm
4. Create TaskGenerator prototype

### Priority 3: Implementation Planning
1. Create detailed task breakdown
2. Estimate effort for each phase
3. Define acceptance criteria
4. Schedule implementation timeline

## 11. COMPLETE FILE INVENTORY FOR EXAM DATA HIERARCHY

### 11.1. Core Entities (DATMOS.Core/Entities/)
- `ExamSubject.cs` - Entity cho môn thi (Word, Excel, PowerPoint)
- `ExamList.cs` - Entity cho danh sách bài thi
- `ExamProject.cs` - Entity cho dự án thi
- `ExamTask.cs` - Entity cho nhiệm vụ thi

### 11.2. Database Context & Migrations (DATMOS.Data/)
- `AppDbContext.cs` - Database context với DbSet cho tất cả exam entities
- `Migrations/20251229133938_AddExamSubjectEntity.cs` - Migration cho ExamSubject
- `Migrations/20251230031619_AddExamListEntity.cs` - Migration cho ExamList
- `Migrations/20251230072316_AddExamProjectEntity.cs` - Migration cho ExamProject
- `Migrations/20251230080000_AddExamTaskEntityAndRelationships.cs` - Migration cho ExamTask

### 11.3. Data Seeders (DATMOS.Data/)
- `ExamSubjectSeeder.cs` - Seeder cho 5 môn thi
- `ExamListSeeder.cs` - Seeder cho 9 danh sách bài thi
- `ExamProjectSeeder.cs` - Seeder cho 54 dự án thi
- `ExamTaskSeeder.cs` - Seeder cho 324+ nhiệm vụ thi (phụ thuộc JSON)

### 11.4. JSON Data Sources (DATMOS.Web/wwwroot/areas/customer/json/)
- `exam-subject.json` - Dữ liệu 5 môn thi (Word, Excel, PowerPoint, Outlook, Access)
- `exam-list.json` - Dữ liệu 8 danh sách bài thi
- `exam-tasks.json` - Dữ liệu 105 nhiệm vụ thi (cho 21 projects đầu)

### 11.5. Services (DATMOS.Web/Services/)
- `ExamSubjectService.cs` - Service quản lý môn thi
- `ExamListService.cs` - Service quản lý danh sách bài thi
- `IExamSubjectService.cs` - Interface cho ExamSubjectService
- `IExamListService.cs` - Interface cho ExamListService

### 11.6. ViewModels (DATMOS.Web/ViewModels/)
- `UserProfileViewModel.cs` - Chứa thông tin profile người dùng (có thể mở rộng cho exam progress)

### 11.7. Memory Bank Files (memory-bank/DATMOS.Web/areas/customer/)
- `exam-data-hierarchy.md` - Kế hoạch tổng thể cấu trúc dữ liệu thi
- `exam-subject.md` - Chi tiết quản lý môn thi
- `exam-list.md` - Chi tiết chuyển đổi JSON sang Entity Framework
- `profile.md` - Quản lý profile người dùng
- `exam-data-analysis-plan.md` - File hiện tại: Phân tích & kế hoạch thực hiện

### 11.8. Project Files
- `DATMOS.Core/DATMOS.Core.csproj` - Core project file
- `DATMOS.Data/DATMOS.Data.csproj` - Data project file
- `DATMOS.Web/DATMOS.Web.csproj` - Web project file
- `DATMOS_Desktop.sln` - Solution file

### 11.9. Configuration Files
- `DATMOS.Web/appsettings.json` - Configuration chính (connection strings)
- `DATMOS.Web/appsettings.Development.json` - Configuration development

### 11.10. Files Cần Tạo (Chưa có)
#### Controllers (DATMOS.Web/Areas/Customer/Controllers/)
- `ExamSubjectController.cs` - Controller cho môn thi
- `ExamListController.cs` - Controller cho danh sách bài thi
- `ExamProjectController.cs` - Controller cho dự án thi
- `ExamTaskController.cs` - Controller cho nhiệm vụ thi
- `ExamTakingController.cs` - Controller cho giao diện làm bài

#### ViewModels (Cần tạo)
- `ExamSubjectViewModel.cs`
- `ExamListViewModel.cs`
- `ExamProjectViewModel.cs`
- `ExamTaskViewModel.cs`
- `ExamHierarchyViewModel.cs`
- `ExamProgressViewModel.cs`

#### Views (DATMOS.Web/Areas/Customer/Views/)
- `ExamSubject/Index.cshtml`
- `ExamSubject/Details.cshtml`
- `ExamList/Index.cshtml`
- `ExamList/Details.cshtml`
- `ExamList/TakeExam.cshtml`
- `ExamProject/Details.cshtml`
- `Shared/_ExamTaskPanel.cshtml`

### 11.11. Utility Files (Có thể cần tạo)
- `TaskGenerator.cs` - Công cụ tự động sinh tasks
- `ExamDataValidator.cs` - Validator cho dữ liệu exam
- `ExamStatisticsService.cs` - Service thống kê exam data

## 12. FILE DEPENDENCY MAP

```
DATMOS.Core/Entities/           DATMOS.Data/                 DATMOS.Web/
├── ExamSubject.cs              ├── AppDbContext.cs          ├── Services/
├── ExamList.cs                 ├── Migrations/              │   ├── ExamSubjectService.cs
├── ExamProject.cs              │   ├── AddExamSubjectEntity │   └── ExamListService.cs
└── ExamTask.cs                 │   ├── AddExamListEntity    ├── wwwroot/areas/customer/json/
                                │   ├── AddExamProjectEntity │   ├── exam-subject.json
                                ├── ExamSubjectSeeder.cs     │   ├── exam-list.json
                                ├── ExamListSeeder.cs        │   └── exam-tasks.json
                                ├── ExamProjectSeeder.cs     └── Areas/Customer/
                                └── ExamTaskSeeder.cs            ├── Controllers/ (cần tạo)
                                                                ├── ViewModels/ (cần tạo)
                                                                └── Views/ (cần tạo)
```

## 13. FILE STATUS TRACKING

### ✅ Đã tồn tại và hoàn chỉnh:
1. Core Entities (4 files)
2. Database Migrations (4 files)
3. Data Seeders (4 files)
4. JSON Data Sources (3 files)
5. Services (2 files + 2 interfaces)
6. Existing Memory Banks (4 files + file hiện tại)

### ⚠️ Cần kiểm tra/sửa:
1. `ExamListSeeder.cs` - Lỗi DbSet reference (`context.Exams` → `context.ExamLists`)
2. `ExamTaskSeeder.cs` - Phụ thuộc JSON không đầy đủ

### 🆕 Cần tạo mới:
1. Controllers (5 files)
2. ViewModels (6 files)
3. Views (7 files)
4. Utility files (3 files)

## 14. DETAILED IMPLEMENTATION GUIDE FOR NEW FILES

### 14.1. Controllers Implementation Details

#### **ExamSubjectController.cs**
```csharp
using DATMOS.Core.Entities;
using DATMOS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ExamSubjectController : Controller
    {
        private readonly IExamSubjectService _examSubjectService;
        private readonly IExamListService _examListService;

        public ExamSubjectController(IExamSubjectService examSubjectService, IExamListService examListService)
        {
            _examSubjectService = examSubjectService;
            _examListService = examListService;
        }

        // GET: Customer/ExamSubject
        public async Task<IActionResult> Index()
        {
            var subjects = await _examSubjectService.GetAllExamSubjectsAsync();
            var viewModel = subjects.Select(s => new ExamSubjectViewModel
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                ShortName = s.ShortName,
                Description = s.Description,
                Icon = s.Icon,
                ColorClass = s.ColorClass,
                TotalExams = s.TotalExams,
                TotalLists = await _examListService.GetExamListCountBySubjectAsync(s.Id)
            }).ToList();

            return View(viewModel);
        }

        // GET: Customer/ExamSubject/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _examSubjectService.GetExamSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var examLists = await _examListService.GetExamListsBySubjectIdAsync(id);
            
            var viewModel = new ExamSubjectDetailsViewModel
            {
                Subject = new ExamSubjectViewModel
                {
                    Id = subject.Id,
                    Code = subject.Code,
                    Name = subject.Name,
                    ShortName = subject.ShortName,
                    Description = subject.Description,
                    Icon = subject.Icon,
                    ColorClass = subject.ColorClass,
                    Duration = subject.Duration,
                    TotalLessons = subject.TotalLessons,
                    TotalExams = subject.TotalExams
                },
                ExamLists = examLists.Select(el => new ExamListViewModel
                {
                    Id = el.Id,
                    Code = el.Code,
                    Name = el.Name,
                    Type = el.Type,
                    Mode = el.Mode,
                    Difficulty = el.Difficulty,
                    TimeLimit = el.TimeLimit,
                    TotalProjects = el.TotalProjects,
                    TotalTasks = el.TotalTasks
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
```

#### **ExamListController.cs** (Key Methods)
```csharp
// GET: Customer/ExamList/BySubject/5
public async Task<IActionResult> BySubject(int subjectId)
{
    var examLists = await _examListService.GetExamListsBySubjectIdAsync(subjectId);
    var subject = await _examSubjectService.GetExamSubjectByIdAsync(subjectId);
    
    var viewModel = new ExamListBySubjectViewModel
    {
        Subject = subject,
        ExamLists = examLists,
        UserProgress = await GetUserProgressForSubject(subjectId)
    };
    
    return View(viewModel);
}

// GET: Customer/ExamList/TakeExam/5
public async Task<IActionResult> TakeExam(int id)
{
    var examList = await _examListService.GetExamListWithHierarchyAsync(id);
    if (examList == null) return NotFound();
    
    var viewModel = new ExamTakingViewModel
    {
        ExamList = examList,
        CurrentProjectIndex = 0,
        TotalProjects = examList.ExamProjects?.Count ?? 0,
        StartTime = DateTime.UtcNow,
        TimeRemaining = examList.TimeLimit * 60 // convert minutes to seconds
    };
    
    return View(viewModel);
}

// POST: Customer/ExamList/SubmitExam
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SubmitExam(ExamSubmissionViewModel submission)
{
    // Validate submission
    // Calculate scores
    // Save results to database
    // Return results view
}
```

### 14.2. ViewModels Implementation Details

#### **ExamSubjectViewModel.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamSubjectViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Mã môn")]
        public string Code { get; set; } = string.Empty;
        
        [Display(Name = "Tên môn")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Tên ngắn")]
        public string ShortName { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Biểu tượng")]
        public string Icon { get; set; } = string.Empty;
        
        [Display(Name = "Màu sắc")]
        public string ColorClass { get; set; } = string.Empty;
        
        [Display(Name = "Thời lượng")]
        public string Duration { get; set; } = string.Empty;
        
        [Display(Name = "Số bài học")]
        public int TotalLessons { get; set; }
        
        [Display(Name = "Số bài thi")]
        public int TotalExams { get; set; }
        
        [Display(Name = "Số danh sách thi")]
        public int TotalLists { get; set; }
        
        [Display(Name = "Tiến độ")]
        public double ProgressPercentage { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chưa bắt đầu";
    }
}
```

#### **ExamHierarchyViewModel.cs** (Aggregate ViewModel)
```csharp
public class ExamHierarchyViewModel
{
    public ExamSubjectViewModel Subject { get; set; }
    public List<ExamListViewModel> ExamLists { get; set; } = new();
    public Dictionary<int, List<ExamProjectViewModel>> ProjectsByList { get; set; } = new();
    public Dictionary<int, List<ExamTaskViewModel>> TasksByProject { get; set; } = new();
    public UserProgressViewModel UserProgress { get; set; }
    
    // Helper methods
    public int GetTotalTasks()
    {
        return TasksByProject.Values.Sum(tasks => tasks.Count);
    }
    
    public double GetCompletionPercentage()
    {
        if (UserProgress == null) return 0;
        var totalTasks = GetTotalTasks();
        return totalTasks > 0 ? (UserProgress.CompletedTasks * 100.0 / totalTasks) : 0;
    }
}
```

### 14.3. Views Implementation Details

#### **ExamSubject/Index.cshtml** (Main View)
```html
@model List<ExamSubjectViewModel>

@{
    ViewData["Title"] = "Danh sách Môn thi";
    Layout = "~/Areas/Customer/Views/Shared/_Layout.cshtml";
}

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">@ViewData["Title"]</h4>
                    <p class="card-subtitle">Chọn môn thi để bắt đầu luyện tập</p>
                </div>
                <div class="card-body">
                    <div class="row">
                        @foreach (var subject in Model)
                        {
                            <div class="col-xxl-4 col-md-6 mb-4">
                                <div class="card h-100 border-@subject.ColorClass">
                                    <div class="card-header d-flex justify-content-between align-items-center">
                                        <div>
                                            <span class="badge bg-@subject.ColorClass">@subject.Code</span>
                                            <h5 class="card-title mb-0">@subject.Name</h5>
                                        </div>
                                        <i class="@subject.Icon fs-1 text-@subject.ColorClass"></i>
                                    </div>
                                    <div class="card-body">
                                        <p class="card-text">@subject.Description</p>
                                        <div class="row g-3">
                                            <div class="col-6">
                                                <div class="d-flex align-items-center">
                                                    <i class="ti ti-clock me-2"></i>
                                                    <span>@subject.Duration</span>
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="d-flex align-items-center">
                                                    <i class="ti ti-book me-2"></i>
                                                    <span>@subject.TotalLessons bài</span>
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="d-flex align-items-center">
                                                    <i class="ti ti-list me-2"></i>
                                                    <span>@subject.TotalLists danh sách</span>
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="d-flex align-items-center">
                                                    <i class="ti ti-checkbox me-2"></i>
                                                    <span>@subject.TotalExams bài thi</span>
                                                </div>
                                            </div>
                                        </div>
                                        
                                        <!-- Progress bar -->
                                        @if (subject.ProgressPercentage > 0)
                                        {
                                            <div class="mt-3">
                                                <div class="d-flex justify-content-between mb-1">
                                                    <span class="text-muted">Tiến độ</span>
                                                    <span class="text-muted">@subject.ProgressPercentage.ToString("0.0")%</span>
                                                </div>
                                                <div class="progress" style="height: 6px;">
                                                    <div class="progress-bar bg-@subject.ColorClass" 
                                                         role="progressbar" 
                                                         style="width: @subject.ProgressPercentage%" 
                                                         aria-valuenow="@subject.ProgressPercentage" 
                                                         aria-valuemin="0" 
                                                         aria-valuemax="100"></div>
                                                </div>
                                            </div>
                                        }
                                    </div>
                                    <div class="card-footer">
                                        <div class="d-grid gap-2">
                                            <a asp-action="Details" asp-route-id="@subject.Id" 
                                               class="btn btn-@subject.ColorClass">
                                                <i class="ti ti-eye me-2"></i>Xem chi tiết
                                            </a>
                                            @if (subject.ProgressPercentage == 0)
                                            {
                                                <a href="#" class="btn btn-outline-@subject.ColorClass">
                                                    <i class="ti ti-play me-2"></i>Bắt đầu học
                                                </a>
                                            }
                                            else if (subject.ProgressPercentage < 100)
                                            {
                                                <a href="#" class="btn btn-outline-@subject.ColorClass">
                                                    <i class="ti ti-refresh me-2"></i>Tiếp tục
                                                </a>
                                            }
                                            else
                                            {
                                                <a href="#" class="btn btn-outline-success">
                                                    <i class="ti ti-check me-2"></i>Đã hoàn thành
                                                </a>
                                            }
                                        </div>
                                    </div>
                                </div>
                            </div>
                        }
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

#### **Shared/_ExamTaskPanel.cshtml** (Reusable Component)
```html
@model ExamTaskViewModel

<div class="card task-card mb-3" data-task-id="@Model.Id">
    <div class="card-header d-flex justify-content-between align-items-center">
        <div>
            <span class="badge bg-@Model.TaskTypeColor">@Model.TaskType</span>
            <h6 class="card-title mb-0">@Model.Name</h6>
        </div>
        <span class="text-muted">@Model.MaxScore điểm</span>
    </div>
    <div class="card-body">
        <p class="card-text">@Model.Description</p>
        
        @if (!string.IsNullOrEmpty(Model.Instructions))
        {
            <div class="alert alert-info">
                <h6><i class="ti ti-info-circle me-2"></i>Hướng dẫn:</h6>
                <p class="mb-0">@Html.Raw(Model.Instructions.Replace("\n", "<br>"))</p>
            </div>
        }
        
        <!-- Task actions -->
        <div class="task-actions mt-3">
            @if (Model.IsCompleted)
            {
                <span class="badge bg-success">
                    <i class="ti ti-check me-1"></i>Đã hoàn thành
                </span>
            }
            else
            {
                <button type="button" class="btn btn-sm btn-primary mark-complete-btn">
                    <i class="ti ti-check me-1"></i>Đánh dấu hoàn thành
                </button>
            }
            
            <button type="button" class="btn btn-sm btn-outline-secondary show-hint-btn">
                <i class="ti ti-help me-1"></i>Gợi ý
            </button>
        </div>
    </div>
</div>

<script>
    $(document).ready(function() {
        $('.mark-complete-btn').click(function() {
            const taskId = $(this).closest('.task-card').data('task-id');
            // AJAX call to mark task as complete
        });
        
        $('.show-hint-btn').click(function() {
            // Show hint modal
        });
    });
</script>
```

### 14.4. Utility Files Implementation

#### **TaskGenerator.cs** (Auto-generation Tool)
```csharp
using DATMOS.Core.Entities;
using System.Text.Json;

namespace DATMOS.Data.Utilities
{
    public class TaskGenerator
    {
        private readonly List<string> _wordTaskTemplates = new()
        {
            "Định dạng {element} thành {format}",
            "Chèn {object} vào vị trí {location}",
            "Thay đổi {property} của {element} thành {value}",
            "Áp dụng {style} cho {element}",
            "Tạo {object} mới với {parameters}"
        };

        private readonly List<string> _excelTaskTemplates = new()
        {
            "Tạo công thức tính {calculation}",
            "Áp dụng định dạng có điều kiện cho {range}",
            "Tạo biểu đồ {chartType} cho dữ liệu {data}",
            "Sử dụng hàm {function} để {action}",
            "Thiết lập {setting} cho {element}"
        };

        private readonly List<string> _powerPointTaskTemplates = new()
        {
            "Thêm hiệu ứng {effect} cho {element}",
            "Chỉnh sửa bố cục {layout} của slide",
            "Chèn {mediaType} vào slide {slideNumber}",
            "Áp dụng theme {themeName}",
            "Thiết lập chuyển tiếp {transition} giữa các slide"
        };

        public List<ExamTask> GenerateTasksForProject(int projectId, string subjectType, int numberOfTasks = 5)
        {
            var tasks = new List<ExamTask>();
            var templates = GetTemplatesBySubject(subjectType);
            
            for (int i = 1; i <= numberOfTasks; i++)
            {
                var template = templates[new Random().Next(templates.Count)];
                var taskName = GenerateTaskName(template, i);
                
                tasks.Add(new ExamTask
                {
                    ExamProjectId = projectId,
                    Name = taskName,
                    Description = GenerateDescription(taskName, subjectType),
                    Instructions = GenerateInstructions(taskName),
                    OrderIndex = i,
                    MaxScore = 100.0 / 6.0,
                    TaskType = InferTaskType(taskName),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            // Add the mandatory "Save & Close" task
            tasks.Add(new ExamTask
            {
                ExamProjectId = projectId,
                Name = "Task 6: Save & Close",
                Description = "Lưu tài liệu và đóng cửa sổ làm việc.",
                Instructions = "Bước 1: Nhấn vào biểu tượng Save trên thanh Quick Access Toolbar.\nBước 2: Vào File > Close để đóng tài liệu.",
                OrderIndex = 6,
                MaxScore = 100.0 / 6.0,
                TaskType = "File",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            
            return tasks;
        }

        private List<string> GetTemplatesBySubject(string subjectType)
        {
            return subjectType.ToLower() switch
            {
                "word" => _wordTaskTemplates,
                "excel" => _excelTaskTemplates,
                "powerpoint" => _powerPointTaskTemplates,
                _ => _wordTaskTemplates
            };
        }

        private string GenerateTaskName(string template, int taskNumber)
        {
            var elements = new[] { "văn bản", "bảng", "hình ảnh", "biểu đồ", "tiêu đề" };
            var formats = new[] { "in đậm", "màu đỏ", "cỡ chữ 14", "căn giữa", "gạch chân" };
            var objects = new[] { "hình ảnh", "bảng", "biểu đồ", "SmartArt", "Text Box" };
            var locations = new[] { "đầu trang", "cuối trang", "bên trái", "bên phải", "giữa" };
            
            var random = new Random();
            var taskName = template
                .Replace("{element}", elements[random.Next(elements.Length)])
                .Replace("{format}", formats[random.Next(formats.Length)])
                .Replace("{object}", objects[random.Next(objects.Length)])
                .Replace("{location}", locations[random.Next(locations.Length)])
                .Replace("{property}", "màu sắc")
                .Replace("{value}", "xanh dương")
                .Replace("{style}", "Heading 1")
                .Replace("{parameters}", "kiểu Modern");
            
            return $"Task {taskNumber}: {taskName}";
        }

        private string GenerateDescription(string taskName, string subjectType)
        {
            return $"Thực hiện yêu cầu: {taskName}. Đây là bài tập thực hành cho {subjectType}.";
        }

        private string GenerateInstructions(string taskName)
        {
            return $"Bước 1: Mở tài liệu cần chỉnh sửa.\nBước 2: Tìm đối tượng cần xử lý.\nBước 3: Thực hiện thao tác theo yêu cầu: {taskName}.\nBước 4: Kiểm tra kết quả.";
        }

        private string InferTaskType(string taskName)
        {
            if (taskName.Contains("định dạng") || taskName.Contains("format")) return "Format";
            if (taskName.Contains("chèn") || taskName.Contains("insert")) return "Insert";
            if (taskName.Contains("bảng") || taskName.Contains("table")) return "Table";
            if (taskName.Contains("biểu đồ") || taskName.Contains("chart")) return "Chart";
            if (taskName.Contains("công thức") || taskName.Contains("formula")) return "Formula";
            return "General";
        }
    }
}
```

## 15. STEP-BY-STEP IMPLEMENTATION PROCESS

### Phase 1: Data Validation & Preparation (Week 1)

#### **Day 1-2: Database Setup & Validation**
1. **Run database migrations:**
   ```bash
   cd DATMOS.Data
   dotnet ef database update
   ```

2. **Create validation script:** `ValidateExamData.sql`
   ```sql
   -- Check data completeness
   SELECT 'ExamSubjects' as TableName, COUNT(*) as RecordCount FROM "ExamSubjects";
   SELECT 'ExamLists' as TableName, COUNT(*) as RecordCount FROM "ExamLists";
   SELECT 'ExamProjects' as TableName, COUNT(*) as RecordCount FROM "ExamProjects";
   SELECT 'ExamTasks' as TableName, COUNT(*) as RecordCount FROM "ExamTasks";
   
   -- Check relationships
   SELECT es.Name as Subject, COUNT(el.Id) as ListCount
   FROM "ExamSubjects" es
   LEFT JOIN "ExamLists" el ON es.Id = el.SubjectId
   GROUP BY es.Id, es.Name;
   
   SELECT el.Name as ExamList, COUNT(ep.Id) as ProjectCount
   FROM "ExamLists" el
   LEFT JOIN "ExamProjects" ep ON el.Id = ep.ExamListId
   GROUP BY el.Id, el.Name;
   ```

3. **Fix ExamListSeeder DbSet reference:**
   ```csharp
   // Change from:
   if (context.Exams.Any())
   // To:
   if (context.ExamLists.Any())
   
   // And:
   context.Exams.Add(examList);
   // To:
   context.ExamLists.Add(examList);
   ```

#### **Day 3-4: Data Gap Analysis**
1. **Create gap analysis report:**
   ```csharp
   // Script to identify missing tasks
   var projectsWithoutTasks = await context.ExamProjects
       .Where(p => !p.ExamTasks.Any())
       .Select(p => new { p.Id, p.Name, p.ExamList.Name })
       .ToListAsync();
   
   var projectsWithIncompleteTasks = await context.ExamProjects
       .Where(p => p.ExamTasks.Count() < 6)
       .Select(p => new { 
           p.Id, 
           p.Name, 
           TaskCount = p.ExamTasks.Count(),
           MissingTasks = 6 - p.ExamTasks.Count()
       })
       .ToListAsync();
   ```

2. **Calculate exact missing task count:**
   - Total projects: 54
   - Projects with JSON tasks: 21
   - Projects needing tasks: 33
   - Tasks per project: 6
   - Total missing tasks: 33 × 6 = 198 tasks

### Phase 2: Data Completion & Enhancement (Week 2-3)

#### **Week 2: Task Generation & Seeder Enhancement**
1. **Implement TaskGenerator.cs** (as shown in section 14.4)
2. **Enhance ExamTaskSeeder.cs:**
   ```csharp
   public static async Task SeedAsync(AppDbContext context)
   {
       if (await context.ExamTasks.AnyAsync()) return;
       
       var projects = await context.ExamProjects.ToListAsync();
       var taskGenerator = new TaskGenerator();
       var allTasks = new List<ExamTask>();
       
       foreach (var project in projects)
       {
           // Try to get tasks from JSON first
           var jsonTasks = await GetTasksFromJson(project.Id);
           
           if (jsonTasks.Any())
           {
               allTasks.AddRange(jsonTasks);
           }
           else
           {
               // Generate tasks automatically
               var subjectType = await GetSubjectTypeForProject(context, project.Id);
               var generatedTasks = taskGenerator.GenerateTasksForProject(
                   project.Id, subjectType);
               allTasks.AddRange(generatedTasks);
           }
       }
       
       await context.ExamTasks.AddRangeAsync(allTasks);
       await context.SaveChangesAsync();
   }
   ```

3. **Create data quality validation:**
   ```csharp
   public class ExamDataValidator
   {
       public ValidationResult ValidateExamTask(ExamTask task)
       {
           var result = new ValidationResult();
           
           if (string.IsNullOrEmpty(task.Instructions))
               result.Errors.Add("Instructions are required");
           
           if (task.MaxScore <= 0 || task.MaxScore > 100)
               result.Errors.Add("MaxScore must be between 0 and 100");
           
           if (string.IsNullOrEmpty(task.TaskType))
               result.Errors.Add("TaskType is required");
           
           return result;
       }
   }
   ```

#### **Week 3: Services & Business Logic**
1. **Enhance existing services:**
   ```csharp
   public class ExamSubjectService : IExamSubjectService
   {
       public async Task<ExamSubject> GetExamSubjectWithHierarchyAsync(int id)
       {
           return await _context.ExamSubjects
               .Include(s => s.ExamLists)
                   .ThenInclude(l => l.ExamProjects)
                       .ThenInclude(p => p.ExamTasks)
               .FirstOrDefaultAsync(s => s.Id == id);
       }
       
       public async Task<List<ExamSubjectProgress>> GetUserProgressAsync(string userId)
       {
           // Calculate user progress for each subject
       }
   }
   ```

2. **Create new services:**
   - `ExamStatisticsService.cs` - For analytics and reporting
   - `ExamScoringService.cs` - For automatic scoring
   - `ExamProgressService.cs` - For tracking user progress

### Phase 3: UI/UX Development (Week 4-6)

#### **Week 4: Controllers & ViewModels**
1. **Create all 5 controllers** (as shown in section 14.1)
2. **Create all 6 ViewModels** (as shown in section 14.2)
3. **Set up dependency injection in Program.cs:**
   ```csharp
   builder.Services.AddScoped<IExamSubjectService, ExamSubjectService>();
   builder.Services.AddScoped<IExamListService, ExamListService>();
   builder.Services.AddScoped<ExamStatisticsService>();
   builder.Services.AddScoped<ExamProgressService>();
   ```

#### **Week 5: Views Development**
1. **Create layout structure:**
   ```bash
   mkdir -p Areas/Customer/Views/ExamSubject
   mkdir -p Areas/Customer/Views/ExamList
   mkdir -p Areas/Customer/Views/ExamProject
   mkdir -p Areas/Customer/Views/Shared
   ```

2. **Implement all 7 views** (starting with Index.cshtml as shown in section 14.3)
3. **Create shared components:**
   - `_ExamNavigation.cshtml` - Breadcrumb navigation
   - `_ExamProgressBar.cshtml` - Progress tracking
   - `_ExamTimer.cshtml` - Countdown timer for exams

#### **Week 6: JavaScript & API Integration**
1. **Create exam-taking JavaScript:**
   ```javascript
   // exam-taking.js
   class ExamTaking {
       constructor(examId, timeLimit) {
           this.examId = examId;
           this.timeRemaining = timeLimit;
           this.currentProjectIndex = 0;
           this.answers = {};
       }
       
       startTimer() {
           this.timer = setInterval(() => {
               this.timeRemaining--;
               this.updateTimerDisplay();
               if (this.timeRemaining <= 0) {
                   this.submitExam();
               }
           }, 1000);
       }
       
       async submitTask(taskId, answer) {
           this.answers[taskId] = answer;
           await this.saveProgress();
       }
       
       async submitExam() {
           const result = await this.calculateScore();
           await this.saveExamResult(result);
           this.showResults(result);
       }
   }
   ```

2. **Implement API endpoints:**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class ExamApiController : ControllerBase
   {
       [HttpPost("submit-task")]
       public async Task<IActionResult> SubmitTask([FromBody] TaskSubmissionDto submission)
       {
           // Save task submission
           return Ok(new { success = true, score = calculatedScore });
       }
       
       [HttpGet("progress/{examId}")]
       public async Task<IActionResult> GetProgress(int examId)
       {
           // Get exam progress
           return Ok(progressData);
       }
   }
   ```

## 16. TESTING STRATEGY

### 16.1. Unit Tests
```csharp
[TestClass]
public class ExamSubjectServiceTests
{
    [TestMethod]
    public async Task GetExamSubjectById_ReturnsCorrectSubject()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>();
        var service = new ExamSubjectService(mockContext.Object);
        
        // Act
        var result = await service.GetExamSubjectByIdAsync(1);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }
    
    [TestMethod]
    public void TaskGenerator_GeneratesCorrectNumberOfTasks()
    {
        // Arrange
        var generator = new TaskGenerator();
        
        // Act
        var tasks = generator.GenerateTasksForProject(1, "Word", 5);
        
        // Assert
        Assert.AreEqual(6, tasks.Count); // 5 generated + 1 Save & Close
    }
}
```

### 16.2. Integration Tests
```csharp
[TestClass]
public class ExamControllerIntegrationTests
{
    [TestMethod]
    public async Task ExamSubjectIndex_ReturnsViewWithSubjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Customer/ExamSubject");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("text/html; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
}
```

### 16.3. End-to-End Tests
```csharp
[TestClass]
public class ExamTakingE2ETests
{
    [TestMethod]
    public async Task CompleteExamFlow_WorksCorrectly()
    {
        // 1. Login as test user
        // 2. Navigate to exam subject
        // 3. Start exam
        // 4. Complete tasks
        // 5. Submit exam
        // 6. Verify results
    }
}
```

## 17. DEPLOYMENT CHECKLIST

### Pre-deployment:
- [ ] All database migrations applied
- [ ] All seeders executed successfully
- [ ] Data validation passed (324+ tasks created)
- [ ] Unit tests passing (90%+ coverage)
- [ ] Integration tests passing
- [ ] Performance tests completed
- [ ] Security review completed

### Deployment Steps:
1. **Database deployment:**
   ```bash
   dotnet ef migrations script --idempotent --output migration.sql
   psql -d datmos_db -f migration.sql
   ```

2. **Application deployment:**
   ```bash
   dotnet publish -c Release -o ./publish
   # Deploy to IIS/Apache/Nginx
   ```

3. **Post-deployment verification:**
   - Verify all endpoints respond correctly
   - Test exam taking flow
   - Verify data persistence
   - Check performance metrics

### Rollback Plan:
1. **Database rollback:** Restore from backup
2. **Application rollback:** Redeploy previous version
3. **Configuration rollback:** Revert appsettings changes

## 18. MAINTENANCE & MONITORING

### Monitoring Setup:
1. **Application Insights integration**
2. **Database performance monitoring**
3. **User activity tracking**
4. **Error logging and alerting**

### Regular Maintenance Tasks:
1. **Weekly:** Check data integrity
2. **Monthly:** Performance optimization
3. **Quarterly:** Content updates (new exam tasks)
4. **Yearly:** Major version updates

### Backup Strategy:
1. **Database:** Daily full backup + transaction log backups
2. **Files:** Weekly backup of JSON data and uploads
3. **Configuration:** Version-controlled in Git

---

**Document Version:** 3.0  
**Last Updated:** 30/12/2025  
**Author:** System Analysis  
**Status:** Complete Implementation Guide - Ready for Development
