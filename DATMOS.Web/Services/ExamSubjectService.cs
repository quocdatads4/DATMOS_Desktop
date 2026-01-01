using DATMOS.Web.Areas.Customer.ViewModels;
using DATMOS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public class ExamSubjectService : IExamSubjectService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _jsonFilePath;
        private List<ExamSubjectViewModel>? _subjectsCache;
        private DateTime _lastCacheUpdate;

        public ExamSubjectService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _jsonFilePath = Path.Combine(_env.WebRootPath, "areas", "customer", "json", "exam-subject.json");
            _subjectsCache = null;
            _lastCacheUpdate = DateTime.MinValue;
        }

        public async Task<List<ExamSubjectViewModel>> GetAllSubjectsAsync()
        {
            // Try to get from database first
            try
            {
                var subjects = await _context.ExamSubjects
                    .AsNoTracking()
                    .Select(s => new ExamSubjectViewModel
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        ShortName = s.ShortName,
                        Title = s.Title,
                        Description = s.Description,
                        Icon = s.Icon,
                        ColorClass = s.ColorClass,
                        Duration = s.Duration,
                        TotalLessons = s.TotalLessons,
                        TotalExams = s.TotalExams,
                        BadgeText = s.BadgeText,
                        BadgeIcon = s.BadgeIcon,
                        BadgeColorClass = s.BadgeColorClass
                    })
                    .ToListAsync();

                if (subjects != null && subjects.Any())
                {
                    return subjects;
                }
            }
            catch (Exception ex)
            {
                // Log error (in production use ILogger)
                Console.WriteLine($"Error loading exam subjects from database: {ex.Message}");
            }

            // Fallback to JSON file if database fails or empty
            await EnsureCacheIsLoadedAsync();
            return _subjectsCache ?? new List<ExamSubjectViewModel>();
        }

        public async Task<ExamSubjectViewModel?> GetSubjectByIdAsync(int id)
        {
            // Try to get from database first
            try
            {
                var subject = await _context.ExamSubjects
                    .AsNoTracking()
                    .Where(s => s.Id == id)
                    .Select(s => new ExamSubjectViewModel
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        ShortName = s.ShortName,
                        Title = s.Title,
                        Description = s.Description,
                        Icon = s.Icon,
                        ColorClass = s.ColorClass,
                        Duration = s.Duration,
                        TotalLessons = s.TotalLessons,
                        TotalExams = s.TotalExams,
                        BadgeText = s.BadgeText,
                        BadgeIcon = s.BadgeIcon,
                        BadgeColorClass = s.BadgeColorClass
                    })
                    .FirstOrDefaultAsync();

                if (subject != null)
                {
                    return subject;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading exam subject from database: {ex.Message}");
            }

            // Fallback to JSON file
            await EnsureCacheIsLoadedAsync();
            return _subjectsCache?.Find(s => s.Id == id);
        }

        public async Task<ExamSubjectViewModel?> GetSubjectByCodeAsync(string subjectCode)
        {
            if (string.IsNullOrEmpty(subjectCode))
                return null;

            // Try to get from database first
            try
            {
                var subject = await _context.ExamSubjects
                    .AsNoTracking()
                    .Where(s => s.Code == subjectCode)
                    .Select(s => new ExamSubjectViewModel
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        ShortName = s.ShortName,
                        Title = s.Title,
                        Description = s.Description,
                        Icon = s.Icon,
                        ColorClass = s.ColorClass,
                        Duration = s.Duration,
                        TotalLessons = s.TotalLessons,
                        TotalExams = s.TotalExams,
                        BadgeText = s.BadgeText,
                        BadgeIcon = s.BadgeIcon,
                        BadgeColorClass = s.BadgeColorClass
                    })
                    .FirstOrDefaultAsync();

                if (subject != null)
                {
                    return subject;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading exam subject from database: {ex.Message}");
            }

            // Fallback to JSON file
            await EnsureCacheIsLoadedAsync();
            return _subjectsCache?.Find(s => s.Code.Equals(subjectCode, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ExamSubjectDetailsViewModel?> GetSubjectDetailsAsync(int id)
        {
            var subject = await GetSubjectByIdAsync(id);
            if (subject == null)
                return null;

            // Trong thực tế, có thể lấy thêm thông tin từ các nguồn khác
            // Ví dụ: danh sách khóa học, bài thi, thống kê, v.v.
            return new ExamSubjectDetailsViewModel
            {
                Subject = subject,
                RelatedCourses = await GetRelatedCoursesAsync(id),
                Statistics = new ExamSubjectStatistics
                {
                    TotalStudents = 150,
                    AverageScore = 85.5,
                    CompletionRate = 78.2
                }
            };
        }

        private async Task EnsureCacheIsLoadedAsync()
        {
            // Cache trong 5 phút
            if (_subjectsCache == null || (DateTime.Now - _lastCacheUpdate).TotalMinutes > 5)
            {
                await LoadSubjectsFromJsonAsync();
                _lastCacheUpdate = DateTime.Now;
            }
        }

        private async Task LoadSubjectsFromJsonAsync()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                {
                    _subjectsCache = new List<ExamSubjectViewModel>();
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(_jsonFilePath);
                var jsonData = JsonSerializer.Deserialize<ExamSubjectJsonData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _subjectsCache = new List<ExamSubjectViewModel>();
                if (jsonData?.Subjects != null)
                {
                    foreach (var subject in jsonData.Subjects)
                    {
                        _subjectsCache.Add(new ExamSubjectViewModel
                        {
                            Id = subject.Id,
                            Code = subject.Code,
                            Name = subject.Name,
                            ShortName = subject.ShortName,
                            Title = subject.Title,
                            Description = subject.Description,
                            Icon = subject.Icon,
                            ColorClass = subject.ColorClass,
                            Duration = subject.Duration,
                            TotalLessons = subject.TotalLessons,
                            TotalExams = subject.TotalExams,
                            BadgeText = subject.Badge?.Text,
                            BadgeIcon = subject.Badge?.Icon,
                            BadgeColorClass = subject.Badge?.ColorClass
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error (trong thực tế nên dùng ILogger)
                Console.WriteLine($"Error loading exam subjects: {ex.Message}");
                _subjectsCache = new List<ExamSubjectViewModel>();
            }
        }

        private async Task<List<CourseViewModel>> GetRelatedCoursesAsync(int subjectId)
        {
            // Mock data - trong thực tế có thể lấy từ database hoặc JSON khác
            await Task.Delay(10); // Simulate async operation

            return new List<CourseViewModel>
            {
                new CourseViewModel
                {
                    Id = subjectId * 100 + 1,
                    SubjectId = subjectId,
                    Name = $"Khóa học cơ bản {subjectId}",
                    Description = "Khóa học cơ bản dành cho người mới bắt đầu",
                    Level = "Cơ bản",
                    Duration = "4 tuần",
                    Price = 0,
                    IsFree = true
                },
                new CourseViewModel
                {
                    Id = subjectId * 100 + 2,
                    SubjectId = subjectId,
                    Name = $"Khóa học nâng cao {subjectId}",
                    Description = "Khóa học nâng cao với các kỹ thuật chuyên sâu",
                    Level = "Nâng cao",
                    Duration = "6 tuần",
                    Price = 299000,
                    IsFree = false
                }
            };
        }

        // Classes for JSON deserialization
        private class ExamSubjectJsonData
        {
            public List<JsonSubject>? Subjects { get; set; }
            public JsonMetadata? Metadata { get; set; }
        }

        private class JsonSubject
        {
            public int Id { get; set; }
            public string? Code { get; set; }
            public string? Name { get; set; }
            public string? ShortName { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Icon { get; set; }
            public string? ColorClass { get; set; }
            public string? Duration { get; set; }
            public int TotalLessons { get; set; }
            public int TotalExams { get; set; }
            public JsonBadge? Badge { get; set; }
        }

        private class JsonBadge
        {
            public string? Text { get; set; }
            public string? Icon { get; set; }
            public string? ColorClass { get; set; }
        }

        private class JsonMetadata
        {
            public string? Version { get; set; }
            public string? LastUpdated { get; set; }
            public int TotalSubjects { get; set; }
        }
    }
}
