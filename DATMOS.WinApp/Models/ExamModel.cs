using System;
using System.Collections.Generic;

namespace DATMOS.WinApp.Models
{
    public class ExamModel
    {
        public string Title { get; set; } = "DATMOS Practice Exam";
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
        public TimeSpan TotalTime { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan RemainingTime { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public int CurrentQuestionIndex { get; set; } = 0;
        
        public int TotalQuestions => Questions.Count;
        public int CompletedQuestions => Questions.Count(q => q.Status == QuestionModel.QuestionStatus.Completed);
        public double ProgressPercentage => TotalQuestions > 0 ? (CompletedQuestions * 100.0) / TotalQuestions : 0;
        
        public QuestionModel? CurrentQuestion => 
            CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Count 
                ? Questions[CurrentQuestionIndex] 
                : null;
                
        public void MoveToNextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
            }
        }
        
        public void MoveToPreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
            }
        }
        
        public void MarkQuestionCompleted(int questionId)
        {
            var question = Questions.Find(q => q.Id == questionId);
            if (question != null)
            {
                question.Status = QuestionModel.QuestionStatus.Completed;
                question.CompletedAt = DateTime.Now;
            }
        }
        
        public void MarkQuestionForReview(int questionId, bool markForReview)
        {
            var question = Questions.Find(q => q.Id == questionId);
            if (question != null)
            {
                question.IsMarkedForReview = markForReview;
            }
        }
    }
}
