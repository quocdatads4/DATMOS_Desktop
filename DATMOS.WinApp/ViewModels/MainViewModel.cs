using DATMOS.WinApp.Grading;
using DATMOS.WinApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DATMOS.WinApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly WordDocumentService _wordDocumentService;
        private readonly GradingService _gradingService;
        
        private string _selectedProjectId = "1";
        private string _statusMessage = "Sẵn sàng";
        private bool _isBusy = false;
        private WordGrader.GradingResult _gradingResult;
        private string _studentFilePath;
        
        public MainViewModel()
        {
            _wordDocumentService = new WordDocumentService();
            _gradingService = new GradingService(_wordDocumentService);
        }
        
        public MainViewModel(WordDocumentService wordDocumentService, GradingService gradingService)
        {
            _wordDocumentService = wordDocumentService;
            _gradingService = gradingService;
        }
        
        public string SelectedProjectId
        {
            get => _selectedProjectId;
            set
            {
                if (_selectedProjectId != value)
                {
                    _selectedProjectId = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public WordGrader.GradingResult GradingResult
        {
            get => _gradingResult;
            set
            {
                if (_gradingResult != value)
                {
                    _gradingResult = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public string StudentFilePath
        {
            get => _studentFilePath;
            set
            {
                if (_studentFilePath != value)
                {
                    _studentFilePath = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public List<string> ProjectIds => new List<string> { "1", "2", "3" };
        
        public void OpenWordDocument()
        {
            try
            {
                IsBusy = true;
                StatusMessage = $"Đang mở Project {SelectedProjectId}...";
                
                _wordDocumentService.OpenWordDocument(SelectedProjectId);
                
                StatusMessage = $"Đã mở Project {SelectedProjectId}. Hãy thực hiện bài làm trong Word.";
                StudentFilePath = _wordDocumentService.GetStudentFilePath(SelectedProjectId);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi mở file: {ex.Message}";
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public void SubmitDocument()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Đang lưu và đóng Word...";
                
                // Auto save and close Word document
                string savedFilePath = _wordDocumentService.AutoSaveAndCloseWordDocument(SelectedProjectId);
                
                if (string.IsNullOrEmpty(savedFilePath))
                {
                    StatusMessage = "Không tìm thấy file đã lưu. Vui lòng kiểm tra lại.";
                    return;
                }
                
                StudentFilePath = savedFilePath;
                StatusMessage = "Đang chấm điểm...";
                
                // Grade the document
                GradingResult = _gradingService.GradeDocument(savedFilePath, SelectedProjectId);
                
                // Save result to JSON
                _gradingService.SaveTrainingResultToJson(SelectedProjectId, GradingResult, savedFilePath);
                
                StatusMessage = $"Chấm điểm hoàn tất! Điểm: {GradingResult.TotalScore}/{GradingResult.MaxScore} - {(GradingResult.Passed ? "ĐẬU" : "RỚT")}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi chấm điểm: {ex.Message}";
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public void KillWordProcesses()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Đang đóng tất cả tiến trình Word...";
                
                _wordDocumentService.KillWordProcesses();
                
                StatusMessage = "Đã đóng tất cả tiến trình Word.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi đóng tiến trình Word: {ex.Message}";
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
