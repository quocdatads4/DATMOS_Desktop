using DATMOS.WinApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DATMOS.WinApp.Controllers
{
    public class GMetrixController : IDisposable
    {
        private ExamModel? _exam;
        private TimerModel? _timer;
        private List<ProjectModel>? _projects;
        private int _currentProjectIndex;
        
        public event EventHandler<TimeSpan>? TimeUpdated;
        public event EventHandler<QuestionModel>? QuestionChanged;
        public event EventHandler<double>? ProgressUpdated;
        public event EventHandler<string>? Notification;
        
        public GMetrixController()
        {
            InitializeModels();
        }
        
        private void InitializeModels()
        {
            // Initialize exam with sample questions
            _exam = new ExamModel
            {
                Title = "DATMOS GMetrix Practice Exam",
                TotalTime = TimeSpan.FromMinutes(30),
                RemainingTime = TimeSpan.FromMinutes(20) + TimeSpan.FromSeconds(49)
            };
            
            // Add sample questions
            var questions = new List<QuestionModel>
            {
                new QuestionModel { Id = 0, Title = "Tổng quan", Instruction = "Xem tổng quan tất cả các câu hỏi trong bài thi." },
                new QuestionModel { Id = 1, Title = "Câu 1", Instruction = "Dưới tiêu đề <strong>Landscaping Made Easy</strong>, chèn một ảnh chụp màn hình của bức ảnh hiển thị trên tài liệu <strong>Project</strong>." },
                new QuestionModel { Id = 2, Title = "Câu 2", Instruction = "Định dạng tiêu đề \"Landscaping Made Easy\" với kiểu chữ <strong>Title</strong>." },
                new QuestionModel { Id = 3, Title = "Câu 3", Instruction = "Chèn một bảng với 3 cột và 4 hàng bên dưới đoạn văn đầu tiên." },
                new QuestionModel { Id = 4, Title = "Câu 4", Instruction = "Áp dụng kiểu <strong>Grid Table 4 - Accent 1</strong> cho bảng vừa tạo." },
                new QuestionModel { Id = 5, Title = "Câu 5", Instruction = "Căn giữa bảng và thêm chú thích \"Landscaping Costs\" phía trên bảng." }
            };
            
            _exam.Questions = questions;
            _exam.CurrentQuestionIndex = 1; // Start with question 1
            
            // Initialize timer
            _timer = new TimerModel(_exam.RemainingTime);
            _timer.TimeUpdated += OnTimerTimeUpdated;
            _timer.TimeExpired += OnTimerExpired;
            
            // Initialize projects
            _projects = new List<ProjectModel>
            {
                new ProjectModel { Id = 1, Name = "Project 1", TotalTasks = 5, CompletedTasks = 1 },
                new ProjectModel { Id = 2, Name = "Project 2", TotalTasks = 3, CompletedTasks = 0 },
                new ProjectModel { Id = 3, Name = "Project 3", TotalTasks = 7, CompletedTasks = 0 }
            };
            
            _currentProjectIndex = 0;
        }
        
        public void Initialize()
        {
            // Start the timer
            _timer?.Start();
            
            // Notify initial state
            if (_exam != null)
            {
                OnQuestionChanged(_exam.CurrentQuestion);
                OnProgressUpdated(_exam.ProgressPercentage);
            }
        }
        
        public void ChangeQuestion(int questionIndex)
        {
            if (_exam != null && questionIndex >= 0 && questionIndex < _exam.Questions.Count)
            {
                _exam.CurrentQuestionIndex = questionIndex;
                OnQuestionChanged(_exam.CurrentQuestion);
            }
        }
        
        public void MoveToNextQuestion()
        {
            _exam?.MoveToNextQuestion();
            if (_exam != null)
            {
                OnQuestionChanged(_exam.CurrentQuestion);
                Notification?.Invoke(this, "Đã chuyển đến câu tiếp theo");
            }
        }
        
        public void MoveToPreviousQuestion()
        {
            _exam?.MoveToPreviousQuestion();
            if (_exam != null)
            {
                OnQuestionChanged(_exam.CurrentQuestion);
                Notification?.Invoke(this, "Đã chuyển đến câu trước");
            }
        }
        
        public void MarkCurrentQuestionCompleted()
        {
            if (_exam?.CurrentQuestion != null)
            {
                _exam.MarkQuestionCompleted(_exam.CurrentQuestion.Id);
                OnProgressUpdated(_exam.ProgressPercentage);
                Notification?.Invoke(this, $"Đã đánh dấu câu {_exam.CurrentQuestionIndex} hoàn thành");
            }
        }
        
        public void ToggleMarkForReview()
        {
            if (_exam?.CurrentQuestion != null)
            {
                bool newState = !_exam.CurrentQuestion.IsMarkedForReview;
                _exam.MarkQuestionForReview(_exam.CurrentQuestion.Id, newState);
                
                string message = newState 
                    ? $"Đã đánh dấu câu {_exam.CurrentQuestionIndex} cần xem lại" 
                    : $"Đã bỏ đánh dấu câu {_exam.CurrentQuestionIndex} cần xem lại";
                
                Notification?.Invoke(this, message);
            }
        }
        
        public void ChangeProject(int projectIndex)
        {
            if (_projects != null && projectIndex >= 0 && projectIndex < _projects.Count)
            {
                _currentProjectIndex = projectIndex;
                var project = _projects[projectIndex];
                Notification?.Invoke(this, $"Đã chuyển đến {project.Name}");
            }
        }
        
        public void ShowHelp()
        {
            Notification?.Invoke(this, "Đang tải trợ giúp...");
            // In a real implementation, this would open a help dialog or documentation
        }
        
        public void SaveProgress()
        {
            Notification?.Invoke(this, "Đang lưu tiến độ...");
            // In a real implementation, this would save to file or database
        }
        
        public void RestartExam()
        {
            if (_exam != null && _timer != null)
            {
                // Reset exam state
                foreach (var question in _exam.Questions)
                {
                    question.Status = QuestionModel.QuestionStatus.NotStarted;
                    question.IsMarkedForReview = false;
                    question.CompletedAt = null;
                }
                
                _exam.CurrentQuestionIndex = 1;
                _timer.Reset();
                _timer.Start();
                
                OnQuestionChanged(_exam.CurrentQuestion);
                OnProgressUpdated(_exam.ProgressPercentage);
                Notification?.Invoke(this, "Đã khởi động lại bài thi");
            }
        }
        
        public void ShowSummary()
        {
            Notification?.Invoke(this, "Đang tải tổng quan...");
            // In a real implementation, this would show a summary dialog
        }
        
        public void GradeExam()
        {
            Notification?.Invoke(this, "Đang chấm điểm...");
            // In a real implementation, this would grade the exam
        }
        
        private void OnTimerTimeUpdated(object? sender, TimeSpan time)
        {
            if (_exam != null && _timer != null)
            {
                _exam.RemainingTime = time;
                TimeUpdated?.Invoke(this, time);
                
                // Update notification for warning/critical time
                if (_timer.IsWarningTime && !_timer.IsCriticalTime)
                {
                    Notification?.Invoke(this, $"Còn {time:mm\\:ss} phút!");
                }
                else if (_timer.IsCriticalTime)
                {
                    Notification?.Invoke(this, $"THỜI GIAN SẮP HẾT! Còn {time:mm\\:ss} phút!");
                }
            }
        }
        
        private void OnTimerExpired(object? sender, EventArgs e)
        {
            Notification?.Invoke(this, "THỜI GIAN ĐÃ HẾT!");
            TimeUpdated?.Invoke(this, TimeSpan.Zero);
        }
        
        private void OnQuestionChanged(QuestionModel? question)
        {
            if (question != null)
            {
                QuestionChanged?.Invoke(this, question);
            }
        }
        
        private void OnProgressUpdated(double progress)
        {
            ProgressUpdated?.Invoke(this, progress);
        }
        
        public ExamModel? GetExam() => _exam;
        public TimerModel? GetTimer() => _timer;
        public List<ProjectModel>? GetProjects() => _projects;
        public ProjectModel? GetCurrentProject() => _projects?[_currentProjectIndex];
        
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
