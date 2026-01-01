using DATMOS.WinApp.Grading;
using DATMOS.WinApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DATMOS.WinApp.Services
{
    public class WordDocumentService
    {
        private readonly Dictionary<string, string> _studentFilePaths = new Dictionary<string, string>();
        
        public WordDocumentService()
        {
        }
        
        public void OpenWordDocument(string projectId)
        {
            try
            {
                // Close any existing Word processes before opening new one
                CloseWordProcesses();
                
                // Create student work file in P1SWork folder
                string studentFilePath = CreateStudentWorkFile(projectId);
                
                if (!string.IsNullOrEmpty(studentFilePath) && File.Exists(studentFilePath))
                {
                    // Open student work file with default application
                    Process process = Process.Start(new ProcessStartInfo
                    {
                        FileName = studentFilePath,
                        UseShellExecute = true
                    });
                    
                    // Wait a moment for Word to open
                    Thread.Sleep(1000);
                    
                    // Try to resize Word window to occupy 80% of screen
                    ResizeWordWindow();
                    
                    // Log successful opening
                    GMetrixLogger.Log($"Đã mở file bài làm cho học viên: {studentFilePath}", LogLevel.INFO);
                }
                else
                {
                    // Fallback to original template file
                    string filePath = GetProjectFilePath(projectId);
                    
                    if (File.Exists(filePath))
                    {
                        Process process = Process.Start(new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                        
                        Thread.Sleep(1000);
                        ResizeWordWindow();
                        
                        GMetrixLogger.Log($"Đã mở file template (fallback): {filePath}", LogLevel.WARNING);
                    }
                    else
                    {
                        throw new FileNotFoundException($"Không tìm thấy file: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("OpenWordDocument", ex);
                throw;
            }
        }
        
        // Try to resize Word window using Windows API
        private void ResizeWordWindow()
        {
            try
            {
                // Get screen dimensions
                var screen = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                int wordHeight = (int)(screen.Height * 0.8); // 80% of screen
                
                // Try to find Word window
                IntPtr wordWindow = WindowsApiHelper.GetWordWindowHandle();
                
                if (wordWindow != IntPtr.Zero)
                {
                    // Set window position and size
                    WindowsApiHelper.SetWindowPos(wordWindow, IntPtr.Zero, 0, 0, screen.Width, wordHeight, 
                        WindowsApiHelper.SWP_NOZORDER | WindowsApiHelper.SWP_NOACTIVATE);
                    
                    // Show window normally
                    WindowsApiHelper.ShowWindow(wordWindow, WindowsApiHelper.SW_SHOWNORMAL);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resizing Word window: {ex.Message}");
                // Continue even if resizing fails
            }
        }
        
        // Close all Word processes automatically
        private void CloseWordProcesses()
        {
            try
            {
                // Get all Word processes
                var wordProcesses = Process.GetProcessesByName("WINWORD");
                
                if (wordProcesses.Length > 0)
                {
                    int closedCount = 0;
                    
                    foreach (var process in wordProcesses)
                    {
                        try
                        {
                            // Try to close gracefully first
                            if (!process.HasExited)
                            {
                                process.CloseMainWindow();
                                
                                // Wait for the process to exit
                                if (!process.WaitForExit(2000))
                                {
                                    // Force kill if not responding
                                    process.Kill();
                                }
                                
                                closedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error closing Word process: {ex.Message}");
                            // Continue with other processes
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CloseWordProcesses: {ex.Message}");
                // Continue opening new file even if closing fails
            }
        }
        
        // Kill all Word processes forcefully to ensure file is unlocked
        public void KillWordProcesses()
        {
            try
            {
                // Get all Word processes
                var wordProcesses = Process.GetProcessesByName("WINWORD");
                
                if (wordProcesses.Length > 0)
                {
                    int killedCount = 0;
                    
                    foreach (var process in wordProcesses)
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                // Force kill immediately
                                process.Kill();
                                killedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error killing Word process: {ex.Message}");
                            // Continue with other processes
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    }
                    
                    GMetrixLogger.Log($"Killed {killedCount} Word processes to ensure file unlock", LogLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("KillWordProcesses", ex);
            }
        }
        
        // Get file path based on project ID - UPDATED to use correct template files
        private string GetProjectFilePath(string projectId)
        {
            string basePath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.WinApp\bin\Debug\DATMOSTemplates\Word2019";
            
            switch (projectId)
            {
                case "1":
                    // Use Project1.docx from P1Demo folder instead of Bicycles.docx
                    return Path.Combine(basePath, "Project1", "P1Demo", "Project1.docx");
                case "2":
                    // Use Project2.docx from P2Demo folder
                    return Path.Combine(basePath, "Project2", "P2Demo", "Project2.docx");
                case "3":
                    // Nếu có Project 3, thêm đường dẫn tương ứng
                    // Tạm thời trả về file Project 2
                    return Path.Combine(basePath, "Project2", "P2Demo", "Project2.docx");
                default:
                    // Mặc định trả về Project 1
                    return Path.Combine(basePath, "Project1", "P1Demo", "Project1.docx");
            }
        }
        
        // Get demo file path from JSON data (more accurate)
        private string GetDemoFilePath(string projectId)
        {
            try
            {
                string jsonPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-word-2019-project-task.json";
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);
                    
                    if (jsonData.projects != null)
                    {
                        foreach (var project in jsonData.projects)
                        {
                            if (project.id.ToString() == projectId)
                            {
                                string demoPath = project.demoFilePath.ToString();
                                // Fix path: replace double backslashes with single backslashes
                                demoPath = demoPath.Replace("\\\\", "\\");
                                if (File.Exists(demoPath))
                                {
                                    GMetrixLogger.Log($"Found demo file path from JSON: {demoPath}", LogLevel.INFO);
                                    return demoPath;
                                }
                                else
                                {
                                    GMetrixLogger.Log($"Demo file not found at path: {demoPath}", LogLevel.WARNING);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("GetDemoFilePath", ex);
                Console.WriteLine($"Error reading demo file path from JSON: {ex.Message}");
            }
            
            // Fallback to default path
            string fallbackPath = GetProjectFilePath(projectId);
            GMetrixLogger.Log($"Using fallback path: {fallbackPath}", LogLevel.WARNING);
            return fallbackPath;
        }
        
        // Remove diacritics from Vietnamese text
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
                
            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            
            foreach (char c in normalizedString)
            {
                System.Globalization.UnicodeCategory unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            
            string result = stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
            
            // Replace specific Vietnamese characters
            result = result.Replace("Đ", "D").Replace("đ", "d");
            
            // Remove spaces and convert to lowercase for folder name
            result = Regex.Replace(result, @"\s+", "").ToLower();
            
            return result;
        }
        
        // Get student info from JSON
        private dynamic GetStudentInfo(string studentId = "1")
        {
            try
            {
                string jsonPath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.Web\wwwroot\areas\customer\json\dmos-students-data.json";
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);
                    
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
                    fullName = "Nguyễn Văn An"
                }
            };
        }
        
        // Create student work file in P1SWork folder
        public string CreateStudentWorkFile(string projectId, string studentId = "1")
        {
            try
            {
                GMetrixLogger.Log($"Bắt đầu tạo file bài làm cho Project {projectId}, Student {studentId}", LogLevel.INFO);
                
                // Get student info
                dynamic student = GetStudentInfo(studentId);
                string studentName = student.studentInfo.fullName.ToString();
                string studentNameNoDiacritics = RemoveDiacritics(studentName);
                
                GMetrixLogger.Log($"Student name: {studentName} -> {studentNameNoDiacritics}", LogLevel.INFO);
                
                // Get demo file path
                string demoFilePath = GetDemoFilePath(projectId);
                GMetrixLogger.Log($"Demo file path: {demoFilePath}", LogLevel.INFO);
                
                if (!File.Exists(demoFilePath))
                {
                    string errorMsg = $"Không tìm thấy file template: {demoFilePath}";
                    GMetrixLogger.Log(errorMsg, LogLevel.ERROR);
                    throw new FileNotFoundException(errorMsg);
                }
                
                // Create student folder in P{projectId}SWork
                string basePath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.WinApp\bin\Debug\DATMOSTemplates\Word2019";
                string studentFolderName = $"{studentId}-{studentNameNoDiacritics}";
                string studentWorkFolder = $"P{projectId}SWork"; // P1SWork for Project 1, P2SWork for Project 2, etc.
                string studentFolderPath = Path.Combine(basePath, $"Project{projectId}", studentWorkFolder, studentFolderName);
                
                GMetrixLogger.Log($"Creating folder: {studentFolderPath}", LogLevel.INFO);
                Directory.CreateDirectory(studentFolderPath);
                GMetrixLogger.Log($"Folder created successfully", LogLevel.INFO);
                
                // Create file name with timestamp (no spaces, lowercase)
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{studentId}-{studentNameNoDiacritics}-project{projectId}-{timestamp}.docx".ToLower();
                string studentFilePath = Path.Combine(studentFolderPath, fileName);
                
                GMetrixLogger.Log($"Student file path: {studentFilePath}", LogLevel.INFO);
                
                // Copy template file to student folder
                GMetrixLogger.Log($"Copying template from {demoFilePath} to {studentFilePath}", LogLevel.INFO);
                File.Copy(demoFilePath, studentFilePath, true);
                GMetrixLogger.Log($"File copied successfully", LogLevel.INFO);
                
                GMetrixLogger.Log($"Created student work file: {studentFilePath}", LogLevel.INFO);
                
                // Store student file path for later use when submitting
                _studentFilePaths[projectId] = studentFilePath;
                GMetrixLogger.Log($"Stored student file path for project {projectId}: {studentFilePath}", LogLevel.INFO);
                
                return studentFilePath;
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("CreateStudentWorkFile", ex);
                throw;
            }
        }
        
        // Get StudentWork directory path
        private string GetStudentWorkDirectory(string projectId)
        {
            string originalPath = GetProjectFilePath(projectId);
            string studentWorkDir = Path.Combine(
                Path.GetDirectoryName(originalPath), 
                "StudentWork"
            );
            
            Directory.CreateDirectory(studentWorkDir);
            return studentWorkDir;
        }
        
        // Find latest saved Word file in StudentWork directory
        private string FindLatestSavedWordFile(string studentWorkDir)
        {
            try
            {
                if (!Directory.Exists(studentWorkDir))
                    return null;
                
                var wordFiles = Directory.GetFiles(studentWorkDir, "*.docx", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                    .FirstOrDefault();
                
                return wordFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding latest saved Word file: {ex.Message}");
                return null;
            }
        }
        
        // Auto save and close Word document - IMPROVED VERSION WITH FILE LOCK HANDLING
        public string AutoSaveAndCloseWordDocument(string projectId)
        {
            try
            {
                GMetrixLogger.Log($"Bắt đầu AutoSaveAndCloseWordDocument cho Project {projectId}", LogLevel.INFO);
                
                // 1. Check if we have stored student file path
                if (_studentFilePaths.ContainsKey(projectId))
                {
                    string studentFilePath = _studentFilePaths[projectId];
                    if (File.Exists(studentFilePath))
                    {
                        GMetrixLogger.Log($"Sử dụng file student work đã tạo: {studentFilePath}", LogLevel.INFO);
                        
                        // Close Word window if open
                        IntPtr wordWindow = WindowsApiHelper.GetWordWindowHandle();
                        if (wordWindow != IntPtr.Zero)
                        {
                            GMetrixLogger.Log($"Đang đóng cửa sổ Word...", LogLevel.INFO);
                            bool closed = WindowsApiHelper.CloseWordWindow(wordWindow);
                            if (closed)
                            {
                                GMetrixLogger.Log($"Đã đóng cửa sổ Word thành công", LogLevel.INFO);
                            }
                            else
                            {
                                GMetrixLogger.Log($"Không thể đóng cửa sổ Word tự động", LogLevel.WARNING);
                            }
                            
                            // Wait longer for Word to fully close and release file lock
                            Thread.Sleep(2000);
                            
                            // Kill any remaining Word processes to ensure file is unlocked
                            KillWordProcesses();
                        }
                        
                        // Wait a bit more to ensure file is unlocked
                        Thread.Sleep(1000);
                        
                        return studentFilePath;
                    }
                }
                
                GMetrixLogger.Log($"Không tìm thấy file student work đã lưu cho Project {projectId}", LogLevel.WARNING);
                
                // 2. Try to find Word window
                IntPtr wordWindow2 = WindowsApiHelper.GetWordWindowHandle();
                if (wordWindow2 == IntPtr.Zero)
                {
                    GMetrixLogger.Log($"Không tìm thấy cửa sổ Word đang mở cho Project {projectId}", LogLevel.WARNING);
                    return null;
                }

                GMetrixLogger.Log($"Tìm thấy cửa sổ Word, đang thử lưu file...", LogLevel.INFO);
                
                // 3. Try to save Word document
                bool saved = WindowsApiHelper.SaveWordDocument(wordWindow2);
                if (!saved)
                {
                    GMetrixLogger.Log($"Không thể lưu file Word tự động", LogLevel.WARNING);
                    return null;
                }

                Thread.Sleep(2000); // Wait longer for save to complete
                
                // 4. Find the saved file
                string studentWorkDir = GetStudentWorkDirectory(projectId);
                string savedFilePath = FindLatestSavedWordFile(studentWorkDir);
                if (string.IsNullOrEmpty(savedFilePath))
                {
                    GMetrixLogger.Log($"Không tìm thấy file đã lưu trong {studentWorkDir}", LogLevel.WARNING);
                    return null;
                }

                GMetrixLogger.Log($"Đã tìm thấy file đã lưu: {savedFilePath}", LogLevel.INFO);
                
                // 5. Close Word window
                bool closed2 = WindowsApiHelper.CloseWordWindow(wordWindow2);
                if (closed2)
                {
                    GMetrixLogger.Log($"Đã đóng cửa sổ Word thành công", LogLevel.INFO);
                }
                else
                {
                    GMetrixLogger.Log($"Không thể đóng cửa sổ Word tự động", LogLevel.WARNING);
                }

                // Wait for Word to fully close and release file lock
                Thread.Sleep(2000);
                
                // Kill any remaining Word processes to ensure file is unlocked
                KillWordProcesses();
                
                // Wait a bit more to ensure file is unlocked
                Thread.Sleep(1000);
                
                return savedFilePath;
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("AutoSaveAndCloseWordDocument", ex);
                return null;
            }
        }
        
        public bool HasStudentFilePath(string projectId)
        {
            return _studentFilePaths.ContainsKey(projectId) && File.Exists(_studentFilePaths[projectId]);
        }
        
        public string GetStudentFilePath(string projectId)
        {
            return _studentFilePaths.ContainsKey(projectId) ? _studentFilePaths[projectId] : null;
        }
    }
}
