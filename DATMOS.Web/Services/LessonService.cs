using DATMOS.Web.Areas.Customer.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public class LessonService : ILessonService
    {
        private readonly IMemoryCache _cache;
        private readonly string _lessonsFilePath;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public LessonService(IMemoryCache cache)
        {
            _cache = cache;
            _lessonsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "areas", "customer", "json", "lessons.json");
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }

        public async Task<List<LessonViewModel>> GetAllLessonsAsync()
        {
            if (!_cache.TryGetValue("all_lessons", out List<LessonViewModel>? lessons))
            {
                var lessonsData = await LoadLessonsDataAsync();
                lessons = lessonsData?.Lessons?.Select(l => MapToLessonViewModel(l)).ToList() ?? new List<LessonViewModel>();
                _cache.Set("all_lessons", lessons, _cacheOptions);
            }
            return lessons ?? new List<LessonViewModel>();
        }

        public async Task<LessonViewModel?> GetLessonByIdAsync(int id)
        {
            var lessons = await GetAllLessonsAsync();
            return lessons.FirstOrDefault(l => l.Id == id);
        }

        public async Task<LessonDetailsViewModel?> GetLessonDetailsAsync(int id)
        {
            var lessonsData = await LoadLessonsDataAsync();
            var lesson = lessonsData?.Lessons?.FirstOrDefault(l => l.Id == id);

            if (lesson == null)
                return null;

            var relatedLessons = lessonsData?.Lessons?
                .Where(l => l.CourseId == lesson.CourseId && l.Id != lesson.Id)
                .Take(3)
                .Select(l => MapToLessonViewModel(l))
                .ToList() ?? new List<LessonViewModel>();

            var allLessons = await GetAllLessonsAsync();
            var courseLessons = allLessons.Where(l => l.CourseId == lesson.CourseId).ToList();

            return new LessonDetailsViewModel
            {
                Lesson = MapToLessonViewModel(lesson),
                Course = await GetCourseForLessonAsync(lesson.CourseId),
                RelatedLessons = relatedLessons,
                AllResources = lesson.Resources?.Select(r => new ResourceViewModel
                {
                    Type = r.Type,
                    Name = r.Name,
                    Url = r.Url
                }).ToList() ?? new List<ResourceViewModel>(),
                Statistics = new LessonStatistics
                {
                    TotalLessons = courseLessons.Count,
                    FreeLessons = courseLessons.Count(l => l.IsFree),
                    PaidLessons = courseLessons.Count(l => !l.IsFree),
                    TotalDuration = CalculateTotalDuration(courseLessons),
                    TotalCompleted = courseLessons.Count(l => l.Completed),
                    AverageProgress = courseLessons.Any() ? courseLessons.Average(l => l.Progress) : 0,
                    CompletionRate = courseLessons.Count > 0 ? (double)courseLessons.Count(l => l.Completed) / courseLessons.Count * 100 : 0,
                    LessonsByDifficulty = courseLessons
                        .Where(l => l.Difficulty != null)
                        .GroupBy(l => l.Difficulty!)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    LessonsByCourse = allLessons
                        .Where(l => l.CourseCode != null)
                        .GroupBy(l => l.CourseCode!)
                        .ToDictionary(g => g.Key, g => g.Count())
                }
            };
        }

        public async Task<List<LessonViewModel>> GetLessonsByCourseAsync(int courseId)
        {
            var lessons = await GetAllLessonsAsync();
            return lessons.Where(l => l.CourseId == courseId).OrderBy(l => l.Order).ToList();
        }

        public async Task<List<LessonViewModel>> GetFreeLessonsAsync()
        {
            var lessons = await GetAllLessonsAsync();
            return lessons.Where(l => l.IsFree).ToList();
        }

        public async Task<List<LessonViewModel>> GetPaidLessonsAsync()
        {
            var lessons = await GetAllLessonsAsync();
            return lessons.Where(l => !l.IsFree).ToList();
        }

        public async Task<List<LessonViewModel>> GetLessonsByDifficultyAsync(string difficulty)
        {
            var lessons = await GetAllLessonsAsync();
            return lessons.Where(l => l.Difficulty != null && l.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<LessonViewModel>> SearchLessonsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllLessonsAsync();

            var lessons = await GetAllLessonsAsync();
            var searchTerm = keyword.ToLower();
            
            return lessons.Where(l =>
                (l.Name != null && l.Name.ToLower().Contains(searchTerm)) ||
                (l.Description != null && l.Description.ToLower().Contains(searchTerm)) ||
                (l.Code != null && l.Code.ToLower().Contains(searchTerm)) ||
                (l.Title != null && l.Title.ToLower().Contains(searchTerm)) ||
                (l.Objectives != null && l.Objectives.Any(o => o.ToLower().Contains(searchTerm)))
            ).ToList();
        }

        public async Task<LessonStatistics> GetLessonStatisticsAsync()
        {
            var lessons = await GetAllLessonsAsync();
            
            return new LessonStatistics
            {
                TotalLessons = lessons.Count,
                FreeLessons = lessons.Count(l => l.IsFree),
                PaidLessons = lessons.Count(l => !l.IsFree),
                TotalDuration = CalculateTotalDuration(lessons),
                TotalCompleted = lessons.Count(l => l.Completed),
                AverageProgress = lessons.Any() ? lessons.Average(l => l.Progress) : 0,
                CompletionRate = lessons.Count > 0 ? (double)lessons.Count(l => l.Completed) / lessons.Count * 100 : 0,
                LessonsByDifficulty = lessons
                    .Where(l => l.Difficulty != null)
                    .GroupBy(l => l.Difficulty!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                LessonsByCourse = lessons
                    .Where(l => l.CourseCode != null)
                    .GroupBy(l => l.CourseCode!)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        public async Task<LessonViewModel?> GetNextLessonAsync(int currentLessonId)
        {
            var currentLesson = await GetLessonByIdAsync(currentLessonId);
            if (currentLesson == null)
                return null;

            var courseLessons = await GetLessonsByCourseAsync(currentLesson.CourseId);
            var nextLesson = courseLessons.FirstOrDefault(l => l.Order > currentLesson.Order);
            
            return nextLesson ?? courseLessons.FirstOrDefault(); // Return first lesson if no next
        }

        public async Task<LessonViewModel?> GetPreviousLessonAsync(int currentLessonId)
        {
            var currentLesson = await GetLessonByIdAsync(currentLessonId);
            if (currentLesson == null)
                return null;

            var courseLessons = await GetLessonsByCourseAsync(currentLesson.CourseId);
            var previousLesson = courseLessons.LastOrDefault(l => l.Order < currentLesson.Order);
            
            return previousLesson ?? courseLessons.LastOrDefault(); // Return last lesson if no previous
        }

        private async Task<LessonsData?> LoadLessonsDataAsync()
        {
            if (!File.Exists(_lessonsFilePath))
                return new LessonsData { Lessons = new List<LessonData>() };

            var json = await File.ReadAllTextAsync(_lessonsFilePath);
            return JsonConvert.DeserializeObject<LessonsData>(json);
        }

        private LessonViewModel MapToLessonViewModel(LessonData lesson)
        {
            return new LessonViewModel
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                CourseCode = lesson.CourseCode,
                Code = lesson.Code,
                Name = lesson.Name,
                Title = lesson.Title,
                Description = lesson.Description,
                Icon = lesson.Icon,
                ColorClass = lesson.ColorClass,
                Order = lesson.Order,
                Duration = lesson.Duration,
                VideoUrl = lesson.VideoUrl,
                Thumbnail = lesson.Thumbnail,
                ContentType = lesson.ContentType,
                IsFree = lesson.IsFree,
                Difficulty = lesson.Difficulty,
                Objectives = lesson.Objectives ?? new List<string>(),
                Resources = lesson.Resources?.Select(r => new ResourceViewModel
                {
                    Type = r.Type,
                    Name = r.Name,
                    Url = r.Url
                }).ToList() ?? new List<ResourceViewModel>(),
                Completed = lesson.Completed,
                Progress = lesson.Progress
            };
        }

        private Task<CourseViewModel> GetCourseForLessonAsync(int courseId)
        {
            // This is a simplified version - in a real app, you would inject ICoursesService
            // For now, we'll return a mock course
            return Task.FromResult(new CourseViewModel
            {
                Id = courseId,
                Name = $"Course {courseId}",
                Code = $"CRS{courseId:000}",
                Description = "Course description placeholder"
            });
        }

        private string CalculateTotalDuration(List<LessonViewModel> lessons)
        {
            var totalMinutes = 0;
            foreach (var lesson in lessons)
            {
                if (lesson.Duration != null && lesson.Duration.Contains("phút"))
                {
                    var parts = lesson.Duration.Split(' ');
                    if (int.TryParse(parts[0], out int minutes))
                    {
                        totalMinutes += minutes;
                    }
                }
            }
            
            var hours = totalMinutes / 60;
            var minutesRemainder = totalMinutes % 60;
            
            if (hours > 0)
                return $"{hours} giờ {minutesRemainder} phút";
            else
                return $"{minutesRemainder} phút";
        }

        #region Data Models for JSON Deserialization
        private class LessonsData
        {
            public List<LessonData>? Lessons { get; set; }
            public Metadata? Metadata { get; set; }
        }

        private class LessonData
        {
            public int Id { get; set; }
            public int CourseId { get; set; }
            public string? CourseCode { get; set; }
            public string? Code { get; set; }
            public string? Name { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Icon { get; set; }
            public string? ColorClass { get; set; }
            public int Order { get; set; }
            public string? Duration { get; set; }
            public string? VideoUrl { get; set; }
            public string? Thumbnail { get; set; }
            public string? ContentType { get; set; }
            public bool IsFree { get; set; }
            public string? Difficulty { get; set; }
            public List<string>? Objectives { get; set; }
            public List<ResourceData>? Resources { get; set; }
            public bool Completed { get; set; }
            public int Progress { get; set; }
        }

        private class ResourceData
        {
            public string? Type { get; set; }
            public string? Name { get; set; }
            public string? Url { get; set; }
        }

        private class Metadata
        {
            public string? Version { get; set; }
            public string? LastUpdated { get; set; }
            public int TotalLessons { get; set; }
            public int FreeLessons { get; set; }
            public int PaidLessons { get; set; }
            public string? TotalDuration { get; set; }
        }
        #endregion
    }
}
