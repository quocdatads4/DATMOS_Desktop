using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class BadgeViewModel
    {
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public string? ColorClass { get; set; }
    }

    public class CourseViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? ColorClass { get; set; }
        public string? Level { get; set; }
        public string? Duration { get; set; }
        public int TotalLessons { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public string? Instructor { get; set; }
        public double Rating { get; set; }
        public int EnrolledStudents { get; set; }
        public BadgeViewModel? Badge { get; set; }
    }

    public class CourseDetailsViewModel
    {
        public CourseViewModel? Course { get; set; }
        public List<string> Syllabus { get; set; } = new List<string>();
        public InstructorViewModel? Instructor { get; set; }
        public List<CourseViewModel> RelatedCourses { get; set; } = new List<CourseViewModel>();
        public CourseStatistics? Statistics { get; set; }
    }

    public class InstructorViewModel
    {
        public string? Name { get; set; }
        public double Rating { get; set; }
        public int TotalCourses { get; set; }
    }

    public class CourseStatistics
    {
        public int TotalCourses { get; set; }
        public int FreeCourses { get; set; }
        public int PaidCourses { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public double CompletionRate { get; set; }
        public double SatisfactionRate { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
