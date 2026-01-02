using DATMOS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
[Authorize(Roles = "User")]
public class ExamSubjectController : Controller
    {
        private readonly IExamSubjectService _subjectService;

        public ExamSubjectController(IExamSubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Hiển thị danh sách tất cả môn học
        /// Route: /Customer/ExamSubject
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return View(subjects);
        }

        /// <summary>
        /// Hiển thị chi tiết một môn học
        /// Route: /Customer/ExamSubject/Details/{id}
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var subjectDetails = await _subjectService.GetSubjectDetailsAsync(id);
            if (subjectDetails == null)
            {
                return NotFound();
            }

            return View(subjectDetails);
        }

        /// <summary>
        /// Hiển thị các khóa học của một môn học
        /// Route: /Customer/ExamSubject/Courses/{subjectId}
        /// </summary>
        public async Task<IActionResult> Courses(int subjectId)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
            if (subject == null)
            {
                return NotFound();
            }

            var subjectDetails = await _subjectService.GetSubjectDetailsAsync(subjectId);
            if (subjectDetails == null)
            {
                return NotFound();
            }

            ViewBag.Subject = subject;
            return View(subjectDetails.RelatedCourses);
        }

        /// <summary>
        /// API endpoint để lấy danh sách môn học dưới dạng JSON
        /// Route: /Customer/ExamSubject/GetAll
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return Json(new { success = true, data = subjects });
        }

        /// <summary>
        /// API endpoint để lấy thông tin môn học theo ID dưới dạng JSON
        /// Route: /Customer/ExamSubject/GetById/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return Json(new { success = false, message = "Môn học không tồn tại" });
            }

            return Json(new { success = true, data = subject });
        }
    }
}
