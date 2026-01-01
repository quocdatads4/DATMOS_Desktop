using System;

namespace DATMOS.Web.Areas.Admin.ViewModels
{
    public class AdminCourseViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; } = true;
        public string ImageUrl { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public int TotalLessons { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public bool IsFree { get; set; }
        public int SubjectId { get; set; }
    }

    public class AdminCourseCreateEditViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public string Category { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public int TotalLessons { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public bool IsFree { get; set; }
        public int SubjectId { get; set; }
        public int EnrolledStudents { get; set; }
        public double Rating { get; set; }
    }
}
