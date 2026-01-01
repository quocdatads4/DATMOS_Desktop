using DATMOS.Web.Areas.Customer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public interface IExamListService
    {
        /// <summary>
        /// Lấy danh sách tất cả bài thi
        /// </summary>
        Task<List<ExamListViewModel>> GetAllExamsAsync();
        
        /// <summary>
        /// Lấy danh sách bài thi theo mã môn học
        /// </summary>
        Task<List<ExamListViewModel>> GetExamsBySubjectCodeAsync(string subjectCode);
        
        /// <summary>
        /// Lấy thông tin chi tiết của một bài thi theo ID
        /// </summary>
        Task<ExamListViewModel?> GetExamByIdAsync(int id);
        
        /// <summary>
        /// Lấy thông tin chi tiết của một bài thi theo mã bài thi
        /// </summary>
        Task<ExamListViewModel?> GetExamByCodeAsync(string examCode);
        
        /// <summary>
        /// Lấy thông tin đầy đủ của bài thi (bao gồm thông tin môn học, thống kê)
        /// </summary>
        Task<ExamListDetailsViewModel?> GetExamDetailsAsync(int id);
        
        /// <summary>
        /// Lấy danh sách bài thi theo loại (Practice Exam, Skill Review)
        /// </summary>
        Task<List<ExamListViewModel>> GetExamsByTypeAsync(string examType);
        
        /// <summary>
        /// Lấy danh sách bài thi đang hoạt động
        /// </summary>
        Task<List<ExamListViewModel>> GetActiveExamsAsync();
    }
}
