using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using DATMOS.Web.Areas.Customer.Models;

namespace DATMOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
[Authorize(Roles = "User")]
    public class Word2019ResultsController : Controller
    {
        public IActionResult Index()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "areas", "customer", "json", "dmos-word-2019-traning-results.json");
            var json = System.IO.File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TrainingData>(json);
            var results = data.TrainingResults.Where(r => r.StudentId == "1").ToList();
            return View(results);
        }

        public IActionResult AllResults()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "areas", "customer", "json", "dmos-word-2019-traning-results.json");
            var json = System.IO.File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TrainingData>(json);
            return View(data);
        }

        public IActionResult StudentResult(string studentId, string examId)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "areas", "customer", "json", "dmos-word-2019-traning-results.json");
            var json = System.IO.File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TrainingData>(json);
            var result = data.TrainingResults.FirstOrDefault(r => r.StudentId == studentId && r.ExamId == examId);
            return View("Details", result);
        }

        public IActionResult Details(string id)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "areas", "customer", "json", "dmos-word-2019-traning-results.json");
            var json = System.IO.File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TrainingData>(json);
            var result = data.TrainingResults.FirstOrDefault(r => r.Id == id);
            
            if (result == null)
            {
                return NotFound();
            }
            
            return View(result);
        }
    }
}
