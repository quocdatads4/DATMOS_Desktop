using DATMOS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
[Authorize(Roles = "User")]
public class LessonController : Controller
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonController> _logger;

        public LessonController(ILessonService lessonService, ILogger<LessonController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị danh sách tất cả bài học
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                ViewData["Title"] = "Bài học";
                return View(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lessons");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị chi tiết bài học
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var lessonDetails = await _lessonService.GetLessonDetailsAsync(id);
                
                if (lessonDetails == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = lessonDetails.Lesson.Name;
                return View(lessonDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading lesson details for id: {id}");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị bài học theo khóa học
        /// </summary>
        public async Task<IActionResult> ByCourse(int courseId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseAsync(courseId);
                ViewData["Title"] = "Bài học theo khóa";
                ViewData["CourseId"] = courseId;
                return View(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading lessons for course: {courseId}");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị bài học miễn phí
        /// </summary>
        public async Task<IActionResult> Free()
        {
            try
            {
                var lessons = await _lessonService.GetFreeLessonsAsync();
                ViewData["Title"] = "Bài học miễn phí";
                return View("Index", lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading free lessons");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị bài học trả phí
        /// </summary>
        public async Task<IActionResult> Paid()
        {
            try
            {
                var lessons = await _lessonService.GetPaidLessonsAsync();
                ViewData["Title"] = "Bài học trả phí";
                return View("Index", lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading paid lessons");
                return View("Error");
            }
        }

        /// <summary>
        /// Tìm kiếm bài học
        /// </summary>
        public async Task<IActionResult> Search(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return RedirectToAction(nameof(Index));
                }

                var lessons = await _lessonService.SearchLessonsAsync(keyword);
                ViewData["Title"] = $"Tìm kiếm: {keyword}";
                ViewData["SearchKeyword"] = keyword;
                return View("Index", lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching lessons with keyword: {keyword}");
                return View("Error");
            }
        }

        /// <summary>
        /// Bài học tiếp theo
        /// </summary>
        public async Task<IActionResult> Next(int currentLessonId)
        {
            try
            {
                var nextLesson = await _lessonService.GetNextLessonAsync(currentLessonId);
                
                if (nextLesson == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Details), new { id = nextLesson.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting next lesson for id: {currentLessonId}");
                return View("Error");
            }
        }

        /// <summary>
        /// Bài học trước đó
        /// </summary>
        public async Task<IActionResult> Previous(int currentLessonId)
        {
            try
            {
                var previousLesson = await _lessonService.GetPreviousLessonAsync(currentLessonId);
                
                if (previousLesson == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Details), new { id = previousLesson.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting previous lesson for id: {currentLessonId}");
                return View("Error");
            }
        }

        /// <summary>
        /// API: Lấy danh sách tất cả bài học (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/lessons")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll API");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// API: Lấy bài học theo ID (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/lessons/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                
                if (lesson == null)
                {
                    return NotFound(new { error = "Lesson not found" });
                }

                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetById API for id: {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// API: Lấy bài học theo khóa học (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/lessons/course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseAsync(courseId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByCourse API for courseId: {courseId}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// API: Lấy thống kê bài học (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/lessons/statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _lessonService.GetLessonStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatistics API");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Trang học bài học
        /// </summary>
        public async Task<IActionResult> Study(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                
                if (lesson == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = $"Học: {lesson.Name}";
                return View(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading study page for lesson id: {id}");
                return View("Error");
            }
        }
    }
}
