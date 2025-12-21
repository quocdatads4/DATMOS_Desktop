using DATMOS.Core.Entities;
using DATMOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        public ProductController()
        {
        }

        // GET: Customer/Product
        public async Task<IActionResult> Index()
        {
            // Mock data for products (courses)
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "MOS Word 2019", Price = 0, CreatedAt = System.DateTime.UtcNow },
                new Product { Id = 2, Name = "MOS Excel 2019", Price = 0, CreatedAt = System.DateTime.UtcNow },
                new Product { Id = 3, Name = "MOS PowerPoint 2019", Price = 0, CreatedAt = System.DateTime.UtcNow },
                new Product { Id = 4, Name = "MOS Access 2019", Price = 0, CreatedAt = System.DateTime.UtcNow },
                new Product { Id = 5, Name = "MOS Outlook 2019", Price = 0, CreatedAt = System.DateTime.UtcNow }
            };
            
            ViewBag.RemainingCredits = 15; // Mock: user has 15 credits left
            return View(products);
        }

        // GET: Customer/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Mock data for product details
            var product = new Product { Id = id.Value, Name = $"MOS Product {id}", Price = 0, CreatedAt = System.DateTime.UtcNow };
            
            // Mock data for exams
            var exams = new List<ExamViewModel>
            {
                new ExamViewModel { Id = 1, ProductId = id.Value, Name = "Exam 1", Description = "Bài thi số 1", HasPractice = true, HasTest = true, Order = 1 },
                new ExamViewModel { Id = 2, ProductId = id.Value, Name = "Exam 2", Description = "Bài thi số 2", HasPractice = true, HasTest = true, Order = 2 },
                new ExamViewModel { Id = 3, ProductId = id.Value, Name = "Exam 3", Description = "Bài thi số 3", HasPractice = true, HasTest = false, Order = 3 },
                new ExamViewModel { Id = 4, ProductId = id.Value, Name = "Exam 4", Description = "Bài thi số 4", HasPractice = false, HasTest = true, Order = 4 }
            };
            
            ViewBag.Exams = exams;
            ViewBag.RemainingCredits = 15;
            ViewBag.Product = product;
            
            return View(product);
        }

        // GET: Customer/Product/Practice/{productId}/{examId}
        public IActionResult Practice(int productId, int examId)
        {
            // Mock data for practice session
            var practiceData = new PracticeViewModel
            {
                ProductId = productId,
                ProductName = $"MOS Product {productId}",
                ExamId = examId,
                ExamName = $"Exam {examId}",
                ExamType = "Practice",
                RemainingTime = 3600, // 60 minutes in seconds
                RemainingCredits = 15,
                Projects = GetMockProjects(examId),
                CurrentProjectId = 1
            };
            
            return View(practiceData);
        }

        // GET: Customer/Product/Test/{productId}/{examId}
        public IActionResult Test(int productId, int examId)
        {
            // Mock data for test session
            var testData = new PracticeViewModel
            {
                ProductId = productId,
                ProductName = $"MOS Product {productId}",
                ExamId = examId,
                ExamName = $"Exam {examId}",
                ExamType = "Test",
                RemainingTime = 2700, // 45 minutes in seconds
                RemainingCredits = 15,
                Projects = GetMockProjects(examId),
                CurrentProjectId = 1
            };
            
            return View(testData);
        }

        // GET: Customer/Product/Project/{projectId}
        public IActionResult Project(int projectId)
        {
            // Mock data for project details
            var projectData = new ProjectViewModel
            {
                Id = projectId,
                Name = $"Project {projectId}",
                Description = $"Mô tả chi tiết cho Project {projectId}. Đây là một project trong bài thi MOS.",
                ExamId = (projectId % 2) + 1,
                ExamName = $"Exam {(projectId % 2) + 1}",
                Tasks = GetMockTasks(projectId),
                RemainingCredits = 15
            };
            
            return View(projectData);
        }

        // POST: Customer/Product/SubmitAnswer
        [HttpPost]
        public IActionResult SubmitAnswer(int taskId, string answer)
        {
            // Mock submission
            return Json(new { success = true, score = 85, correctAnswer = "Mock correct answer" });
        }

        // POST: Customer/Product/StartSession
        [HttpPost]
        public IActionResult StartSession(int productId, int examId, string sessionType)
        {
            // Mock session start - deduct credit
            return Json(new { success = true, remainingCredits = 14, sessionId = 12345 });
        }

        // Helper methods for mock data
        private List<ProjectViewModel> GetMockProjects(int examId)
        {
            var projects = new List<ProjectViewModel>();
            for (int i = 1; i <= 7; i++)
            {
                var project = new ProjectViewModel
                {
                    Id = examId * 10 + i,
                    Name = $"Project {i}",
                    Description = $"Project {i} cho Exam {examId}",
                    ExamId = examId,
                    TaskCount = 5,
                    CompletedTasks = i - 1,
                    IsCurrent = i == 1
                };
                // Add mock tasks for each project
                project.Tasks = GetMockTasks(project.Id);
                projects.Add(project);
            }
            return projects;
        }

        private List<TaskViewModel> GetMockTasks(int projectId)
        {
            var tasks = new List<TaskViewModel>();
            for (int i = 1; i <= 5; i++)
            {
                tasks.Add(new TaskViewModel
                {
                    Id = projectId * 10 + i,
                    ProjectId = projectId,
                    Question = $"Câu hỏi {i} cho Project {projectId}: Hãy thực hiện thao tác XYZ?",
                    Hint = $"Gợi ý {i}: Sử dụng menu File -> Save As...",
                    Answer = $"Đáp án {i}: Chọn File, sau đó chọn Save As, đặt tên file và nhấn Save.",
                    Order = i,
                    UserAnswer = "",
                    IsCorrect = null
                });
            }
            return tasks;
        }
    }

    // ViewModel classes
    public class ExamViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasPractice { get; set; }
        public bool HasTest { get; set; }
        public int Order { get; set; }
    }

    public class PracticeViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamType { get; set; } // "Practice" or "Test"
        public int RemainingTime { get; set; } // in seconds
        public int RemainingCredits { get; set; }
        public List<ProjectViewModel> Projects { get; set; }
        public int CurrentProjectId { get; set; }
    }

    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            Tasks = new List<TaskViewModel>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTasks { get; set; }
        public bool IsCurrent { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public int RemainingCredits { get; set; }
    }

    public class TaskViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Question { get; set; }
        public string Hint { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
        public string UserAnswer { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
