using DATMOS.Web.Areas.Customer.ViewModels;
using DATMOS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public class ExamListService : IExamListService
    {
        private readonly AppDbContext _context;
        private readonly IExamSubjectService _examSubjectService;

        public ExamListService(AppDbContext context, IExamSubjectService examSubjectService)
        {
            _context = context;
            _examSubjectService = examSubjectService;
        }

        public async Task<List<ExamListViewModel>> GetAllExamsAsync()
        {
            var exams = await _context.ExamLists
                .AsNoTracking()
                .Include(e => e.Subject)
                .Include(e => e.ExamProjects.Where(p => p.IsActive))
                .Where(e => e.IsActive)
                .OrderBy(e => e.Id)
                .Select(e => new ExamListViewModel
                {
                    Id = e.Id,
                    ExamCode = e.Code,
                    Title = e.Name,
                    Description = e.Description,
                    SubjectCode = e.Subject.Code,
                    SubjectName = e.Subject.Name,
                    ExamType = e.Type,
                    Mode = e.Mode,
                    DurationMinutes = e.TimeLimit,
                    TotalQuestions = e.TotalTasks,
                    TotalProjects = e.TotalProjects,
                    TotalTasks = e.TotalTasks,
                    PassingScore = e.PassingScore,
                    IsActive = e.IsActive,
                    OrderIndex = e.Id,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Icon = ExamListService.GetIconForSubject(e.Subject.Code),
                    ColorClass = ExamListService.GetColorClassForSubject(e.Subject.Code),
                    BadgeText = ExamListService.GetBadgeTextForExamType(e.Type),
                    ExamProjects = e.ExamProjects.Select(p => new ExamProjectViewModel
                    {
                        Id = p.Id,
                        ExamListId = p.ExamListId,
                        Name = p.Name,
                        Description = p.Description,
                        TotalTasks = p.TotalTasks,
                        OrderIndex = p.OrderIndex,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return exams ?? new List<ExamListViewModel>();
        }

        public async Task<List<ExamListViewModel>> GetExamsBySubjectCodeAsync(string subjectCode)
        {
            var allExams = await GetAllExamsAsync();
            return allExams
                .Where(e => e.SubjectCode.Equals(subjectCode, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.OrderIndex)
                .ToList();
        }

        public async Task<ExamListViewModel?> GetExamByIdAsync(int id)
        {
            var exam = await _context.ExamLists
                .AsNoTracking()
                .Include(e => e.Subject)
                .Include(e => e.ExamProjects.Where(p => p.IsActive))
                .Where(e => e.Id == id)
                .Select(e => new ExamListViewModel
                {
                    Id = e.Id,
                    ExamCode = e.Code,
                    Title = e.Name,
                    Description = e.Description,
                    SubjectCode = e.Subject.Code,
                    SubjectName = e.Subject.Name,
                    ExamType = e.Type,
                    Mode = e.Mode,
                    DurationMinutes = e.TimeLimit,
                    TotalQuestions = e.TotalTasks,
                    TotalProjects = e.TotalProjects,
                    TotalTasks = e.TotalTasks,
                    PassingScore = e.PassingScore,
                    IsActive = e.IsActive,
                    OrderIndex = e.Id,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Icon = ExamListService.GetIconForSubject(e.Subject.Code),
                    ColorClass = ExamListService.GetColorClassForSubject(e.Subject.Code),
                    BadgeText = ExamListService.GetBadgeTextForExamType(e.Type),
                    ExamProjects = e.ExamProjects.Select(p => new ExamProjectViewModel
                    {
                        Id = p.Id,
                        ExamListId = p.ExamListId,
                        Name = p.Name,
                        Description = p.Description,
                        TotalTasks = p.TotalTasks,
                        OrderIndex = p.OrderIndex,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return exam;
        }

        public async Task<ExamListViewModel?> GetExamByCodeAsync(string examCode)
        {
            var exam = await _context.ExamLists
                .AsNoTracking()
                .Include(e => e.Subject)
                .Include(e => e.ExamProjects.Where(p => p.IsActive))
                .Where(e => e.Code == examCode)
                .Select(e => new ExamListViewModel
                {
                    Id = e.Id,
                    ExamCode = e.Code,
                    Title = e.Name,
                    Description = e.Description,
                    SubjectCode = e.Subject.Code,
                    SubjectName = e.Subject.Name,
                    ExamType = e.Type,
                    Mode = e.Mode,
                    DurationMinutes = e.TimeLimit,
                    TotalQuestions = e.TotalTasks,
                    TotalProjects = e.TotalProjects,
                    TotalTasks = e.TotalTasks,
                    PassingScore = e.PassingScore,
                    IsActive = e.IsActive,
                    OrderIndex = e.Id,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Icon = ExamListService.GetIconForSubject(e.Subject.Code),
                    ColorClass = ExamListService.GetColorClassForSubject(e.Subject.Code),
                    BadgeText = ExamListService.GetBadgeTextForExamType(e.Type),
                    ExamProjects = e.ExamProjects.Select(p => new ExamProjectViewModel
                    {
                        Id = p.Id,
                        ExamListId = p.ExamListId,
                        Name = p.Name,
                        Description = p.Description,
                        TotalTasks = p.TotalTasks,
                        OrderIndex = p.OrderIndex,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return exam;
        }

        public async Task<ExamListDetailsViewModel?> GetExamDetailsAsync(int id)
        {
            var exam = await GetExamByIdAsync(id);
            if (exam == null)
                return null;

            // Get subject information
            var subjects = await _examSubjectService.GetAllSubjectsAsync();
            var subject = subjects.FirstOrDefault(s => s.Code.Equals(exam.SubjectCode, StringComparison.OrdinalIgnoreCase));

            return new ExamListDetailsViewModel
            {
                Exam = exam,
                Subject = subject,
                RecentAttempts = await GetRecentAttemptsAsync(id),
                Statistics = await GetExamStatisticsAsync(id)
            };
        }

        public async Task<List<ExamListViewModel>> GetExamsByTypeAsync(string examType)
        {
            var allExams = await GetAllExamsAsync();
            return allExams
                .Where(e => e.ExamType.Equals(examType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.OrderIndex)
                .ToList();
        }

        public async Task<List<ExamListViewModel>> GetActiveExamsAsync()
        {
            var allExams = await GetAllExamsAsync();
            return allExams
                .Where(e => e.IsActive)
                .OrderBy(e => e.OrderIndex)
                .ToList();
        }

        // Các method JSON fallback đã được xóa vì chỉ sử dụng database

        private async Task<List<ExamAttemptViewModel>> GetRecentAttemptsAsync(int examId)
        {
            // Mock data - trong thực tế có thể lấy từ database
            await Task.Delay(10);
            
            return new List<ExamAttemptViewModel>
            {
                new ExamAttemptViewModel
                {
                    Id = 1,
                    UserId = "user1",
                    UserName = "Nguyễn Văn A",
                    AttemptDate = DateTime.Now.AddDays(-2),
                    Score = 85,
                    MaxScore = 100,
                    IsPassed = true,
                    TimeTaken = TimeSpan.FromMinutes(45)
                },
                new ExamAttemptViewModel
                {
                    Id = 2,
                    UserId = "user2",
                    UserName = "Trần Thị B",
                    AttemptDate = DateTime.Now.AddDays(-5),
                    Score = 72,
                    MaxScore = 100,
                    IsPassed = true,
                    TimeTaken = TimeSpan.FromMinutes(50)
                }
            };
        }

        private async Task<ExamStatistics> GetExamStatisticsAsync(int examId)
        {
            // Mock data - trong thực tế có thể lấy từ database
            await Task.Delay(10);
            
            return new ExamStatistics
            {
                TotalAttempts = 150,
                AverageScore = 78.5,
                PassRate = 82.3,
                BestScore = 98,
                AverageTime = TimeSpan.FromMinutes(48)
            };
        }

        private static string GetIconForSubject(string subjectCode)
        {
            if (string.IsNullOrEmpty(subjectCode))
                return "bx bx-question-mark";
                
            return subjectCode.ToUpper() switch
            {
                "WORD" => "bx bx-file",
                "EXCEL" => "bx bx-table",
                "PPT" => "bx bx-slideshow",
                _ => "bx bx-question-mark"
            };
        }

        private static string GetColorClassForSubject(string subjectCode)
        {
            if (string.IsNullOrEmpty(subjectCode))
                return "bg-label-secondary";
                
            return subjectCode.ToUpper() switch
            {
                "WORD" => "bg-label-primary",
                "EXCEL" => "bg-label-success",
                "PPT" => "bg-label-warning",
                _ => "bg-label-secondary"
            };
        }

        private static string GetBadgeTextForExamType(string examType)
        {
            if (string.IsNullOrEmpty(examType))
                return string.Empty;
                
            return examType switch
            {
                "Practice Exam" => "PE",
                "Skill Review" => "SR",
                _ => examType
            };
        }

        // Các class JSON đã được xóa vì chỉ sử dụng database
    }
}
