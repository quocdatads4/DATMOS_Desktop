using System;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class UserProgressViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int NotStartedTasks { get; set; }
        public double AverageScore { get; set; }
        public DateTime LastActivity { get; set; }
        public double CompletionPercentage => TotalTasks > 0 ? (CompletedTasks * 100.0 / TotalTasks) : 0;
        
        // Progress by task type
        public int FormatTasksCompleted { get; set; }
        public int InsertTasksCompleted { get; set; }
        public int TableTasksCompleted { get; set; }
        public int ChartTasksCompleted { get; set; }
        public int FormulaTasksCompleted { get; set; }
        public int FileTasksCompleted { get; set; }
        
        // Time tracking
        public TimeSpan TotalTimeSpent { get; set; }
        public TimeSpan AverageTimePerTask => CompletedTasks > 0 ? 
            TimeSpan.FromSeconds(TotalTimeSpent.TotalSeconds / CompletedTasks) : TimeSpan.Zero;
        
        // Streak and achievements
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int AchievementsUnlocked { get; set; }
        public int TotalPoints { get; set; }
        
        // Performance metrics
        public double AccuracyRate { get; set; }
        public double SpeedScore { get; set; }
        public double EfficiencyScore { get; set; }
        
        // Recommendations
        public string[] RecommendedTopics { get; set; }
        public string[] AreasForImprovement { get; set; }
        
        public UserProgressViewModel()
        {
            UserName = string.Empty;
            SubjectName = string.Empty;
            RecommendedTopics = Array.Empty<string>();
            AreasForImprovement = Array.Empty<string>();
        }
    }
}
