using DATMOS.Core.Entities;
using DATMOS.Data.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DATMOS.Data
{
    public class ExamTaskSeeder
    {
        private static readonly TaskGenerator _taskGenerator = new TaskGenerator();
        public static async Task SeedAsync(AppDbContext context)
        {
            // Check if there are already ExamTasks
            if (await context.ExamTasks.AnyAsync())
            {
                Console.WriteLine("ExamTasks already seeded. Skipping...");
                return;
            }

            // Get all projects
            var projects = await context.ExamProjects.ToListAsync();
            if (!projects.Any())
            {
                Console.WriteLine("No ExamProjects found. Please seed ExamProjects first.");
                return;
            }

            Console.WriteLine($"Found {projects.Count} ExamProjects. Reading tasks from JSON...");

            try
            {
                // Locate the JSON file
                // Assuming the structure is DATMOS_Desktop/DATMOS.Data and DATMOS_Desktop/DATMOS.Web
                // We try to find the file relative to the execution directory
                string baseDir = AppContext.BaseDirectory;
                string jsonPath = "";
                
                // Try to find the solution root by going up directories
                DirectoryInfo dir = new DirectoryInfo(baseDir);
                while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "DATMOS.Web")))
                {
                    dir = dir.Parent;
                }

                if (dir != null)
                {
                    jsonPath = Path.Combine(dir.FullName, "DATMOS.Web", "wwwroot", "areas", "customer", "json", "exam-tasks.json");
                }
                else
                {
                    // Fallback for specific environment or absolute path provided by user
                    jsonPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\exam-tasks.json";
                }

                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"JSON file not found at: {jsonPath}. Falling back to dummy data.");
                    // Fallback logic could go here, but for now we return or throw
                    return;
                }

                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var taskData = JsonSerializer.Deserialize<TaskDataWrapper>(jsonContent, options);

                var tasksToAdd = new List<ExamTask>();
                
                // Group JSON tasks by ProjectId
                var tasksByProject = taskData.Tasks.GroupBy(t => t.ProjectId).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var project in projects)
                {
                    if (tasksByProject.ContainsKey(project.Id))
                    {
                        var jsonTasks = tasksByProject[project.Id];
                        
                        // Add tasks from JSON (usually 5)
                        foreach (var jsonTask in jsonTasks)
                        {
                            tasksToAdd.Add(new ExamTask
                            {
                                ExamProjectId = project.Id,
                                Name = jsonTask.Name,
                                Description = jsonTask.Description,
                                Instructions = GenerateInstructions(jsonTask.Name), // Generate generic instructions
                                OrderIndex = jsonTask.Order,
                                MaxScore = 100.0 / 6.0, // Score for 6 tasks
                                TaskType = InferTaskType(jsonTask.Name),
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        // Add the 6th task (Required by system)
                        tasksToAdd.Add(new ExamTask
                        {
                            ExamProjectId = project.Id,
                            Name = "Task 6: Save & Close",
                            Description = "Lưu tài liệu và đóng cửa sổ làm việc.",
                            Instructions = "Bước 1: Nhấn vào biểu tượng Save trên thanh Quick Access Toolbar.\nBước 2: Vào File > Close để đóng tài liệu.",
                            OrderIndex = 6,
                            MaxScore = 100.0 / 6.0,
                            TaskType = "File",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // Fallback if project ID not in JSON: Use TaskGenerator to create realistic tasks
                        // Get subject type from project's exam list and subject
                        var subjectType = await GetSubjectTypeForProject(context, project.Id);
                        var generatedTasks = _taskGenerator.GenerateTasksForProject(project.Id, subjectType);
                        tasksToAdd.AddRange(generatedTasks);
                    }
                }

                // Insert in batches to avoid performance issues
                await context.ExamTasks.AddRangeAsync(tasksToAdd);
                await context.SaveChangesAsync();

                Console.WriteLine($"Successfully seeded {tasksToAdd.Count} ExamTasks from JSON source.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding ExamTasks: {ex.Message}");
                throw;
            }
        }

        // Helper classes for JSON deserialization
        private class TaskDataWrapper
        {
            public List<JsonTaskItem> Tasks { get; set; }
        }

        private class JsonTaskItem
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Order { get; set; }
        }

        private static string GenerateInstructions(string taskName)
        {
            return $"Thực hiện các bước để hoàn thành nhiệm vụ: {taskName}.\nSử dụng các công cụ trên thanh Ribbon để thao tác.";
        }

        private static string InferTaskType(string taskName)
        {
            taskName = taskName.ToLower();
            if (taskName.Contains("table")) return "Table";
            if (taskName.Contains("image") || taskName.Contains("picture") || taskName.Contains("shape")) return "Insert";
            if (taskName.Contains("chart")) return "Chart";
            if (taskName.Contains("style") || taskName.Contains("font")) return "Format";
            return "General";
        }

        private static string GetTaskName(int index)
        {
            return index switch
            {
                1 => "Định dạng văn bản",
                2 => "Chèn đối tượng",
                3 => "Thiết lập trang",
                4 => "Tạo bảng biểu",
                5 => "Tham chiếu & Chú thích",
                6 => "Lưu & Xuất bản",
                _ => "Nhiệm vụ chung"
            };
        }

        private static string GetTaskInstructions(int index)
        {
            return $"Bước 1: Chọn đối tượng cần xử lý.\nBước 2: Vào tab tương ứng trên thanh Ribbon.\nBước 3: Thực hiện cấu hình theo yêu cầu Task {index}.";
        }

        private static string GetTaskType(int index)
        {
            string[] types = { "Format", "Insert", "Layout", "Table", "Reference", "File" };
            return types[(index - 1) % types.Length];
        }

        private static async Task<string> GetSubjectTypeForProject(AppDbContext context, int projectId)
        {
            var project = await context.ExamProjects
                .Include(p => p.ExamList)
                    .ThenInclude(l => l.Subject)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project?.ExamList?.Subject != null)
            {
                var subjectName = project.ExamList.Subject.Name.ToLower();
                if (subjectName.Contains("word")) return "Word";
                if (subjectName.Contains("excel")) return "Excel";
                if (subjectName.Contains("powerpoint")) return "PowerPoint";
            }

            return "Word"; // Default fallback
        }
    }
}
