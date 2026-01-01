using DATMOS.WinApp.Services;
using DATMOS.WinApp.ViewModels;
using System;
using System.Windows.Forms;

namespace DATMOS.WinApp.Controllers
{
    public class Word2019Controller
    {
        private readonly WordDocumentService _wordDocumentService;
        private readonly GradingService _gradingService;
        private readonly MainViewModel _viewModel;
        
        public Word2019Controller()
        {
            _wordDocumentService = new WordDocumentService();
            _gradingService = new GradingService(_wordDocumentService);
            _viewModel = new MainViewModel(_wordDocumentService, _gradingService);
        }
        
        public Word2019Controller(WordDocumentService wordDocumentService, GradingService gradingService, MainViewModel viewModel)
        {
            _wordDocumentService = wordDocumentService;
            _gradingService = gradingService;
            _viewModel = viewModel;
        }
        
        public void OpenWordDocument(string projectId)
        {
            try
            {
                _viewModel.SelectedProjectId = projectId;
                _viewModel.OpenWordDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        
        public void SubmitDocument(string projectId)
        {
            try
            {
                _viewModel.SelectedProjectId = projectId;
                _viewModel.SubmitDocument();
                
                // Show result if available
                if (_viewModel.GradingResult != null)
                {
                    ShowGradingResult(_viewModel.GradingResult, projectId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý nộp bài: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        
        public void KillWordProcesses()
        {
            try
            {
                _viewModel.KillWordProcesses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đóng tiến trình Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        
        public string GetStudentFilePath(string projectId)
        {
            return _viewModel.StudentFilePath;
        }
        
        public string GetStatusMessage()
        {
            return _viewModel.StatusMessage;
        }
        
        public bool IsBusy()
        {
            return _viewModel.IsBusy;
        }
        
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
                
                MessageBox.Show(sb.ToString(), "Kết quả chấm điểm", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Điểm: {result.TotalScore}/{result.MaxScore} - {(result.Passed ? "ĐẬU" : "RỚT")}", 
                    "Kết quả chấm điểm", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
