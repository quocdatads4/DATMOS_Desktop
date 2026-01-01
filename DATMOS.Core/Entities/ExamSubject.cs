using System.ComponentModel.DataAnnotations;

namespace DATMOS.Core.Entities
{
    public class ExamSubject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShortName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(20)]
        public string ColorClass { get; set; } = string.Empty;

        [StringLength(50)]
        public string Duration { get; set; } = string.Empty;

        public int TotalLessons { get; set; }
        public int TotalExams { get; set; }

        // Badge Properties (Flattened for simplicity)
        [StringLength(50)]
        public string BadgeText { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string BadgeIcon { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string BadgeColorClass { get; set; } = string.Empty;
    }
}
