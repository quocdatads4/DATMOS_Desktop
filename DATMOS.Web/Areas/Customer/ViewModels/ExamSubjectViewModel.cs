using System.ComponentModel.DataAnnotations;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamSubjectViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Mã môn")]
        public string Code { get; set; } = string.Empty;
        
        [Display(Name = "Tên môn")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Tên ngắn")]
        public string ShortName { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Biểu tượng")]
        public string Icon { get; set; } = string.Empty;
        
        [Display(Name = "Màu sắc")]
        public string ColorClass { get; set; } = string.Empty;
        
        [Display(Name = "Thời lượng")]
        public string Duration { get; set; } = string.Empty;
        
        [Display(Name = "Số bài học")]
        public int TotalLessons { get; set; }
        
        [Display(Name = "Số bài thi")]
        public int TotalExams { get; set; }
        
        [Display(Name = "Số danh sách thi")]
        public int TotalLists { get; set; }
        
        [Display(Name = "Tiến độ")]
        public double ProgressPercentage { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chưa bắt đầu";
    }
}