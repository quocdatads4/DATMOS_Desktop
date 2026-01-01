using System;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamProjectViewModel
    {
        public int Id { get; set; }
        public int ExamListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
