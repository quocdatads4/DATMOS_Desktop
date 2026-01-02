using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class SubjectExamSummaryViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public ExamSubjectViewModel? Subject { get; set; }
        public List<ExamListViewModel>? Exams { get; set; }
        public int TotalExams { get; set; }
        public int TotalTrainingExams { get; set; }
        public int TotalTestingExams { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        
        // Additional statistics
        public double AverageTasksPerExam => TotalExams > 0 ? (double)TotalTasks / TotalExams : 0;
        public double AverageProjectsPerExam => TotalExams > 0 ? (double)TotalProjects / TotalExams : 0;
        public double TrainingPercentage => TotalExams > 0 ? (TotalTrainingExams * 100.0 / TotalExams) : 0;
        public double TestingPercentage => TotalExams > 0 ? (TotalTestingExams * 100.0 / TotalExams) : 0;
        
        // Progress tracking
        public int CompletedExams { get; set; }
        public int InProgressExams { get; set; }
        public int NotStartedExams { get; set; }
        
        public double CompletionPercentage => TotalExams > 0 ? (CompletedExams * 100.0 / TotalExams) : 0;
        public double InProgressPercentage => TotalExams > 0 ? (InProgressExams * 100.0 / TotalExams) : 0;
        public double NotStartedPercentage => TotalExams > 0 ? (NotStartedExams * 100.0 / TotalExams) : 0;
        
        // Performance metrics
        public double AverageScore { get; set; }
        public double BestScore { get; set; }
        public double WorstScore { get; set; }
        public double PassRate { get; set; }
        
        // Time tracking
        public double AverageTimePerExam { get; set; }
        public double TotalTimeSpent { get; set; }
        
        // Recommendations
        public string[] RecommendedNextExams { get; set; } = new string[0];
        public string[] AreasForImprovement { get; set; } = new string[0];
        
        // User-specific data
        public bool IsEnrolled { get; set; }
        public bool HasAccess { get; set; }
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
    }
}
