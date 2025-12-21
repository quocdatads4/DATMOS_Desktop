using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATMOS.WinApp
{
    public partial class MainForm : Form
    {
        private WebView2? webView;
        private Button? refreshButton;
        private Label? statusLabel;
        private Panel controlPanel;
        private Button? settingsButton;

        public MainForm()
        {
            InitializeComponent();
            InitializeWebView();
            Load += MainForm_Load;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            webView = new WebView2();
            controlPanel = new Panel();
            refreshButton = new Button();
            settingsButton = new Button();
            statusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            controlPanel.SuspendLayout();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Dock = DockStyle.Fill;
            webView.Location = new Point(0, 50);
            webView.Name = "webView";
            webView.Size = new Size(1174, 679);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // controlPanel
            // 
            controlPanel.BackColor = Color.LightGray;
            controlPanel.Controls.Add(refreshButton);
            controlPanel.Controls.Add(settingsButton);
            controlPanel.Controls.Add(statusLabel);
            controlPanel.Dock = DockStyle.Top;
            controlPanel.Location = new Point(0, 0);
            controlPanel.Name = "controlPanel";
            controlPanel.Size = new Size(1174, 50);
            controlPanel.TabIndex = 1;
            // 
            // refreshButton
            // 
            refreshButton.Location = new Point(10, 10);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(80, 30);
            refreshButton.TabIndex = 0;
            refreshButton.Text = "Refresh";
            refreshButton.Click += RefreshButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.Location = new Point(100, 10);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(80, 30);
            settingsButton.TabIndex = 1;
            settingsButton.Text = "Settings";
            settingsButton.Click += SettingsButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.ForeColor = Color.DarkBlue;
            statusLabel.Location = new Point(200, 15);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(400, 20);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Initializing...";
            // 
            // MainForm
            // 
            ClientSize = new Size(1174, 729);
            Controls.Add(webView);
            Controls.Add(controlPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DATMOS Desktop Application";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            controlPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private async void InitializeWebView()
        {
            try
            {
                if (statusLabel != null)
                    statusLabel.Text = "Initializing WebView2...";
                
                // Khởi tạo WebView2 environment
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync();
                if (webView != null)
                    await webView.EnsureCoreWebView2Async(environment);

                // Cấu hình WebView2
                if (webView?.CoreWebView2?.Settings != null)
                {
                    webView.CoreWebView2.Settings.IsScriptEnabled = true;
                    webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                    webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                }

                // Xử lý navigation events
                if (webView != null)
                {
                    webView.NavigationStarting += (s, e) =>
                    {
                        if (statusLabel != null)
                            statusLabel.Text = $"Loading: {e.Uri}";
                    };

                    webView.NavigationCompleted += (s, e) =>
                    {
                        if (statusLabel != null)
                        {
                            if (e.IsSuccess)
                            {
                                statusLabel.Text = "Ready";
                            }
                            else
                            {
                                statusLabel.Text = $"Navigation failed: {e.WebErrorStatus}";
                            }
                        }
                    };
                }

                if (statusLabel != null)
                    statusLabel.Text = "WebView2 initialized successfully";
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                    statusLabel.Text = $"WebView2 init failed: {ex.Message}";
                MessageBox.Show($"Failed to initialize WebView2: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            // Đợi server khởi động
            if (statusLabel != null)
                statusLabel.Text = "Waiting for server to start...";
            await Task.Delay(3000);

            try
            {
                // Navigate đến localhost:5000
                if (webView != null)
                {
                    webView.Source = new Uri("http://localhost:5243");
                    if (statusLabel != null)
                        statusLabel.Text = "Navigating to http://localhost:5243";

                    // Thêm retry logic nếu navigation fails
                    webView.NavigationCompleted += async (s, args) =>
                    {
                        if (!args.IsSuccess && statusLabel != null)
                        {
                            statusLabel.Text = "Navigation failed, retrying in 2 seconds...";
                            await Task.Delay(2000);
                            webView.Reload();
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                    statusLabel.Text = $"Navigation error: {ex.Message}";
                MessageBox.Show($"Failed to navigate: {ex.Message}\n\nMake sure the web server is running on port 5000.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.Reload();
                if (statusLabel != null)
                    statusLabel.Text = "Refreshing...";
            }
        }

        private void SettingsButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("DATMOS Desktop Application Settings\n\n" +
                "• Web Server: http://localhost:5243\n" +
                "• Database: PostgreSQL (for web app)\n" +
                "• Technology: .NET 8, WinForms, WebView2\n" +
                "• Architecture: Hybrid Desktop with embedded ASP.NET Core",
                "Application Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dọn dẹp WebView2 resources
            if (webView != null)
            {
                webView.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
