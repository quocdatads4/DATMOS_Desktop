using System.Collections.Generic;
using System.Linq;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    public class ExamHierarchyViewModel
    {
        public ExamSubjectViewModel Subject { get; set; }
        public List<ExamListViewModel> ExamLists { get; set; } = new();
        public Dictionary<int, List<ExamProjectViewModel>> ProjectsByList { get; set; } = new();
        public Dictionary<int, List<ExamTaskViewModel>> TasksByProject { get; set; } = new();
        public UserProgressViewModel UserProgress { get; set; }
        
        // Helper methods
        public int GetTotalTasks()
        {
            return TasksByProject.Values.Sum(tasks => tasks.Count);
        }
        
        public double GetCompletionPercentage()
        {
            if (UserProgress == null) return 0;
            var totalTasks = GetTotalTasks();
            return totalTasks > 0 ? (UserProgress.CompletedTasks * 100.0 / totalTasks) : 0;
        }
    }
}
