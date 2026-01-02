using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace DATMOS.Modules.Template.Controllers
{
    /// <summary>
    /// Template controller for DATMOS modules
    /// Replace [Template] with actual module name when using this template
    /// </summary>
    [Area("[ModuleName]")]
    [Route("[module-name]")]
    public class TemplateController : Controller
    {
        private readonly ILogger<TemplateController> _logger;
        private readonly ModuleOptions _options;

        public TemplateController(
            ILogger<TemplateController> logger,
            IOptions<ModuleOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _logger.LogInformation("TemplateController initialized for module: {ModuleName}", 
                _options.ModuleName);
        }

        /// <summary>
        /// Default action - module dashboard/home
        /// </summary>
        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            ViewData["Title"] = $"{_options.ModuleName} Dashboard";
            ViewData["ModuleName"] = _options.ModuleName;
            ViewData["ModuleVersion"] = _options.Version;
            
            _logger.LogDebug("Accessing {ModuleName} dashboard", _options.ModuleName);
            
            return View();
        }

        /// <summary>
        /// Example action with parameter
        /// </summary>
        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            ViewData["Title"] = $"Details - {_options.ModuleName}";
            ViewData["ItemId"] = id;
            
            _logger.LogInformation("Viewing details for item {ItemId} in module {ModuleName}", 
                id, _options.ModuleName);
            
            return View();
        }

        /// <summary>
        /// Example API endpoint
        /// </summary>
        [HttpGet("api/data")]
        [Produces("application/json")]
        public IActionResult GetData()
        {
            var data = new
            {
                Module = _options.ModuleName,
                Version = _options.Version,
                Timestamp = DateTime.UtcNow,
                Status = "Active",
                Features = new
                {
                    EnableCaching = _options.EnableCaching,
                    EnableExport = _options.EnableExport,
                    EnableImport = _options.EnableImport
                }
            };
            
            return Ok(data);
        }

        /// <summary>
        /// Health check endpoint for module
        /// </summary>
        [HttpGet("health")]
        [Produces("application/json")]
        public IActionResult Health()
        {
            var healthStatus = new
            {
                Module = _options.ModuleName,
                Status = _options.IsEnabled ? "Healthy" : "Disabled",
                Timestamp = DateTime.UtcNow,
                Uptime = GetUptime(),
                MemoryUsage = GetMemoryUsage()
            };
            
            return Ok(healthStatus);
        }

        /// <summary>
        /// Example POST action
        /// </summary>
        [HttpPost("submit")]
        [ValidateAntiForgeryToken]
        public IActionResult Submit([FromForm] TemplateModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for submission in module {ModuleName}", 
                    _options.ModuleName);
                return View("Index", model);
            }

            try
            {
                // Process submission
                _logger.LogInformation("Processing submission in module {ModuleName}", 
                    _options.ModuleName);
                
                TempData["SuccessMessage"] = "Submission successful!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing submission in module {ModuleName}", 
                    _options.ModuleName);
                
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View("Index", model);
            }
        }

        /// <summary>
        /// Error handling example
        /// </summary>
        [HttpGet("error")]
        public IActionResult Error()
        {
            var errorInfo = new
            {
                RequestId = HttpContext.TraceIdentifier,
                Module = _options.ModuleName,
                Timestamp = DateTime.UtcNow
            };
            
            return View(errorInfo);
        }

        // Helper methods
        private string GetUptime()
        {
            // In a real implementation, this would track module uptime
            return "00:00:00"; // Placeholder
        }

        private string GetMemoryUsage()
        {
            // In a real implementation, this would get actual memory usage
            return "0 MB"; // Placeholder
        }
    }

    /// <summary>
    /// Template model for form submissions
    /// </summary>
    public class TemplateModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Value must be between 1 and 100")]
        public int Value { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
    }
}
