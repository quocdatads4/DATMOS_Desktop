using DATMOS.Web.Areas.Customer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services
{
    public interface IExamSubjectService
    {
        /// <summary>
        /// Lấy danh sách tất cả môn học
        /// </summary>
        Task<List<ExamSubjectViewModel>> GetAllSubjectsAsync();

        /// <summary>
        /// Lấy thông tin chi tiết của một môn học theo ID
        /// </summary>
        Task<ExamSubjectViewModel?> GetSubjectByIdAsync(int id);

        /// <summary>
        /// Lấy thông tin đầy đủ của môn học (bao gồm các khóa học liên quan)
        /// </summary>
        Task<ExamSubjectDetailsViewModel?> GetSubjectDetailsAsync(int id);

        /// <summary>
        /// Lấy thông tin môn học theo mã môn học
        /// </summary>
        Task<ExamSubjectViewModel?> GetSubjectByCodeAsync(string subjectCode);
    }
}
