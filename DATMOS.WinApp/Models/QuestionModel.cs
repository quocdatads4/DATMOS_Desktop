using System;

namespace DATMOS.WinApp.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
        public QuestionStatus Status { get; set; } = QuestionStatus.NotStarted;
        public bool IsMarkedForReview { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        public enum QuestionStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Skipped
        }
    }
}
