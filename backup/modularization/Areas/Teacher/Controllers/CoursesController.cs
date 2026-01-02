using DATMOS.Web.Areas.Customer.ViewModels;
using DATMOS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _coursesService;

        public CoursesController(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        // GET: Teacher/Courses
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả khóa học (tạm thời)
            // Sau này có thể lọc theo giáo viên đang đăng nhập
            var courses = await _coursesService.GetAllCoursesAsync();
            
            // Chuyển đổi sang ViewModel cho Teacher
            var teacherCourses = courses.Select(c => new TeacherCourseViewModel
            {
                Id = c.Id,
                Code = c.Code ?? string.Empty,
                Name = c.Name ?? string.Empty,
                Description = c.Description ?? string.Empty,
                Level = c.Level ?? string.Empty,
                Duration = c.Duration ?? string.Empty,
                Instructor = c.Instructor ?? string.Empty,
                StudentCount = c.EnrolledStudents,
                Rating = c.Rating,
                Progress = CalculateProgress(c.Id), // Giả định tính toán tiến độ
                AssignmentCount = c.TotalTasks,
                CompletedAssignments = (int)(c.TotalTasks * 0.7), // Giả định 70% hoàn thành
                LastActivity = System.DateTime.Now.AddDays(-c.Id) // Giả định
            }).ToList();

            return View(teacherCourses);
        }

        // GET: Teacher/Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _coursesService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var courseDetails = await _coursesService.GetCourseDetailsAsync(id);
            var students = GetMockStudents(); // Tạm thời dùng mock data

            var viewModel = new TeacherCourseDetailsViewModel
            {
                Course = new TeacherCourseViewModel
                {
                    Id = course.Id,
                    Code = course.Code ?? string.Empty,
                    Name = course.Name ?? string.Empty,
                    Description = course.Description ?? string.Empty,
                    Level = course.Level ?? string.Empty,
                    Duration = course.Duration ?? string.Empty,
                    Instructor = course.Instructor ?? string.Empty,
                    StudentCount = course.EnrolledStudents,
                    Rating = course.Rating,
                    Progress = CalculateProgress(course.Id),
                    AssignmentCount = course.TotalTasks,
                    CompletedAssignments = (int)(course.TotalTasks * 0.7),
                    LastActivity = System.DateTime.Now.AddDays(-course.Id)
                },
                Students = students,
                Statistics = new TeacherCourseStatistics
                {
                    TotalStudents = course.EnrolledStudents,
                    AverageProgress = 65.5, // Mock data
                    AssignmentCompletionRate = 70.0, // Mock data
                    AverageScore = 8.2, // Mock data
                    ActiveStudents = (int)(course.EnrolledStudents * 0.8) // Mock data
                },
                Syllabus = courseDetails?.Syllabus ?? new List<string>(),
                RecentActivities = GetMockRecentActivities()
            };

            return View(viewModel);
        }

        // GET: Teacher/Courses/Students/5
        public async Task<IActionResult> Students(int id)
        {
            var course = await _coursesService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var students = GetMockStudents();
            ViewData["CourseName"] = course.Name;
            ViewData["CourseId"] = id;

            return View(students);
        }

        // GET: Teacher/Courses/Analytics/5
        public async Task<IActionResult> Analytics(int id)
        {
            var course = await _coursesService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var analytics = new TeacherCourseAnalyticsViewModel
            {
                CourseId = id,
                CourseName = course.Name ?? string.Empty,
                EnrollmentTrend = GetMockEnrollmentTrend(),
                ProgressDistribution = GetMockProgressDistribution(),
                AssignmentScores = GetMockAssignmentScores(),
                StudentEngagement = GetMockStudentEngagement()
            };

            return View(analytics);
        }

        // Helper methods (tạm thời dùng mock data)
        private double CalculateProgress(int courseId)
        {
            // Giả định tính toán tiến độ dựa trên courseId
            return (courseId % 10) * 10.0;
        }

        private List<StudentViewModel> GetMockStudents()
        {
            return new List<StudentViewModel>
            {
                new StudentViewModel { Id = 1, Name = "Nguyễn Văn A", Email = "a@example.com", Progress = 85.5, LastLogin = System.DateTime.Now.AddDays(-1), AssignmentScore = 9.2 },
                new StudentViewModel { Id = 2, Name = "Trần Thị B", Email = "b@example.com", Progress = 72.3, LastLogin = System.DateTime.Now.AddDays(-2), AssignmentScore = 8.5 },
                new StudentViewModel { Id = 3, Name = "Lê Văn C", Email = "c@example.com", Progress = 90.1, LastLogin = System.DateTime.Now.AddHours(-12), AssignmentScore = 9.8 },
                new StudentViewModel { Id = 4, Name = "Phạm Thị D", Email = "d@example.com", Progress = 68.7, LastLogin = System.DateTime.Now.AddDays(-3), AssignmentScore = 7.9 },
                new StudentViewModel { Id = 5, Name = "Hoàng Văn E", Email = "e@example.com", Progress = 95.2, LastLogin = System.DateTime.Now, AssignmentScore = 9.5 }
            };
        }

        private List<RecentActivityViewModel> GetMockRecentActivities()
        {
            return new List<RecentActivityViewModel>
            {
                new RecentActivityViewModel { StudentName = "Nguyễn Văn A", Activity = "Hoàn thành bài tập 1", Time = System.DateTime.Now.AddHours(-2), Score = 9.5 },
                new RecentActivityViewModel { StudentName = "Trần Thị B", Activity = "Nộp bài tập 2", Time = System.DateTime.Now.AddHours(-5), Score = 8.0 },
                new RecentActivityViewModel { StudentName = "Lê Văn C", Activity = "Tham gia thảo luận", Time = System.DateTime.Now.AddHours(-8), Score = null },
                new RecentActivityViewModel { StudentName = "Phạm Thị D", Activity = "Xem video bài giảng", Time = System.DateTime.Now.AddHours(-12), Score = null }
            };
        }

        private List<EnrollmentDataPoint> GetMockEnrollmentTrend()
        {
            return new List<EnrollmentDataPoint>
            {
                new EnrollmentDataPoint { Month = "Tháng 1", Enrollment = 15 },
                new EnrollmentDataPoint { Month = "Tháng 2", Enrollment = 28 },
                new EnrollmentDataPoint { Month = "Tháng 3", Enrollment = 42 },
                new EnrollmentDataPoint { Month = "Tháng 4", Enrollment = 35 },
                new EnrollmentDataPoint { Month = "Tháng 5", Enrollment = 50 }
            };
        }

        private List<ProgressDistribution> GetMockProgressDistribution()
        {
            return new List<ProgressDistribution>
            {
                new ProgressDistribution { Range = "0-20%", Count = 2 },
                new ProgressDistribution { Range = "21-40%", Count = 3 },
                new ProgressDistribution { Range = "41-60%", Count = 8 },
                new ProgressDistribution { Range = "61-80%", Count = 12 },
                new ProgressDistribution { Range = "81-100%", Count = 25 }
            };
        }

        private List<AssignmentScore> GetMockAssignmentScores()
        {
            return new List<AssignmentScore>
            {
                new AssignmentScore { AssignmentName = "Bài tập 1", AverageScore = 8.5, MaxScore = 10, MinScore = 6.0 },
                new AssignmentScore { AssignmentName = "Bài tập 2", AverageScore = 7.8, MaxScore = 10, MinScore = 5.5 },
                new AssignmentScore { AssignmentName = "Bài tập 3", AverageScore = 9.2, MaxScore = 10, MinScore = 7.0 },
                new AssignmentScore { AssignmentName = "Bài tập 4", AverageScore = 8.9, MaxScore = 10, MinScore = 6.5 }
            };
        }

        private List<StudentEngagement> GetMockStudentEngagement()
        {
            return new List<StudentEngagement>
            {
                new StudentEngagement { Day = "Thứ 2", LoginCount = 45, AssignmentSubmissions = 32 },
                new StudentEngagement { Day = "Thứ 3", LoginCount = 52, AssignmentSubmissions = 38 },
                new StudentEngagement { Day = "Thứ 4", LoginCount = 48, AssignmentSubmissions = 35 },
                new StudentEngagement { Day = "Thứ 5", LoginCount = 55, AssignmentSubmissions = 42 },
                new StudentEngagement { Day = "Thứ 6", LoginCount = 60, AssignmentSubmissions = 48 },
                new StudentEngagement { Day = "Thứ 7", LoginCount = 40, AssignmentSubmissions = 28 },
                new StudentEngagement { Day = "Chủ nhật", LoginCount = 35, AssignmentSubmissions = 22 }
            };
        }
    }

    // ViewModels cho Teacher Area
    public class TeacherCourseViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double Rating { get; set; }
        public double Progress { get; set; }
        public int AssignmentCount { get; set; }
        public int CompletedAssignments { get; set; }
        public System.DateTime LastActivity { get; set; }
    }

    public class TeacherCourseDetailsViewModel
    {
        public TeacherCourseViewModel Course { get; set; } = new TeacherCourseViewModel();
        public List<StudentViewModel> Students { get; set; } = new List<StudentViewModel>();
        public TeacherCourseStatistics Statistics { get; set; } = new TeacherCourseStatistics();
        public List<string> Syllabus { get; set; } = new List<string>();
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new List<RecentActivityViewModel>();
    }

    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public double Progress { get; set; }
        public System.DateTime LastLogin { get; set; }
        public double? AssignmentScore { get; set; }
    }

    public class TeacherCourseStatistics
    {
        public int TotalStudents { get; set; }
        public double AverageProgress { get; set; }
        public double AssignmentCompletionRate { get; set; }
        public double AverageScore { get; set; }
        public int ActiveStudents { get; set; }
    }

    public class RecentActivityViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public System.DateTime Time { get; set; }
        public double? Score { get; set; }
    }

    public class TeacherCourseAnalyticsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<EnrollmentDataPoint> EnrollmentTrend { get; set; } = new List<EnrollmentDataPoint>();
        public List<ProgressDistribution> ProgressDistribution { get; set; } = new List<ProgressDistribution>();
        public List<AssignmentScore> AssignmentScores { get; set; } = new List<AssignmentScore>();
        public List<StudentEngagement> StudentEngagement { get; set; } = new List<StudentEngagement>();
    }

    public class EnrollmentDataPoint
    {
        public string Month { get; set; } = string.Empty;
        public int Enrollment { get; set; }
    }

    public class ProgressDistribution
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AssignmentScore
    {
        public string AssignmentName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public double MaxScore { get; set; }
        public double MinScore { get; set; }
    }

    public class StudentEngagement
    {
        public string Day { get; set; } = string.Empty;
        public int LoginCount { get; set; }
        public int AssignmentSubmissions { get; set; }
    }
}
