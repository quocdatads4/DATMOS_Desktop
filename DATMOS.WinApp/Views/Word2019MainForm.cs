using Microsoft.Web.WebView2.WinForms;
using DATMOS.WinApp.ViewModels;
using DATMOS.WinApp.Services;
using DATMOS.WinApp.Utilities;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATMOS.WinApp.Views
{
    public partial class Word2019MainForm : Form
    {
        private WebView2 webView;
        private Panel loadingPanel;
        private Label loadingLabel;
        private Panel resultPanel;
        private Label resultLabel;
        private Button closeResultButton;
        
        private readonly MainViewModel _viewModel;
        
        public Word2019MainForm()
        {
            InitializeComponent();
            PositionFormAtBottom();
            InitializeWebView();
            Load += Word2019MainForm_Load;
            
            // Initialize ViewModel with services
            var wordDocumentService = new WordDocumentService();
            var gradingService = new GradingService(wordDocumentService);
            _viewModel = new MainViewModel(wordDocumentService, gradingService);
            
            // Log form initialization
            GMetrixLogger.Log("Word2019MainForm khởi tạo thành công", LogLevel.INFO);
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

        private async void Word2019MainForm_Load(object? sender, EventArgs e)
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
                            HandleOpenProject(projectId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in HandleOpenProject: {ex.Message}");
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
                            HandleOpenProject("2");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in HandleOpenProject (legacy): {ex.Message}");
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

        // Handle open project using ViewModel
        private void HandleOpenProject(string projectId)
        {
            try
            {
                _viewModel.SelectedProjectId = projectId;
                _viewModel.OpenWordDocument();
                
                // Send feedback to web with project ID
                SendMessageToWeb($"WORD_OPENED_SUCCESS_{projectId}");
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("HandleOpenProject", ex);
                MessageBox.Show($"Lỗi khi mở file Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SendMessageToWeb($"WORD_OPEN_ERROR_{projectId}");
            }
        }

        // Handle submit project using ViewModel
        private void HandleSubmitProject(string projectId)
        {
            try
            {
                _viewModel.SelectedProjectId = projectId;
                _viewModel.SubmitDocument();
                
                // Show grading result
                if (_viewModel.GradingResult != null)
                {
                    ShowGradingResult(_viewModel.GradingResult, projectId);
                }
            }
            catch (Exception ex)
            {
                GMetrixLogger.LogError("HandleSubmitProject", ex);
                MessageBox.Show($"Lỗi khi xử lý nộp bài: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SendMessageToWeb($"SUBMIT_ERROR_{projectId}");
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

        // Show grading result on Form (above WebView2) instead of MessageBox
        private void ShowGradingResult(DATMOS.WinApp.Grading.WordGrader.GradingResult result, string projectId)
        {
            try
            {
                // Build result text
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
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

        // Send grading details to web
        private async void SendGradingDetailsToWeb(string projectId)
        {
            try
            {
                // Find latest grading details
                string gradingDir = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DATMOS",
                    "Grading"
                );
                
                if (System.IO.Directory.Exists(gradingDir))
                {
                    var latestFile = System.IO.Directory.GetFiles(gradingDir, $"grading_details_{projectId}_*.json")
                        .OrderByDescending(f => f)
                        .FirstOrDefault();
                    
                    if (latestFile != null && System.IO.File.Exists(latestFile))
                    {
                        string json = System.IO.File.ReadAllText(latestFile);
                        
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
    }
}
