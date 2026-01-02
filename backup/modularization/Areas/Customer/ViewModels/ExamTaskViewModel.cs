using System.ComponentModel.DataAnnotations;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamTaskViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tên nhiệm vụ")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Hướng dẫn")]
        public string Instructions { get; set; } = string.Empty;

        [Display(Name = "Thứ tự")]
        public int OrderIndex { get; set; }

        [Display(Name = "Điểm tối đa")]
        public double MaxScore { get; set; }

        [Display(Name = "Loại nhiệm vụ")]
        public string TaskType { get; set; } = string.Empty;

        [Display(Name = "Màu loại nhiệm vụ")]
        public string TaskTypeColor { get; set; } = "primary";

        [Display(Name = "Hoàn thành")]
        public bool IsCompleted { get; set; }
    }
}
