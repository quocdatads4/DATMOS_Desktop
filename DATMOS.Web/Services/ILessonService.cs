using DATMOS.Web.Areas.Customer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public interface ILessonService
    {
        /// <summary>
        /// Lấy danh sách tất cả bài học
        /// </summary>
        Task<List<LessonViewModel>> GetAllLessonsAsync();

        /// <summary>
        /// Lấy bài học theo ID
        /// </summary>
        Task<LessonViewModel?> GetLessonByIdAsync(int id);

        /// <summary>
        /// Lấy chi tiết bài học (bao gồm objectives, resources, etc.)
        /// </summary>
        Task<LessonDetailsViewModel?> GetLessonDetailsAsync(int id);

        /// <summary>
        /// Lấy danh sách bài học theo courseId
        /// </summary>
        Task<List<LessonViewModel>> GetLessonsByCourseAsync(int courseId);

        /// <summary>
        /// Lấy danh sách bài học miễn phí
        /// </summary>
        Task<List<LessonViewModel>> GetFreeLessonsAsync();

        /// <summary>
        /// Lấy danh sách bài học trả phí
        /// </summary>
        Task<List<LessonViewModel>> GetPaidLessonsAsync();

        /// <summary>
        /// Lọc bài học theo độ khó
        /// </summary>
        Task<List<LessonViewModel>> GetLessonsByDifficultyAsync(string difficulty);

        /// <summary>
        /// Tìm kiếm bài học theo từ khóa
        /// </summary>
        Task<List<LessonViewModel>> SearchLessonsAsync(string keyword);

        /// <summary>
        /// Lấy thống kê về bài học
        /// </summary>
        Task<LessonStatistics> GetLessonStatisticsAsync();

        /// <summary>
        /// Lấy bài học tiếp theo trong cùng khóa học
        /// </summary>
        Task<LessonViewModel?> GetNextLessonAsync(int currentLessonId);

        /// <summary>
        /// Lấy bài học trước đó trong cùng khóa học
        /// </summary>
        Task<LessonViewModel?> GetPreviousLessonAsync(int currentLessonId);
    }
}
