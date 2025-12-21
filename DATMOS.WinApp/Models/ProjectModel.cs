namespace DATMOS.WinApp.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        
        public double ProgressPercentage => TotalTasks > 0 ? (CompletedTasks * 100.0) / TotalTasks : 0;
        
        public string DisplayText => $"{Name} ({TotalTasks} tasks)";
    }
}
