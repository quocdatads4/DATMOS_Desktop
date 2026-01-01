using DATMOS.WinApp.Grading;
using DATMOS.WinApp.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DATMOS.WinApp.Services
{
    public class GradingService
    {
        private readonly WordDocumentService _wordDocumentService;
        
        public GradingService(WordDocumentService wordDocumentService)
        {
            _wordDocumentService = wordDocumentService;
        }
        
        public WordGrader.GradingResult GradeDocument(string studentFilePath, string projectId)
        {
            try
            {
                GMetrixLogger.LogGradingStart(projectId, studentFilePath);
                
                var grader = new WordGrader();
                var result = grader.GradeDocument(studentFilePath, projectId);
                
                GMetrixLogger.LogGradingResult(projectId, result.TotalScore, result.MaxScore, result.Passed);
                GMetrixLogger.LogGradingDetails(projectId, result.Items);
                
                return result;
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("GradeDocument", ex);
                throw;
            }
        }
        
        // Get grading task info from solutions JSON
        public dynamic GetGradingTaskInfo(string projectId, int taskIndex)
        {
            try
            {
                string solutionsPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-word-2019-grading-solutions.json";
                if (!File.Exists(solutionsPath))
                {
                    GMetrixLogger.Log($"File đáp án không tồn tại: {solutionsPath}", LogLevel.WARNING);
                    return null;
                }
                
                string jsonContent = File.ReadAllText(solutionsPath);
                dynamic solutionsData = JsonConvert.DeserializeObject(jsonContent);
                
                if (solutionsData?.projects == null)
                {
                    GMetrixLogger.Log("Không tìm thấy projects trong file đáp án", LogLevel.WARNING);
                    return null;
                }
                
                // Find project by ID
                foreach (var project in solutionsData.projects)
                {
                    if (project.projectId.ToString() == projectId)
                    {
                        if (project.gradingTasks != null)
                        {
                            // Task index is 1-based in the JSON (taskId: 1, 2, 3, ...)
                            // But taskIndex parameter is 0-based from the loop
                            int taskIdToFind = taskIndex + 1;
                            
                            foreach (var task in project.gradingTasks)
                            {
                                if (task.taskId.ToString() == taskIdToFind.ToString())
                                {
                                    return task;
                                }
                            }
                            
                            // If not found by exact taskId, try to get by position
                            if (taskIndex < project.gradingTasks.Count)
                            {
                                return project.gradingTasks[taskIndex];
                            }
                        }
                        break;
                    }
                }
                
                GMetrixLogger.Log($"Không tìm thấy task {taskIndex + 1} cho project {projectId} trong file đáp án", LogLevel.WARNING);
                return null;
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("GetGradingTaskInfo", ex);
                return null;
            }
        }
        
        // Get student info from JSON
        public dynamic GetStudentInfo(string studentId = "1")
        {
            try
            {
                string jsonPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-students-data.json";
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
                    
                    if (jsonData.users != null)
                    {
                        foreach (var user in jsonData.users)
                        {
                            if (user.id.ToString() == studentId)
                            {
                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading student info from JSON: {ex.Message}");
            }
            
            // Return default student info if not found
            return new
            {
                id = "1",
                studentInfo = new
                {
                    fullName = "Nguyễn Văn An",
                    studentCode = "SV001"
                }
            };
        }
        
        // Save training result to JSON file
        public void SaveTrainingResultToJson(string projectId, WordGrader.GradingResult result, string studentFilePath)
        {
            try
            {
                GMetrixLogger.Log($"Bắt đầu lưu kết quả vào JSON cho Project {projectId}", LogLevel.INFO);
                
                // Get student info
                dynamic student = GetStudentInfo("1");
                if (student == null)
                {
                    GMetrixLogger.Log("Không tìm thấy thông tin học viên, sử dụng giá trị mặc định", LogLevel.WARNING);
                    student = new
                    {
                        id = "1",
                        studentInfo = new
                        {
                            fullName = "Nguyễn Văn An",
                            studentCode = "SV001"
                        }
                    };
                }
                
                string studentId = student.id?.ToString() ?? "1";
                string studentName = student.studentInfo?.fullName?.ToString() ?? "Nguyễn Văn An";
                string studentCode = student.studentInfo?.studentCode?.ToString() ?? "SV001";
                
                GMetrixLogger.Log($"Student info: ID={studentId}, Name={studentName}, Code={studentCode}", LogLevel.INFO);
                
                // Get project info from solutions JSON
                string projectName = $"MOS Word 2019 - Project {projectId}";
                string examName = $"MOS Word 2019 - Practice Test {projectId}";
                int projectMaxScore = 100;
                int passingScore = 70;
                
                // Try to get project info from solutions JSON
                try
                {
                    string solutionsPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-word-2019-grading-solutions.json";
                    if (File.Exists(solutionsPath))
                    {
                        string jsonContent = File.ReadAllText(solutionsPath);
                        dynamic solutionsData = JsonConvert.DeserializeObject(jsonContent);
                        
                        if (solutionsData?.projects != null)
                        {
                            foreach (var project in solutionsData.projects)
                            {
                                if (project.projectId?.ToString() == projectId)
                                {
                                    projectName = project.projectName?.ToString() ?? projectName;
                                    projectMaxScore = project.maxScore?.ToObject<int>() ?? 100;
                                    passingScore = project.passingScore?.ToObject<int>() ?? 70;
                                    GMetrixLogger.Log($"Project info từ solutions: Name={projectName}, MaxScore={projectMaxScore}, Passing={passingScore}", LogLevel.INFO);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        GMetrixLogger.Log($"File solutions không tồn tại: {solutionsPath}", LogLevel.WARNING);
                    }
                }
                catch (Exception ex)
                {
                    GMetrixLogger.LogError("GetProjectInfoFromSolutions", ex);
                    // Continue with default values
                }
                
                // Calculate next ID (TR005, TR006, etc.)
                string resultsJsonPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-word-2019-traning-results.json";
                dynamic resultsData = null;
                
                GMetrixLogger.Log($"Đường dẫn file kết quả: {resultsJsonPath}", LogLevel.INFO);
                
                if (File.Exists(resultsJsonPath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(resultsJsonPath);
                        resultsData = JsonConvert.DeserializeObject(jsonContent);
                        GMetrixLogger.Log($"Đã đọc file kết quả hiện tại", LogLevel.INFO);
                    }
                    catch (Exception ex)
                    {
                        GMetrixLogger.LogError("ReadExistingResultsFile", ex);
                        resultsData = null;
                    }
                }
                else
                {
                    GMetrixLogger.Log($"File kết quả chưa tồn tại, sẽ tạo mới", LogLevel.INFO);
                }
                
                // Generate next ID
                string nextId = "TR001"; // Default
                if (resultsData != null && resultsData.trainingResults != null)
                {
                    try
                    {
                        int maxId = 0;
                        foreach (var trainingResult in resultsData.trainingResults)
                        {
                            string idStr = trainingResult.id?.ToString() ?? "";
                            if (idStr.StartsWith("TR"))
                            {
                                string numStr = idStr.Substring(2);
                                if (int.TryParse(numStr, out int num))
                                {
                                    if (num > maxId) maxId = num;
                                }
                            }
                        }
                        nextId = $"TR{maxId + 1:D3}";
                        GMetrixLogger.Log($"Generated next ID: {nextId} (maxId={maxId})", LogLevel.INFO);
                    }
                    catch (Exception ex)
                    {
                        GMetrixLogger.LogError("GenerateNextID", ex);
                        nextId = $"TR{DateTime.Now:yyyyMMddHHmmss}";
                    }
                }
                else
                {
                    nextId = "TR001";
                    GMetrixLogger.Log($"Sử dụng ID mặc định: {nextId}", LogLevel.INFO);
                }
                
                // Create task results from grading items with info from solutions JSON
                var taskResults = new List<object>();
                int taskIndex = 0;
                GMetrixLogger.Log($"Tạo task results, tổng số items: {result.Items.Count}", LogLevel.INFO);
                
                foreach (var item in result.Items)
                {
                    try
                    {
                        dynamic taskInfo = GetGradingTaskInfo(projectId, taskIndex);
                        
                        string taskName = item.Description ?? "Unknown Task";
                        string shortName = item.Description ?? "Unknown";
                        string description = item.Description ?? "No description";
                        int maxPoints = item.MaxScore;
                        string correctAnswer = item.Description ?? "Unknown";
                        
                        if (taskInfo != null)
                        {
                            taskName = taskInfo.taskName?.ToString() ?? taskName;
                            shortName = taskInfo.shortName?.ToString() ?? shortName;
                            maxPoints = taskInfo.maxScore?.ToObject<int>() ?? maxPoints;
                            
                            // Get description from grading criteria
                            if (taskInfo.gradingCriteria != null)
                            {
                                try
                                {
                                    if (taskInfo.gradingCriteria.verificationSteps != null && taskInfo.gradingCriteria.verificationSteps.Count > 0)
                                    {
                                        var steps = taskInfo.gradingCriteria.verificationSteps.ToObject<List<string>>();
                                        description = string.Join("; ", steps);
                                    }
                                    else if (taskInfo.gradingCriteria.validationRules != null && taskInfo.gradingCriteria.validationRules.Count > 0)
                                    {
                                        var rules = taskInfo.gradingCriteria.validationRules.ToObject<List<dynamic>>();
                                        if (rules.Count > 0)
                                        {
                                            description = rules[0].description?.ToString() ?? description;
                                        }
                                    }
                                }
                                catch (Exception innerEx)
                                {
                                    GMetrixLogger.LogError("GetTaskDescription", innerEx);
                                }
                            }
                            
                            // Create correct answer from task info
                            correctAnswer = $"{shortName}: {taskName}";
                        }
                        
                        taskResults.Add(new
                        {
                            taskId = taskIndex + 1,
                            taskName = taskName,
                            shortName = shortName,
                            description = description,
                            isCorrect = item.IsCorrect,
                            userAnswer = item.IsCorrect ? "Đã thực hiện đúng yêu cầu" : "Chưa thực hiện đúng yêu cầu",
                            correctAnswer = correctAnswer,
                            points = item.Score,
                            maxPoints = maxPoints,
                            feedback = item.Feedback ?? (item.IsCorrect ? "Hoàn thành tốt" : "Cần cải thiện"),
                            timeSpentSeconds = 120 // Default value
                        });
                        taskIndex++;
                    }
                    catch (Exception innerEx)
                    {
                        GMetrixLogger.LogError("CreateTaskResultItem", innerEx);
                        // Add basic task result even if error occurs
                        taskResults.Add(new
                        {
                            taskId = taskIndex + 1,
                            taskName = item.Description ?? "Unknown Task",
                            shortName = item.Description ?? "Unknown",
                            description = item.Description ?? "No description",
                            isCorrect = item.IsCorrect,
                            userAnswer = item.IsCorrect ? "Đã thực hiện" : "Chưa thực hiện",
                            correctAnswer = item.Description ?? "Unknown",
                            points = item.Score,
                            maxPoints = item.MaxScore,
                            feedback = item.Feedback ?? (item.IsCorrect ? "OK" : "Need improvement"),
                            timeSpentSeconds = 120
                        });
                        taskIndex++;
                    }
                }
                
                GMetrixLogger.Log($"Đã tạo {taskResults.Count} task results", LogLevel.INFO);
                
                // Create summary with task names from solutions JSON
                var correctTaskNames = new List<string>();
                var incorrectTaskNames = new List<string>();
                
                taskIndex = 0;
                foreach (var item in result.Items)
                {
                    try
                    {
                        dynamic taskInfo = GetGradingTaskInfo(projectId, taskIndex);
                        string taskDisplayName = item.Description ?? "Unknown Task";
                        
                        if (taskInfo != null)
                        {
                            taskDisplayName = taskInfo.shortName?.ToString() ?? taskInfo.taskName?.ToString() ?? taskDisplayName;
                        }
                        
                        if (item.IsCorrect)
                        {
                            correctTaskNames.Add(taskDisplayName);
                        }
                        else
                        {
                            incorrectTaskNames.Add(taskDisplayName);
                        }
                    }
                    catch (Exception ex)
                    {
                        GMetrixLogger.LogError("CreateSummaryTaskNames", ex);
                        // Use default name
                        if (item.IsCorrect)
                        {
                            correctTaskNames.Add(item.Description ?? "Task");
                        }
                        else
                        {
                            incorrectTaskNames.Add(item.Description ?? "Task");
                        }
                    }
                    taskIndex++;
                }
                
                var summary = new
                {
                    correctTasks = result.Items.Count(i => i.IsCorrect),
                    incorrectTasks = result.Items.Count(i => !i.IsCorrect),
                    partiallyCorrectTasks = 0,
                    averageTimePerTask = 140,
                    difficultyLevel = "Intermediate",
                    strengths = correctTaskNames.Take(3).ToList(),
                    weaknesses = incorrectTaskNames.Take(3).ToList(),
                    recommendations = result.Passed ? 
                        "Tiếp tục thực hành để nâng cao kỹ năng" : 
                        "Cần ôn tập lại các phần chưa đạt"
                };
                
                // Create new training result entry
                var newTrainingResult = new
                {
                    id = nextId,
                    studentId = studentId,
                    studentName = studentName,
                    studentCode = studentCode,
                    examId = $"EXAM{projectId:D3}",
                    examName = examName,
                    courseCode = "CRS001",
                    courseName = "MOS Word 2019",
                    projectId = int.Parse(projectId),
                    projectName = projectName,
                    startTime = DateTime.Now.AddMinutes(-45).ToString("yyyy-MM-ddTHH:mm:ss"), // 45 minutes ago
                    endTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    durationMinutes = 45,
                    totalTasks = result.Items.Count,
                    completedTasks = result.Items.Count,
                    score = result.TotalScore,
                    maxScore = projectMaxScore,
                    percentage = (double)result.TotalScore / projectMaxScore * 100,
                    passed = result.TotalScore >= passingScore,
                    passingScore = passingScore,
                    status = "Completed",
                    submissionDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    gradedBy = "System",
                    taskResults = taskResults,
                    summary = summary
                };
                
                // Add to existing data or create new
                if (resultsData == null)
                {
                    resultsData = new
                    {
                        trainingResults = new List<object> { newTrainingResult },
                        metadata = new
                        {
                            version = "1.0.0",
                            createdDate = DateTime.Now.ToString("yyyy-MM-dd"),
                            description = "Dữ liệu kết quả bài thi training MOS Word 2019 - Lưu điểm số và chi tiết câu trả lời",
                            totalResults = 1,
                            totalPassed = result.TotalScore >= passingScore ? 1 : 0,
                            totalFailed = result.TotalScore >= passingScore ? 0 : 1,
                            averageScore = result.TotalScore,
                            dataSource = "JSON File (Auto-generated from grading solutions)",
                            lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                        }
                    };
                    GMetrixLogger.Log($"Tạo file kết quả mới với ID {nextId}", LogLevel.INFO);
                }
                else
                {
                    // Add to existing trainingResults
                    try
                    {
                        // Convert resultsData to JObject for safe manipulation
                        string jsonString = JsonConvert.SerializeObject(resultsData);
                        var jObject = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonString);
                        
                        // Get trainingResults as JArray
                        var trainingResultsArray = jObject["trainingResults"] as Newtonsoft.Json.Linq.JArray;
                        if (trainingResultsArray == null)
                        {
                            trainingResultsArray = new Newtonsoft.Json.Linq.JArray();
                            jObject["trainingResults"] = trainingResultsArray;
                        }
                        
                        // Add new training result
                        var newTrainingResultJson = Newtonsoft.Json.Linq.JObject.FromObject(newTrainingResult);
                        trainingResultsArray.Add(newTrainingResultJson);
                        
                        // Update metadata
                        int totalResults = trainingResultsArray.Count;
                        int totalPassed = trainingResultsArray.Where(r => r["passed"]?.ToObject<bool>() == true).Count();
                        int totalFailed = totalResults - totalPassed;
                        double averageScore = trainingResultsArray.Average(r => r["score"]?.ToObject<double>() ?? 0);
                        
                        jObject["metadata"]["totalResults"] = totalResults;
                        jObject["metadata"]["totalPassed"] = totalPassed;
                        jObject["metadata"]["totalFailed"] = totalFailed;
                        jObject["metadata"]["averageScore"] = averageScore;
                        jObject["metadata"]["lastUpdated"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        
                        resultsData = jObject;
                        GMetrixLogger.Log($"Đã cập nhật file kết quả với ID {nextId}", LogLevel.INFO);
                    }
                    catch (Exception ex)
                    {
                        GMetrixLogger.LogError("UpdateExistingResultsFile", ex);
                        // Fallback: create new data structure
                        resultsData = new
                        {
                            trainingResults = new List<object> { newTrainingResult },
                            metadata = new
                            {
                                version = "1.0.0",
                                createdDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                description = "Dữ liệu kết quả bài thi training MOS Word 2019 - Lưu điểm số và chi tiết câu trả lời",
                                totalResults = 1,
                                totalPassed = result.TotalScore >= passingScore ? 1 : 0,
                                totalFailed = result.TotalScore >= passingScore ? 0 : 1,
                                averageScore = result.TotalScore,
                                dataSource = "JSON File (Auto-generated from grading solutions)",
                                lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                            }
                        };
                        GMetrixLogger.Log($"Tạo file kết quả mới (fallback) với ID {nextId}", LogLevel.WARNING);
                    }
                }
                
                // Save to file
                string jsonToSave = JsonConvert.SerializeObject(resultsData, Formatting.Indented);
                File.WriteAllText(resultsJsonPath, jsonToSave);
                
                GMetrixLogger.Log($"Đã lưu kết quả vào file: {resultsJsonPath}", LogLevel.INFO);
                GMetrixLogger.Log($"Kết quả đã được lưu với ID: {nextId}", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("SaveTrainingResultToJson", ex);
                throw;
            }
        }
    }
}
