using System;
using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.Models
{
    public class TrainingData
    {
        public List<TrainingResult> TrainingResults { get; set; }
        public Metadata Metadata { get; set; }

        public TrainingData()
        {
            TrainingResults = new List<TrainingResult>();
            Metadata = new Metadata();
        }
    }

    public class Metadata
    {
        public string Version { get; set; }
        public string CreatedDate { get; set; }
        public string Description { get; set; }
        public int TotalResults { get; set; }
        public int TotalPassed { get; set; }
        public int TotalFailed { get; set; }
        public double AverageScore { get; set; }
        public string DataSource { get; set; }
        public DateTime LastUpdated { get; set; }

        public Metadata()
        {
            Version = string.Empty;
            CreatedDate = string.Empty;
            Description = string.Empty;
            DataSource = string.Empty;
        }
    }

    public class TrainingResult
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public bool Passed { get; set; }
        public int PassingScore { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string GradedBy { get; set; }
        public List<TaskResult> TaskResults { get; set; }
        public Summary Summary { get; set; }

        public TrainingResult()
        {
            Id = string.Empty;
            StudentId = string.Empty;
            StudentName = string.Empty;
            StudentCode = string.Empty;
            ExamId = string.Empty;
            ExamName = string.Empty;
            CourseCode = string.Empty;
            CourseName = string.Empty;
            ProjectName = string.Empty;
            Status = string.Empty;
            GradedBy = string.Empty;
            TaskResults = new List<TaskResult>();
            Summary = new Summary();
        }
    }

    public class TaskResult
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public int Points { get; set; }
        public int MaxPoints { get; set; }
        public string Feedback { get; set; }
        public int TimeSpentSeconds { get; set; }

        public TaskResult()
        {
            TaskName = string.Empty;
            Description = string.Empty;
            UserAnswer = string.Empty;
            CorrectAnswer = string.Empty;
            Feedback = string.Empty;
        }
    }

    public class Summary
    {
        public int CorrectTasks { get; set; }
        public int IncorrectTasks { get; set; }
        public int PartiallyCorrectTasks { get; set; }
        public int AverageTimePerTask { get; set; }
        public string DifficultyLevel { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> Weaknesses { get; set; }
        public string Recommendations { get; set; }

        public Summary()
        {
            DifficultyLevel = string.Empty;
            Strengths = new List<string>();
            Weaknesses = new List<string>();
            Recommendations = string.Empty;
        }
    }
}
