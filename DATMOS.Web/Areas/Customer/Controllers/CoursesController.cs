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
public class CoursesController : Controller
    {
        private readonly ICoursesService _coursesService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICoursesService coursesService, ILogger<CoursesController> logger)
        {
            _coursesService = coursesService;
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị danh sách tất cả khóa học
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var courses = await _coursesService.GetAllCoursesAsync();
                ViewData["Title"] = "Khóa học";
                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị chi tiết khóa học
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var courseDetails = await _coursesService.GetCourseDetailsAsync(id);
                
                if (courseDetails == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = courseDetails.Course.Name;
                return View(courseDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading course details for id: {id}");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị khóa học theo môn học
        /// </summary>
        public async Task<IActionResult> BySubject(int subjectId)
        {
            try
            {
                var courses = await _coursesService.GetCoursesBySubjectAsync(subjectId);
                ViewData["Title"] = "Khóa học theo môn";
                ViewData["SubjectId"] = subjectId;
                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading courses for subject: {subjectId}");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị khóa học miễn phí
        /// </summary>
        public async Task<IActionResult> Free()
        {
            try
            {
                var courses = await _coursesService.GetFreeCoursesAsync();
                ViewData["Title"] = "Khóa học miễn phí";
                return View("Index", courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading free courses");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị khóa học trả phí
        /// </summary>
        public async Task<IActionResult> Paid()
        {
            try
            {
                var courses = await _coursesService.GetPaidCoursesAsync();
                ViewData["Title"] = "Khóa học trả phí";
                return View("Index", courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading paid courses");
                return View("Error");
            }
        }

        /// <summary>
        /// Tìm kiếm khóa học
        /// </summary>
        public async Task<IActionResult> Search(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return RedirectToAction(nameof(Index));
                }

                var courses = await _coursesService.SearchCoursesAsync(keyword);
                ViewData["Title"] = $"Tìm kiếm: {keyword}";
                ViewData["SearchKeyword"] = keyword;
                return View("Index", courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching courses with keyword: {keyword}");
                return View("Error");
            }
        }

        /// <summary>
        /// API: Lấy danh sách tất cả khóa học (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/courses")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var courses = await _coursesService.GetAllCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll API");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// API: Lấy khóa học theo ID (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/courses/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var course = await _coursesService.GetCourseByIdAsync(id);
                
                if (course == null)
                {
                    return NotFound(new { error = "Course not found" });
                }

                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetById API for id: {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// API: Lấy thống kê khóa học (JSON)
        /// </summary>
        [HttpGet]
        [Route("api/courses/statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _coursesService.GetCourseStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatistics API");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Trang đăng ký khóa học
        /// </summary>
        public async Task<IActionResult> Enroll(int id)
        {
            try
            {
                var course = await _coursesService.GetCourseByIdAsync(id);
                
                if (course == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = $"Đăng ký: {course.Name}";
                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading enroll page for course id: {id}");
                return View("Error");
            }
        }

        /// <summary>
        /// Hiển thị khóa học của người dùng
        /// </summary>
        public async Task<IActionResult> MyCourses()
        {
            try
            {
                // TODO: Lấy danh sách khóa học của người dùng hiện tại
                // Hiện tại trả về danh sách rỗng
                var courses = new List<DATMOS.Web.Areas.Customer.ViewModels.CourseViewModel>();
                ViewData["Title"] = "Khóa học của tôi";
                return View("Index", courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading my courses");
                return View("Error");
            }
        }
    }
}
