using System;
using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamListViewModel
    {
        public int Id { get; set; }
        public string ExamCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty; // "Practice Exam", "Skill Review"
        public string Mode { get; set; } = string.Empty; // "Testing", "Training", "Testing,Training"
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int PassingScore { get; set; }
        public bool IsActive { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties (optional, for display)
        public string? Icon { get; set; }
        public string? ColorClass { get; set; }
        public string? BadgeText { get; set; }
        
        // Exam projects
        public List<ExamProjectViewModel>? ExamProjects { get; set; }
        
        // Computed properties for display
        public string BadgeType
        {
            get
            {
                if (Mode.Contains("Testing") && Mode.Contains("Training"))
                    return "success"; // Đầy đủ chế độ
                else if (Mode.Contains("Training"))
                    return "info"; // Chỉ ôn luyện
                else if (Mode.Contains("Testing"))
                    return "warning"; // Chỉ thi thử
                else
                    return "secondary";
            }
        }
        
        public string BadgeLabel
        {
            get
            {
                if (Mode.Contains("Testing") && Mode.Contains("Training"))
                    return "Đầy đủ chế độ";
                else if (Mode.Contains("Training"))
                    return "Chỉ ôn luyện";
                else if (Mode.Contains("Testing"))
                    return "Chỉ thi thử";
                else
                    return "Không xác định";
            }
        }
    }
    
    public class ExamListDetailsViewModel
    {
        public ExamListViewModel Exam { get; set; } = new ExamListViewModel();
        public ExamSubjectViewModel? Subject { get; set; }
        public List<ExamAttemptViewModel>? RecentAttempts { get; set; }
        public ExamStatistics? Statistics { get; set; }
    }
    
    public class ExamAttemptViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime AttemptDate { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public bool IsPassed { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
    
    public class ExamStatistics
    {
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public double PassRate { get; set; }
        public int BestScore { get; set; }
        public TimeSpan AverageTime { get; set; }
    }
    
}
