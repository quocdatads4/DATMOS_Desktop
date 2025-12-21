using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using DATMOS.WinApp.Grading;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace DATMOS.WinApp.Views
{
    // Logger class for detailed logging
    public class GMetrixLogger
    {
        private static readonly object _lock = new object();
        private static string _logDirectory;
        private static string _logFilePath;
        
        static GMetrixLogger()
        {
            // Initialize log directory
            _logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DATMOS",
                "Logs"
            );
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(_logDirectory);
            
            // Set log file path with current date
            _logFilePath = Path.Combine(_logDirectory, $"GMetrix_{DateTime.Now:yyyyMMdd}.log");
        }
        
        public static void Log(string message, LogLevel level = LogLevel.INFO)
        {
            try
            {
                lock (_lock)
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
                    
                    // Write to file
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                    
                    // Also write to console for debugging
                    Console.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERROR] Failed to write log: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] Original message: {message}");
            }
        }
        
        public static void LogGradingStart(string projectId, string studentFilePath)
        {
            Log($"Bắt đầu chấm điểm Project {projectId}", LogLevel.INFO);
            Log($"File học viên: {studentFilePath}", LogLevel.INFO);
        }
        
        public static void LogGradingResult(string projectId, int totalScore, int maxScore, bool passed)
        {
            Log($"Kết quả chấm điểm Project {projectId}: {totalScore}/{maxScore} - {(passed ? "ĐẬU" : "RỚT")}", LogLevel.INFO);
        }
        
        public static void LogGradingDetails(string projectId, List<WordGrader.GradingItem> items)
        {
            Log($"Chi tiết chấm điểm Project {projectId}:", LogLevel.INFO);
            foreach (var item in items)
            {
                string status = item.IsCorrect ? "✓" : "✗";
                Log($"  {status} {item.Description}: {item.Score}/{item.MaxScore}", LogLevel.INFO);
                if (!string.IsNullOrEmpty(item.Feedback))
                    Log($"    Feedback: {item.Feedback}", LogLevel.INFO);
            }
        }
        
        public static void LogError(string context, Exception ex)
        {
            Log($"LỖI trong {context}: {ex.Message}", LogLevel.ERROR);
            Log($"StackTrace: {ex.StackTrace}", LogLevel.ERROR);
        }
        
        public static void LogWebMessage(string message)
        {
            Log($"Nhận message từ web: {message}", LogLevel.DEBUG);
        }
        
        public static void LogWordOperation(string operation, string projectId, bool success, string details = "")
        {
            string status = success ? "THÀNH CÔNG" : "THẤT BẠI";
            string logMessage = $"Thao tác Word - {operation} Project {projectId}: {status}";
            if (!string.IsNullOrEmpty(details))
                logMessage += $" | {details}";
            Log(logMessage, success ? LogLevel.INFO : LogLevel.WARNING);
        }
        
        public static string GetLogFilePath()
        {
            return _logFilePath;
        }
        
        public static List<string> GetRecentLogs(int maxLines = 100)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    var lines = File.ReadAllLines(_logFilePath);
                    return lines.Reverse().Take(maxLines).Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                LogError("GetRecentLogs", ex);
            }
            return new List<string>();
        }
    }
    
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
    
    public partial class GMetrixMainForm : Form
    {
        private WebView2 webView;
        private Panel loadingPanel;
        private Label loadingLabel;
        private Panel resultPanel;
        private Label resultLabel;
        private Button closeResultButton;
        
        // Windows API functions for window manipulation
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessageW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool IsWindowVisible(IntPtr hWnd);
        
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        // Windows Messages
        private const uint WM_CLOSE = 0x0010;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_CLOSE = 0xF060;
        
        // Virtual Key Codes
        private const int VK_CONTROL = 0x11;
        private const int VK_S = 0x53;
        private const int VK_RETURN = 0x0D;
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMAXIMIZED = 3;
        
        public GMetrixMainForm()
        {
            InitializeComponent();
            PositionFormAtBottom();
            InitializeWebView();
            Load += GMetrixMainForm_Load;
            
            // Log form initialization
            GMetrixLogger.Log("GMetrixMainForm khởi tạo thành công", LogLevel.INFO);
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "DATMOS GMetrix Practice";
            // FormBorderStyle sẽ được đặt trong PositionFormAtBottom()
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.MinimumSize = new Size(400, 100); // Kích thước tối thiểu
            
            // Loading Panel (shown while WebView2 initializes)
            loadingPanel = new Panel();
            loadingPanel.BackColor = Color.FromArgb(30, 30, 30);
            loadingPanel.Dock = DockStyle.Fill;
            loadingPanel.Visible = true;
            
            loadingLabel = new Label();
            loadingLabel.Text = "Initializing GMetrix Practice...";
            loadingLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Giảm từ 14 xuống 10 cho form nhỏ
            loadingLabel.ForeColor = Color.White;
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            loadingLabel.Dock = DockStyle.Fill;
            
            loadingPanel.Controls.Add(loadingLabel);
            
            // Result Panel (shown when grading is complete)
            resultPanel = new Panel();
            resultPanel.BackColor = Color.FromArgb(40, 40, 40);
            resultPanel.Dock = DockStyle.Top;
            resultPanel.Height = 180; // Fixed height for result display
            resultPanel.Visible = false;
            resultPanel.Padding = new Padding(10);
            
            // Close button for result panel
            closeResultButton = new Button();
            closeResultButton.Text = "✕";
            closeResultButton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            closeResultButton.ForeColor = Color.White;
            closeResultButton.BackColor = Color.FromArgb(80, 80, 80);
            closeResultButton.FlatStyle = FlatStyle.Flat;
            closeResultButton.FlatAppearance.BorderSize = 0;
            closeResultButton.Size = new Size(25, 25);
            closeResultButton.Location = new Point(resultPanel.Width - 35, 5);
            closeResultButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeResultButton.Click += (s, e) => {
                resultPanel.Visible = false;
                // Adjust form height when hiding result panel
                AdjustFormHeight();
            };
            
            // Result label
            resultLabel = new Label();
            resultLabel.Text = "";
            resultLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            resultLabel.ForeColor = Color.White;
            resultLabel.BackColor = Color.Transparent;
            resultLabel.Dock = DockStyle.Fill;
            resultLabel.Padding = new Padding(5);
            resultLabel.AutoSize = false;
            resultLabel.Size = new Size(resultPanel.Width - 20, resultPanel.Height - 40);
            
            // Add controls to result panel
            resultPanel.Controls.Add(resultLabel);
            resultPanel.Controls.Add(closeResultButton);
            
            // WebView2 - Will fill remaining space
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            webView.DefaultBackgroundColor = Color.White;
            webView.Visible = false; // Hidden until initialized
            
            // Add controls to form (order matters: resultPanel on top, then webView, then loadingPanel)
            this.Controls.Add(resultPanel);
            this.Controls.Add(webView);
            this.Controls.Add(loadingPanel);
        }

        private void PositionFormAtBottom()
        {
            try
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                
                // Thay đổi từ 25% xuống 20% chiều cao màn hình theo yêu cầu mới
                int formHeight = (int)(screen.Height * 0.20); // 20% of screen height
                int formWidth = screen.Width;
                
                this.Size = new Size(formWidth, formHeight);
                this.Location = new Point(0, screen.Height - formHeight);
                
                // Giảm padding từ 30px xuống 15px cho vùng kéo thả
                this.Padding = new Padding(0, 15, 0, 0);
                
                // Luôn hiển thị trên cùng để không bị các cửa sổ khác che
                this.TopMost = true;
                
                // Cho phép thay đổi kích thước form
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            catch (Exception ex)
            {
                // Fallback với kích thước phù hợp (20% của 800px = 160px)
                this.Size = new Size(800, 160);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }
        
        // Adjust form height when showing/hiding result panel
        private void AdjustFormHeight()
        {
            try
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                int baseHeight = (int)(screen.Height * 0.20); // 20% of screen height
                
                if (resultPanel.Visible)
                {
                    // Increase form height to accommodate result panel
                    this.Height = baseHeight + resultPanel.Height;
                }
                else
                {
                    // Restore original height
                    this.Height = baseHeight;
                }
                
                // Keep form at bottom of screen
                this.Location = new Point(0, screen.Height - this.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adjusting form height: {ex.Message}");
            }
        }
        
        private async void InitializeWebView()
        {
            try
            {
                loadingLabel.Text = "Initializing WebView2...";
                
                // Create WebView2 environment
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                    userDataFolder: System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "DATMOS",
                        "WebView2"
                    )
                );
                
                await webView.EnsureCoreWebView2Async(environment);

                // Configure WebView2 settings for better experience
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Settings.IsScriptEnabled = true;
                    webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                    webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = false; // Disable dev tools for production
                    webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
                    webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                    
                    // Handle navigation events
                    webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                    webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                    webView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
                    
                    // Register web message handler
                    webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                }
                
                loadingLabel.Text = "WebView2 initialized successfully!";
                await Task.Delay(500); // Brief pause to show success message
            }
            catch (Exception ex)
            {
                loadingLabel.Text = $"Failed to initialize WebView2: {ex.Message}";
                loadingLabel.ForeColor = Color.Red;
                
                MessageBox.Show($"WebView2 initialization failed: {ex.Message}\n\n" +
                    "Please ensure WebView2 Runtime is installed.\n" +
                    "Download from: https://developer.microsoft.com/en-us/microsoft-edge/webview2/", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoreWebView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            // Update loading message when navigation starts
            this.Invoke((MethodInvoker)delegate {
                loadingLabel.Text = $"Loading: {e.Uri}";
                loadingPanel.Visible = true;
                webView.Visible = false;
            });
        }

        private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Show WebView2 and hide loading panel when navigation completes
            this.Invoke((MethodInvoker)delegate {
                if (e.IsSuccess)
                {
                    loadingPanel.Visible = false;
                    webView.Visible = true;
                    
                    // Focus the WebView2
                    webView.Focus();
                }
                else
                {
                    loadingLabel.Text = $"Failed to load page. Error: {e.WebErrorStatus}";
                    loadingLabel.ForeColor = Color.Red;
                }
            });
        }

        private void CoreWebView2_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            // Update window title with current page
            this.Invoke((MethodInvoker)delegate {
                if (webView.CoreWebView2 != null && !string.IsNullOrEmpty(webView.CoreWebView2.DocumentTitle))
                {
                    this.Text = $"DATMOS GMetrix - {webView.CoreWebView2.DocumentTitle}";
                }
            });
        }

        private async void GMetrixMainForm_Load(object? sender, EventArgs e)
        {
            // Wait for ASP.NET website to fully start (10 seconds)
            loadingLabel.Text = "Đang chờ website khởi động (10 giây)...";
            await Task.Delay(10000); // 10 giây

            try
            {
                // Navigate to GMetrix Practice URL
                string practiceUrl = "http://localhost:5243/Customer/Product/Practice?productId=1&examId=1";
                loadingLabel.Text = $"Đang kết nối: {practiceUrl}";
                
                webView.Source = new Uri(practiceUrl);
            }
            catch (UriFormatException ex)
            {
                loadingLabel.Text = $"Định dạng URL không hợp lệ: {ex.Message}";
                loadingLabel.ForeColor = Color.Red;
                
                MessageBox.Show($"Định dạng URL không hợp lệ. Vui lòng kiểm tra URL thực hành.\n\nLỗi: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                loadingLabel.Text = $"Lỗi kết nối: {ex.Message}";
                loadingLabel.ForeColor = Color.Red;
                
                MessageBox.Show($"Không thể kết nối đến URL thực hành.\n\n" +
                    "Hãy đảm bảo máy chủ web đang chạy trên cổng 5243.\n" +
                    $"Lỗi: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Allow dragging the form by clicking on the top area
        private bool dragging = false;
        private Point dragStartPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Check if click is in the top draggable area (top 15 pixels - matching padding)
            if (e.Y <= 15 && e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragStartPoint = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (dragging)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - dragStartPoint.X;
                newLocation.Y += e.Y - dragStartPoint.Y;
                this.Location = newLocation;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;
        }

        // Handle keyboard shortcuts
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                // Refresh WebView2
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Reload();
                    return true;
                }
            }
            else if (keyData == (Keys.Control | Keys.F5))
            {
                // Hard refresh
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Reload();
                    return true;
                }
            }
            else if (keyData == Keys.Escape)
            {
                // Close form
                this.Close();
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up WebView2 resources
            if (webView != null)
            {
                try
                {
                    if (webView.CoreWebView2 != null)
                    {
                        webView.CoreWebView2.Stop();
                    }
                    webView.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing WebView2: {ex.Message}");
                }
            }
            
            base.OnFormClosing(e);
        }

        // Double-click on title bar to toggle between bottom and normal position
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            
            var mousePos = this.PointToClient(Cursor.Position);
            if (mousePos.Y <= 15) // Double-click in title area (matching padding)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    // Toggle between bottom position and center
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    if (this.Location.Y == screen.Height - this.Height)
                    {
                        // Currently at bottom, move to center
                        this.Location = new Point(
                            (screen.Width - this.Width) / 2,
                            (screen.Height - this.Height) / 2
                        );
                    }
                    else
                    {
                        // Not at bottom, move to bottom
                        this.Location = new Point(0, screen.Height - this.Height);
                    }
                }
            }
        }

        // Web message handler
        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string message = e.TryGetWebMessageAsString();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Received web message: {message}");
                
                // Kiểm tra message mở project
                if (message.StartsWith("OPEN_PROJECT_"))
                {
                    // Lấy project ID từ message (ví dụ: "OPEN_PROJECT_1" -> "1")
                    string projectId = message.Substring("OPEN_PROJECT_".Length);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Opening project: {projectId}");
                    
                    // Chạy trên UI thread
                    this.Invoke((MethodInvoker)delegate {
                        try
                        {
                            OpenWordDocument(projectId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in OpenWordDocument: {ex.Message}");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                            MessageBox.Show($"Lỗi mở file Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                // Hỗ trợ message cũ cho tương thích ngược
                else if (message == "OPEN_PROJECT_DNETWORKING")
                {
                    // Mặc định mở Project 2 (DNetworking)
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Opening project DNetworking (legacy)");
                    
                    this.Invoke((MethodInvoker)delegate {
                        try
                        {
                            OpenWordDocument("2");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in OpenWordDocument (legacy): {ex.Message}");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                            MessageBox.Show($"Lỗi mở file Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                // Kiểm tra message nộp bài
                else if (message.StartsWith("SUBMIT_PROJECT_"))
                {
                    // Lấy project ID từ message
                    string projectId = message.Substring("SUBMIT_PROJECT_".Length);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Submitting project: {projectId}");
                    
                    // Chạy trên UI thread
                    this.Invoke((MethodInvoker)delegate {
                        try
                        {
                            HandleSubmitProject(projectId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in HandleSubmitProject: {ex.Message}");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                            MessageBox.Show($"Lỗi xử lý nộp bài: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                // Kiểm tra message lấy chi tiết chấm điểm
                else if (message.StartsWith("GET_GRADING_DETAILS_"))
                {
                    // Lấy project ID từ message
                    string projectId = message.Substring("GET_GRADING_DETAILS_".Length);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Getting grading details for project: {projectId}");
                    
                    // Chạy trên UI thread
                    this.Invoke((MethodInvoker)delegate {
                        try
                        {
                            SendGradingDetailsToWeb(projectId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in SendGradingDetailsToWeb: {ex.Message}");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                        }
                    });
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Unknown web message: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CRITICAL ERROR in CoreWebView2_WebMessageReceived: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                
                // Try to show error message on UI thread
                try
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"Lỗi nghiêm trọng xử lý message từ web: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR showing error message: {innerEx.Message}");
                }
            }
        }

        // Open Word document method with project ID using Process.Start and Windows API
        private void OpenWordDocument(string projectId)
        {
            try
            {
                // Close any existing Word processes before opening new one
                CloseWordProcesses();
                
                string filePath = GetProjectFilePath(projectId);
                
                if (File.Exists(filePath))
                {
                    // Open file with default application
                    Process process = Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                    
                    // Wait a moment for Word to open
                    Thread.Sleep(1000);
                    
                    // Try to resize Word window to occupy 80% of screen
                    ResizeWordWindow();
                    
                    // Send feedback to web with project ID
                    SendMessageToWeb($"WORD_OPENED_SUCCESS_{projectId}");
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy file: {filePath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SendMessageToWeb($"WORD_FILE_NOT_FOUND_{projectId}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SendMessageToWeb($"WORD_OPEN_ERROR_{projectId}");
            }
        }

        // Try to resize Word window using Windows API
        private void ResizeWordWindow()
        {
            try
            {
                // Get screen dimensions
                var screen = Screen.PrimaryScreen.WorkingArea;
                int winformHeight = this.Height;
                int wordHeight = screen.Height - winformHeight; // 80% còn lại
                
                // Try to find Word window
                IntPtr wordWindow = FindWindow("OpusApp", null); // Word window class name
                
                if (wordWindow != IntPtr.Zero)
                {
                    // Set window position and size
                    SetWindowPos(wordWindow, IntPtr.Zero, 0, 0, screen.Width, wordHeight, 
                        SWP_NOZORDER | SWP_NOACTIVATE);
                    
                    // Show window normally
                    ShowWindow(wordWindow, SW_SHOWNORMAL);
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
                    
                    // Send notification to web
                    if (closedCount > 0)
                    {
                        SendMessageToWeb($"WORD_CLOSED_PREVIOUS_{closedCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CloseWordProcesses: {ex.Message}");
                // Continue opening new file even if closing fails
            }
        }

        // Get file path based on project ID
        private string GetProjectFilePath(string projectId)
        {
            string basePath = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.WinApp\bin\Debug\DATMOSTemplates\Word2019";
            
            switch (projectId)
            {
                case "1":
                    return Path.Combine(basePath, "Project1", "Bicycles.docx");
                case "2":
                    return Path.Combine(basePath, "Project2", "DNetworking.docx");
                case "3":
                    // Nếu có Project 3, thêm đường dẫn tương ứng
                    // Tạm thời trả về file Project 2
                    return Path.Combine(basePath, "Project2", "DNetworking.docx");
                default:
                    // Mặc định trả về Project 1
                    return Path.Combine(basePath, "Project1", "Bicycles.docx");
            }
        }

        // Send message back to web
        private async void SendMessageToWeb(string message)
        {
            if (webView.CoreWebView2 != null)
            {
                await webView.CoreWebView2.ExecuteScriptAsync($"window.dispatchEvent(new CustomEvent('word-open-result', {{ detail: '{message}' }}));");
            }
        }

        // Overload for backward compatibility (không có projectId)
        private void OpenWordDocument()
        {
            OpenWordDocument("2"); // Mặc định mở Project 2
        }

        // Original HandleSubmitProject method (renamed - will be called by new method)
        private void HandleSubmitProjectOriginal(string projectId)
        {
            try
            {
                // 1. Ask user to save Word file first
                MessageBox.Show("Vui lòng lưu file Word trước khi nộp bài.\n\n" +
                    "1. Nhấn Ctrl+S trong Word\n" +
                    "2. Đợi 3 giây\n" +
                    "3. Nhấn OK để tiếp tục chấm điểm", 
                    "Lưu file Word", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 2. Wait 3 seconds for user to save
                Thread.Sleep(3000);

                // 3. Create student work directory
                string originalPath = GetProjectFilePath(projectId);
                string studentWorkDir = Path.Combine(
                    Path.GetDirectoryName(originalPath), 
                    "StudentWork"
                );
                
                Directory.CreateDirectory(studentWorkDir);
                
                // 4. Try to find the latest Word file in Documents folder
                string studentFilePath = FindLatestWordFile(studentWorkDir, projectId);
                
                if (string.IsNullOrEmpty(studentFilePath) || !File.Exists(studentFilePath))
                {
                    // Fallback: Use the original file
                    studentFilePath = Path.Combine(
                        studentWorkDir,
                        $"Bicycles_Student_{DateTime.Now:yyyyMMdd_HHmmss}.docx"
                    );
                    File.Copy(originalPath, studentFilePath, true);
                }

                // 5. Grade the document
                var grader = new WordGrader();
                var result = grader.GradeDocument(studentFilePath, projectId);

                // 6. Show grading result
                ShowGradingResult(result, projectId);

                // 7. Send result to web - DISABLED per user request
                // SendGradingResultToWeb(result, projectId);

                // 8. Save grading details for later retrieval
                SaveGradingDetails(result, projectId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý nộp bài: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SendMessageToWeb($"SUBMIT_ERROR_{projectId}");
            }
        }

        // Find latest Word file in Documents folder
        private string FindLatestWordFile(string studentWorkDir, string projectId)
        {
            try
            {
                // Look in Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var wordFiles = Directory.GetFiles(documentsPath, "*.docx", SearchOption.AllDirectories)
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                    .FirstOrDefault();

                if (wordFiles != null && File.Exists(wordFiles))
                {
                    // Copy to student work directory
                    string destPath = Path.Combine(
                        studentWorkDir,
                        $"{Path.GetFileNameWithoutExtension(wordFiles)}_{DateTime.Now:yyyyMMdd_HHmmss}.docx"
                    );
                    File.Copy(wordFiles, destPath, true);
                    return destPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding latest Word file: {ex.Message}");
            }
            
            return null;
        }

        // Show grading result on Form (above WebView2) instead of MessageBox
        private void ShowGradingResult(WordGrader.GradingResult result, string projectId)
        {
            try
            {
                // Build result text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"=== KẾT QUẢ CHẤM ĐIỂM MOS WORD 2019 ===");
                sb.AppendLine($"Project: {projectId}");
                sb.AppendLine($"Điểm: {result.TotalScore}/{result.MaxScore}");
                sb.AppendLine($"Kết quả: {(result.Passed ? "✅ ĐẬU" : "❌ RỚT")}");
                sb.AppendLine();
                sb.AppendLine("=== CHI TIẾT CHẤM ĐIỂM ===");
                
                foreach (var item in result.Items)
                {
                    string status = item.IsCorrect ? "✓" : "✗";
                    sb.AppendLine($"{status} {item.Description}: {item.Score}/{item.MaxScore}");
                    if (!string.IsNullOrEmpty(item.Feedback))
                        sb.AppendLine($"   {item.Feedback}");
                }
                
                sb.AppendLine();
                sb.AppendLine("=== THÔNG BÁO ===");
                sb.AppendLine("Ứng dụng vẫn đang chạy. Bạn có thể:");
                sb.AppendLine("1. Làm bài khác");
                sb.AppendLine("2. Nộp lại bài này");
                sb.AppendLine("3. Thoát bằng phím ESC");
                sb.AppendLine("4. Nhấn nút ✕ để đóng kết quả");
                
                // Update result label
                resultLabel.Text = sb.ToString();
                
                // Show result panel
                resultPanel.Visible = true;
                
                // Adjust form height to accommodate result panel
                AdjustFormHeight();
                
                // Bring result panel to front
                resultPanel.BringToFront();
                
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Đã hiển thị kết quả chấm điểm trên form. Ứng dụng vẫn đang chạy.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] LỖI trong ShowGradingResult: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] StackTrace: {ex.StackTrace}");
                
                // Fallback to simple message on form
                try
                {
                    resultLabel.Text = $"Điểm: {result.TotalScore}/{result.MaxScore} - {(result.Passed ? "ĐẬU" : "RỚT")}\n\n" +
                        "Ứng dụng vẫn đang chạy. Bạn có thể làm bài khác.\n" +
                        "Nhấn nút ✕ để đóng kết quả.";
                    resultPanel.Visible = true;
                    AdjustFormHeight();
                    resultPanel.BringToFront();
                }
                catch (Exception innerEx)
                {
                    // Ultimate fallback: use MessageBox
                    MessageBox.Show($"Điểm: {result.TotalScore}/{result.MaxScore} - {(result.Passed ? "ĐẬU" : "RỚT")}\n\n" +
                        "Ứng dụng vẫn đang chạy. Bạn có thể làm bài khác.", 
                        "Kết quả chấm điểm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Send grading result to web
        private async void SendGradingResultToWeb(WordGrader.GradingResult result, string projectId)
        {
            if (webView.CoreWebView2 != null)
            {
                string message = $"GRADING_RESULT_{projectId}_{result.TotalScore}_{result.MaxScore}_{result.Passed}";
                await webView.CoreWebView2.ExecuteScriptAsync(
                    $"window.dispatchEvent(new CustomEvent('word-grading-result', {{ detail: '{message}' }}));");
            }
        }

        // Save grading details for later retrieval
        private void SaveGradingDetails(WordGrader.GradingResult result, string projectId)
        {
            try
            {
                string detailsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DATMOS",
                    "Grading",
                    $"grading_details_{projectId}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                );
                
                Directory.CreateDirectory(Path.GetDirectoryName(detailsPath));
                
                // Create simple JSON representation
                var details = new
                {
                    ProjectId = projectId,
                    TotalScore = result.TotalScore,
                    MaxScore = result.MaxScore,
                    Passed = result.Passed,
                    Items = result.Items.Select(i => new
                    {
                        Description = i.Description,
                        Score = i.Score,
                        MaxScore = i.MaxScore,
                        IsCorrect = i.IsCorrect,
                        Feedback = i.Feedback
                    }),
                    Timestamp = DateTime.Now
                };
                
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(details, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(detailsPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving grading details: {ex.Message}");
            }
        }

        // Send grading details to web
        private async void SendGradingDetailsToWeb(string projectId)
        {
            try
            {
                // Find latest grading details
                string gradingDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DATMOS",
                    "Grading"
                );
                
                if (Directory.Exists(gradingDir))
                {
                    var latestFile = Directory.GetFiles(gradingDir, $"grading_details_{projectId}_*.json")
                        .OrderByDescending(f => f)
                        .FirstOrDefault();
                    
                    if (latestFile != null && File.Exists(latestFile))
                    {
                        string json = File.ReadAllText(latestFile);
                        
                        // Send to web
                        if (webView.CoreWebView2 != null)
                        {
                            // Escape JSON for JavaScript
                            string escapedJson = json.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
                            await webView.CoreWebView2.ExecuteScriptAsync(
                                $"window.dispatchEvent(new CustomEvent('grading-details', {{ detail: '{escapedJson}' }}));");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending grading details: {ex.Message}");
            }
        }

        // ========== NEW METHODS FOR AUTO SAVE AND CLOSE WORD ==========

        // Get the main Word window handle
        private IntPtr GetWordWindowHandle()
        {
            return FindWindow("OpusApp", null); // Word window class name
        }

        // Get Word window title
        private string GetWordWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return string.Empty;
            
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return string.Empty;
            
            var builder = new System.Text.StringBuilder(length + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        // Save Word document using Ctrl+S
        private bool SaveWordDocument(IntPtr wordWindow)
        {
            try
            {
                if (wordWindow == IntPtr.Zero) return false;
                
                // Bring Word window to foreground
                SetForegroundWindow(wordWindow);
                Thread.Sleep(200);
                
                // Send Ctrl+S to save
                SendKeys.SendWait("^(s)");
                Thread.Sleep(500);
                
                // Check if Save As dialog appears (untitled document)
                IntPtr saveAsDialog = FindWindow("#32770", "Save As"); // Dialog class
                if (saveAsDialog != IntPtr.Zero)
                {
                    // Auto-save to StudentWork folder
                    string studentWorkDir = GetStudentWorkDirectory("1"); // Default to project 1
                    string defaultFileName = Path.Combine(studentWorkDir, $"Bicycles_Student_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
                    
                    // Type file path and press Enter
                    SendKeys.SendWait(defaultFileName);
                    Thread.Sleep(200);
                    SendKeys.SendWait("{ENTER}");
                    Thread.Sleep(1000);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Word document: {ex.Message}");
                return false;
            }
        }

        // Close Word window
        private bool CloseWordWindow(IntPtr wordWindow)
        {
            try
            {
                if (wordWindow == IntPtr.Zero) return false;
                
                // Send close message
                bool result = PostMessage(wordWindow, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(500);
                
                // If still open, try graceful close
                if (IsWindowVisible(wordWindow))
                {
                    // Try to close via system menu
                    result = PostMessage(wordWindow, WM_SYSCOMMAND, (IntPtr)SC_CLOSE, IntPtr.Zero);
                    Thread.Sleep(500);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing Word window: {ex.Message}");
                return false;
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

        // Auto save and close Word document
        private string AutoSaveAndCloseWordDocument(string projectId)
        {
            try
            {
                // 1. Find Word window
                IntPtr wordWindow = GetWordWindowHandle();
                if (wordWindow == IntPtr.Zero)
                {
                    MessageBox.Show("Không tìm thấy cửa sổ Word đang mở.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                // 2. Get StudentWork directory
                string studentWorkDir = GetStudentWorkDirectory(projectId);
                
                // 3. Save Word document
                MessageBox.Show("Đang tự động lưu file Word...", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                bool saved = SaveWordDocument(wordWindow);
                if (!saved)
                {
                    MessageBox.Show("Không thể lưu file Word tự động.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                Thread.Sleep(1000); // Wait for save to complete
                
                // 4. Find the saved file
                string savedFilePath = FindLatestSavedWordFile(studentWorkDir);
                if (string.IsNullOrEmpty(savedFilePath))
                {
                    // Fallback: Use default path
                    savedFilePath = Path.Combine(studentWorkDir, $"Bicycles_Student_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
                }

                // 5. Close Word window
                MessageBox.Show("Đang đóng file Word...", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                bool closed = CloseWordWindow(wordWindow);
                if (!closed)
                {
                    MessageBox.Show("Không thể đóng file Word tự động.", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Continue anyway
                }

                Thread.Sleep(500); // Wait for close to complete
                
                return savedFilePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tự động lưu và đóng file Word: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Updated HandleSubmitProject with auto save and close
        private void HandleSubmitProject(string projectId)
        {
            try
            {
                // Log start of grading process
                GMetrixLogger.Log($"Bắt đầu chấm điểm Project {projectId}", LogLevel.INFO);
                
                // 1. Auto save and close Word document
                string studentFilePath = AutoSaveAndCloseWordDocument(projectId);
                
                if (string.IsNullOrEmpty(studentFilePath) || !File.Exists(studentFilePath))
                {
                    // Fallback to original method
                    GMetrixLogger.Log($"Không tìm thấy file Word đã lưu, sử dụng phương pháp cũ cho Project {projectId}", LogLevel.WARNING);
                    MessageBox.Show("Không tìm thấy file Word đã lưu. Sử dụng phương pháp cũ...", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Use original method
                    HandleSubmitProjectOriginal(projectId);
                    return;
                }

                GMetrixLogger.Log($"File đã lưu: {studentFilePath}", LogLevel.INFO);
                
                // 2. Grade the document
                var grader = new WordGrader();
                GMetrixLogger.Log($"Đang chấm điểm Project {projectId}...", LogLevel.INFO);
                var result = grader.GradeDocument(studentFilePath, projectId);
                GMetrixLogger.LogGradingResult(projectId, result.TotalScore, result.MaxScore, result.Passed);
                GMetrixLogger.LogGradingDetails(projectId, result.Items);

                // 3. Show grading result
                ShowGradingResult(result, projectId);

                // 4. Send result to web - DISABLED per user request
                // SendGradingResultToWeb(result, projectId);

                // 5. Save grading details for later retrieval
                SaveGradingDetails(result, projectId);
                
                // 6. Send completion message to web - DISABLED per user request
                // SendCompletionMessageToWeb(projectId);
                
                GMetrixLogger.Log($"Quá trình chấm điểm Project {projectId} hoàn tất. Ứng dụng vẫn đang chạy.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("HandleSubmitProject", ex);
                
                MessageBox.Show($"Lỗi khi xử lý nộp bài: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SendMessageToWeb($"SUBMIT_ERROR_{projectId}");
            }
        }

        // Send completion message to web
        private async void SendCompletionMessageToWeb(string projectId)
        {
            try
            {
                if (webView.CoreWebView2 != null)
                {
                    await webView.CoreWebView2.ExecuteScriptAsync(
                        $"window.dispatchEvent(new CustomEvent('grading-complete', {{ detail: 'GRADING_COMPLETE_{projectId}' }}));");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Lỗi gửi completion message: {ex.Message}");
            }
        }

    }
}
