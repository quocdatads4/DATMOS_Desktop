using DATMOS.Web;
using DATMOS.WinApp.Views;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATMOS.WinApp
{
    internal static class Program
    {
        private static IHost? _webHost;
        private static CancellationTokenSource? _cancellationTokenSource;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Tạo và chạy GMetrixMainForm làm form mặc định
            var mainForm = new GMetrixMainForm();
            
            // Khởi tạo cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();

            // Chạy WinForms application
            Application.Run(mainForm);

            // Khi ứng dụng đóng, dừng web server (nếu đang chạy)
            _cancellationTokenSource.Cancel();
            
            // Đợi web server dừng hoàn toàn
            if (_webHost != null)
            {
                Task.Run(() => _webHost?.StopAsync().Wait(5000)).Wait(5000);
            }
        }

        private static Form? ShowApplicationMenu()
        {
            using (var dialog = new Form
            {
                Text = "DATMOS Desktop - Chọn ứng dụng",
                Size = new System.Drawing.Size(400, 250),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var label = new Label
                {
                    Text = "Chọn ứng dụng để khởi động:",
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(350, 30),
                    Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular)
                };

                var mainFormButton = new Button
                {
                    Text = "DATMOS Web Application (Main)",
                    Location = new System.Drawing.Point(20, 60),
                    Size = new System.Drawing.Size(350, 40),
                    Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                    BackColor = System.Drawing.Color.FromArgb(30, 84, 159),
                    ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.Yes
                };

                var gmetrixButton = new Button
                {
                    Text = "GMetrix Practice Application",
                    Location = new System.Drawing.Point(20, 110),
                    Size = new System.Drawing.Size(350, 40),
                    Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                    BackColor = System.Drawing.Color.FromArgb(0, 160, 144),
                    ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.No
                };

                var cancelButton = new Button
                {
                    Text = "Thoát",
                    Location = new System.Drawing.Point(20, 160),
                    Size = new System.Drawing.Size(350, 30),
                    Font = new System.Drawing.Font("Segoe UI", 9F),
                    DialogResult = DialogResult.Cancel
                };

                dialog.Controls.Add(label);
                dialog.Controls.Add(mainFormButton);
                dialog.Controls.Add(gmetrixButton);
                dialog.Controls.Add(cancelButton);

                dialog.AcceptButton = mainFormButton;
                dialog.CancelButton = cancelButton;

                var result = dialog.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    return new MainForm();
                }
                else if (result == DialogResult.No)
                {
                    return new GMetrixMainForm();
                }
                else
                {
                    return null;
                }
            }
        }

        private static void StartWebServer(CancellationToken cancellationToken)
        {
            try
            {
                // Tạo và chạy web host trên port 5000
                _webHost = WebEntryPoint.CreateHostBuilder(new string[] { }, 5000);
                _webHost.Start();

                Console.WriteLine("Web server started on http://localhost:5243");

                // Chờ cho đến khi nhận được cancellation token
                cancellationToken.WaitHandle.WaitOne();

                // Dừng web host
                _webHost?.StopAsync().Wait(5000);
                _webHost?.Dispose();
                
                Console.WriteLine("Web server stopped");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start web server: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
