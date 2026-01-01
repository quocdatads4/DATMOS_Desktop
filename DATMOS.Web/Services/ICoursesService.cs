using DATMOS.Web.Areas.Customer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public interface ICoursesService
    {
        /// <summary>
        /// Lấy danh sách tất cả khóa học
        /// </summary>
        Task<List<CourseViewModel>> GetAllCoursesAsync();

        /// <summary>
        /// Lấy khóa học theo ID
        /// </summary>
        Task<CourseViewModel?> GetCourseByIdAsync(int id);

        /// <summary>
        /// Lấy chi tiết khóa học (bao gồm syllabus, instructor info, etc.)
        /// </summary>
        Task<CourseDetailsViewModel?> GetCourseDetailsAsync(int id);

        /// <summary>
        /// Lấy danh sách khóa học theo subjectId
        /// </summary>
        Task<List<CourseViewModel>> GetCoursesBySubjectAsync(int subjectId);

        /// <summary>
        /// Lấy danh sách khóa học miễn phí
        /// </summary>
        Task<List<CourseViewModel>> GetFreeCoursesAsync();

        /// <summary>
        /// Lấy danh sách khóa học trả phí
        /// </summary>
        Task<List<CourseViewModel>> GetPaidCoursesAsync();

        /// <summary>
        /// Lọc khóa học theo level
        /// </summary>
        Task<List<CourseViewModel>> GetCoursesByLevelAsync(string level);

        /// <summary>
        /// Tìm kiếm khóa học theo từ khóa
        /// </summary>
        Task<List<CourseViewModel>> SearchCoursesAsync(string keyword);

        /// <summary>
        /// Lấy thống kê về khóa học
        /// </summary>
        Task<CourseStatistics> GetCourseStatisticsAsync();
    }
}
