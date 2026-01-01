using DATMOS.Core.Entities;
using DATMOS.Data;
using DATMOS.Web.Areas.Customer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CoursesService(IMemoryCache cache, AppDbContext context)
        {
            _cache = cache;
            _context = context;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }

        public async Task<List<CourseViewModel>> GetAllCoursesAsync()
        {
            if (!_cache.TryGetValue("all_courses", out List<CourseViewModel>? courses))
            {
                var courseEntities = await _context.Courses
                    .AsNoTracking()
                    .OrderBy(c => c.Id)
                    .ToListAsync();
                
                courses = courseEntities.Select(c => MapToCourseViewModel(c)).ToList();
                _cache.Set("all_courses", courses, _cacheOptions);
            }
            return courses ?? new List<CourseViewModel>();
        }

        public async Task<CourseViewModel?> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            
            return course != null ? MapToCourseViewModel(course) : null;
        }

        public async Task<CourseDetailsViewModel?> GetCourseDetailsAsync(int id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (course == null)
                return null;

            // Lấy các khóa học liên quan (cùng subject)
            var relatedCourses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.SubjectId == course.SubjectId && c.Id != course.Id)
                .Take(3)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();

            // Tạo syllabus mẫu dựa trên loại khóa học
            var syllabus = GenerateSyllabus(course);

            return new CourseDetailsViewModel
            {
                Course = MapToCourseViewModel(course),
                Syllabus = syllabus,
                Instructor = new InstructorViewModel
                {
                    Name = course.Instructor,
                    Rating = course.Rating,
                    TotalCourses = await _context.Courses
                        .CountAsync(c => c.Instructor == course.Instructor)
                },
                RelatedCourses = relatedCourses,
                Statistics = new CourseStatistics
                {
                    TotalStudents = course.EnrolledStudents,
                    AverageRating = course.Rating,
                    CompletionRate = 85.5, // Mock data - có thể tính từ database sau
                    SatisfactionRate = 92.0 // Mock data
                }
            };
        }

        public async Task<List<CourseViewModel>> GetCoursesBySubjectAsync(int subjectId)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.SubjectId == subjectId)
                .OrderBy(c => c.Id)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();
            
            return courses;
        }

        public async Task<List<CourseViewModel>> GetFreeCoursesAsync()
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.IsFree)
                .OrderBy(c => c.Id)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();
            
            return courses;
        }

        public async Task<List<CourseViewModel>> GetPaidCoursesAsync()
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => !c.IsFree)
                .OrderBy(c => c.Id)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();
            
            return courses;
        }

        public async Task<List<CourseViewModel>> GetCoursesByLevelAsync(string level)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.Level != null && c.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Id)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();
            
            return courses;
        }

        public async Task<List<CourseViewModel>> SearchCoursesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllCoursesAsync();

            var searchTerm = keyword.ToLower();
            
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchTerm)) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm)) ||
                    (c.Code != null && c.Code.ToLower().Contains(searchTerm)) ||
                    (c.Level != null && c.Level.ToLower().Contains(searchTerm)) ||
                    (c.Instructor != null && c.Instructor.ToLower().Contains(searchTerm))
                )
                .OrderBy(c => c.Id)
                .Select(c => MapToCourseViewModel(c))
                .ToListAsync();
            
            return courses;
        }

        public async Task<CourseStatistics> GetCourseStatisticsAsync()
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .ToListAsync();
            
            return new CourseStatistics
            {
                TotalCourses = courses.Count,
                FreeCourses = courses.Count(c => c.IsFree),
                PaidCourses = courses.Count(c => !c.IsFree),
                TotalStudents = courses.Sum(c => c.EnrolledStudents),
                AverageRating = courses.Any() ? courses.Average(c => c.Rating) : 0,
                TotalRevenue = courses.Where(c => !c.IsFree).Sum(c => c.Price * c.EnrolledStudents * 0.1m) // Mock revenue calculation
            };
        }

        private CourseViewModel MapToCourseViewModel(Course course)
        {
            return new CourseViewModel
            {
                Id = course.Id,
                SubjectId = course.SubjectId,
                Code = course.Code,
                Name = course.Name,
                ShortName = course.ShortName,
                Title = course.Title,
                Description = course.Description,
                Icon = course.Icon,
                ColorClass = course.ColorClass,
                Level = course.Level,
                Duration = course.Duration,
                TotalLessons = course.TotalLessons,
                TotalProjects = course.TotalProjects,
                TotalTasks = course.TotalTasks,
                Price = course.Price,
                IsFree = course.IsFree,
                Instructor = course.Instructor,
                Rating = course.Rating,
                EnrolledStudents = course.EnrolledStudents,
                Badge = GenerateBadge(course) // Tạo badge dựa trên thông tin khóa học
            };
        }

        private BadgeViewModel? GenerateBadge(Course course)
        {
            // Logic tạo badge dựa trên thông tin khóa học
            if (course.IsFree)
            {
                return new BadgeViewModel
                {
                    Text = "Miễn phí",
                    Icon = "ti-gift",
                    ColorClass = "success"
                };
            }
            else if (course.EnrolledStudents > 1000)
            {
                return new BadgeViewModel
                {
                    Text = "Phổ biến",
                    Icon = "ti-crown",
                    ColorClass = "warning"
                };
            }
            else if (course.Rating >= 4.5)
            {
                return new BadgeViewModel
                {
                    Text = "Xuất sắc",
                    Icon = "ti-star",
                    ColorClass = "danger"
                };
            }
            
            return null;
        }

        private List<string> GenerateSyllabus(Course course)
        {
            // Tạo syllabus mẫu dựa trên loại khóa học
            var syllabus = new List<string>();
            
            if (course.Code != null && course.Code.Contains("WD2019"))
            {
                syllabus = new List<string>
                {
                    "Giới thiệu Microsoft Word 2019",
                    "Định dạng văn bản cơ bản",
                    "Làm việc với bảng biểu",
                    "Chèn hình ảnh và đồ họa",
                    "Tạo mục lục và chú thích",
                    "In ấn và xuất bản tài liệu",
                    "Bài tập thực hành và ôn tập"
                };
            }
            else if (course.Code != null && course.Code.Contains("EX2019"))
            {
                syllabus = new List<string>
                {
                    "Giới thiệu Microsoft Excel 2019",
                    "Nhập và định dạng dữ liệu",
                    "Công thức và hàm cơ bản",
                    "Biểu đồ và đồ thị",
                    "Phân tích dữ liệu với PivotTable",
                    "Macro và tự động hóa",
                    "Bài tập thực hành và ôn tập"
                };
            }
            else if (course.Code != null && course.Code.Contains("PP2019"))
            {
                syllabus = new List<string>
                {
                    "Giới thiệu Microsoft PowerPoint 2019",
                    "Tạo slide và bố cục",
                    "Chèn đa phương tiện",
                    "Hiệu ứng và chuyển tiếp",
                    "Trình chiếu và thuyết trình",
                    "Xuất bản và chia sẻ",
                    "Bài tập thực hành và ôn tập"
                };
            }
            else
            {
                // Syllabus mẫu chung
                syllabus = new List<string>
                {
                    "Giới thiệu khóa học",
                    "Kiến thức cơ bản",
                    "Kỹ năng nâng cao",
                    "Thực hành và bài tập",
                    "Ôn tập và kiểm tra",
                    "Dự án cuối khóa"
                };
            }
            
            return syllabus;
        }
    }
}
