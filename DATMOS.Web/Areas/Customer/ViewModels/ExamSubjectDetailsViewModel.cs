using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    /// <summary>
    /// ViewModel cho trang chi tiết môn học
    /// </summary>
    public class ExamSubjectDetailsViewModel
    {
        /// <summary>
        /// Thông tin cơ bản của môn học
        /// </summary>
        public ExamSubjectViewModel? Subject { get; set; }

        /// <summary>
        /// Thống kê về môn học
        /// </summary>
        public ExamSubjectStatistics? Statistics { get; set; }

        /// <summary>
        /// Danh sách khóa học liên quan
        /// </summary>
        public List<CourseViewModel>? RelatedCourses { get; set; }
    }

    /// <summary>
    /// Thống kê về môn học
    /// </summary>
    public class ExamSubjectStatistics
    {
        /// <summary>
        /// Tổng số học viên
        /// </summary>
        public int TotalStudents { get; set; }

        /// <summary>
        /// Điểm trung bình (0-100)
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// Tỷ lệ hoàn thành (%)
        /// </summary>
        public double CompletionRate { get; set; }

        /// <summary>
        /// Tổng số bài thi đã hoàn thành
        /// </summary>
        public int CompletedExams { get; set; }

        /// <summary>
        /// Tổng số bài học đã hoàn thành
        /// </summary>
        public int CompletedLessons { get; set; }

        /// <summary>
        /// Thời gian học trung bình (phút)
        /// </summary>
        public double AverageStudyTime { get; set; }

        /// <summary>
        /// Tỷ lệ đậu (%)
        /// </summary>
        public double PassRate { get; set; }

        /// <summary>
        /// Điểm cao nhất
        /// </summary>
        public double HighestScore { get; set; }

        /// <summary>
        /// Điểm thấp nhất
        /// </summary>
        public double LowestScore { get; set; }

        /// <summary>
        /// Số lượt đánh giá
        /// </summary>
        public int TotalReviews { get; set; }

        /// <summary>
        /// Điểm đánh giá trung bình (1-5)
        /// </summary>
        public double AverageRating { get; set; }
    }
}
