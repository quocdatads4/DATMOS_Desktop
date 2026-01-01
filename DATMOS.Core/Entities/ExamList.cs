using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATMOS.Core.Entities
{
    public class ExamList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // Practice, SkillReview

        [Required]
        [StringLength(20)]
        public string Mode { get; set; } = string.Empty; // Testing, Training

        public int TotalProjects { get; set; }

        public int TotalTasks { get; set; }

        public int TimeLimit { get; set; } // minutes

        public int PassingScore { get; set; }

        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; } = string.Empty; // Easy, Medium, Hard

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("SubjectId")]
        public ExamSubject? Subject { get; set; }

        // Navigation property for ExamProjects
        public ICollection<ExamProject>? ExamProjects { get; set; }
    }
}
