using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ResourceViewModel
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
    }

    public class LessonViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? ColorClass { get; set; }
        public int Order { get; set; }
        public string? Duration { get; set; }
        public string? VideoUrl { get; set; }
        public string? Thumbnail { get; set; }
        public string? ContentType { get; set; }
        public bool IsFree { get; set; }
        public string? Difficulty { get; set; }
        public List<string> Objectives { get; set; } = new List<string>();
        public List<ResourceViewModel> Resources { get; set; } = new List<ResourceViewModel>();
        public bool Completed { get; set; }
        public int Progress { get; set; }
    }

    public class LessonDetailsViewModel
    {
        public LessonViewModel? Lesson { get; set; }
        public CourseViewModel? Course { get; set; }
        public List<LessonViewModel> RelatedLessons { get; set; } = new List<LessonViewModel>();
        public LessonStatistics? Statistics { get; set; }
        public List<ResourceViewModel> AllResources { get; set; } = new List<ResourceViewModel>();
    }

    public class LessonStatistics
    {
        public int TotalLessons { get; set; }
        public int FreeLessons { get; set; }
        public int PaidLessons { get; set; }
        public string? TotalDuration { get; set; }
        public int TotalCompleted { get; set; }
        public double AverageProgress { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<string, int> LessonsByDifficulty { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> LessonsByCourse { get; set; } = new Dictionary<string, int>();
    }
}
