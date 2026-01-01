using DATMOS.Web.Services;
using DATMOS.Web.Areas.Customer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ExamListController : Controller
    {
        private readonly IExamListService _examListService;
        private readonly IExamSubjectService _examSubjectService;

        public ExamListController(IExamListService examListService, IExamSubjectService examSubjectService)
        {
            _examListService = examListService;
            _examSubjectService = examSubjectService;
        }

        // GET: /Customer/ExamList
        public async Task<IActionResult> Index(string? subjectCode)
        {
            if (!string.IsNullOrEmpty(subjectCode))
            {
                // Redirect to BySubject action for cleaner URL
                return RedirectToAction(nameof(BySubject), new { subjectCode });
            }
            
            var exams = await _examListService.GetAllExamsAsync();
            return View(exams);
        }

        // GET: /Customer/ExamList/BySubject/{subjectCode}
        public async Task<IActionResult> BySubject(string subjectCode)
        {
            var exams = await _examListService.GetExamsBySubjectCodeAsync(subjectCode);
            var subject = await _examSubjectService.GetSubjectByCodeAsync(subjectCode);
            
            // Create a simple view model for subject summary
            var subjectSummary = new SubjectExamSummaryViewModel
            {
                SubjectCode = subjectCode,
                SubjectName = subject?.Name ?? subjectCode,
                Subject = subject,
                Exams = exams,
                TotalExams = exams?.Count ?? 0,
                TotalTrainingExams = exams?.Count(e => e.Mode.Contains("Training")) ?? 0,
                TotalTestingExams = exams?.Count(e => e.Mode.Contains("Testing")) ?? 0,
                TotalProjects = exams?.Sum(e => e.TotalProjects) ?? 0,
                TotalTasks = exams?.Sum(e => e.TotalTasks) ?? 0
            };
            
            return View("SubjectSummary", subjectSummary);
        }

        // GET: /Customer/ExamList/Details/{id}
        // GET: /Customer/ExamList/Details?subjectCode={subjectCode}
        public async Task<IActionResult> Details(int? id, string? subjectCode)
        {
            // If id is provided, show details of a specific exam
            if (id.HasValue)
            {
                var examDetails = await _examListService.GetExamDetailsAsync(id.Value);
                if (examDetails == null)
                {
                    return NotFound();
                }

                return View(examDetails);
            }
            // If subjectCode is provided, show summary of all exams for that subject
            else if (!string.IsNullOrEmpty(subjectCode))
            {
                return await BySubject(subjectCode);
            }
            
            // If neither id nor subjectCode is provided, redirect to Index
            return RedirectToAction(nameof(Index));
        }

      
        // GET: /Customer/ExamList/Active
        public async Task<IActionResult> Active()
        {
            var exams = await _examListService.GetActiveExamsAsync();
            return View(exams);
        }

        
    }
}
