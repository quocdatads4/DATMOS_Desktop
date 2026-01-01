using System.ComponentModel.DataAnnotations;

namespace DATMOS.Core.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShortName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(50)]
        public string ColorClass { get; set; } = string.Empty;

        [StringLength(50)]
        public string Level { get; set; } = string.Empty;

        [StringLength(50)]
        public string Duration { get; set; } = string.Empty;

        public int TotalLessons { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }

        [StringLength(100)]
        public string Instructor { get; set; } = string.Empty;

        public double Rating { get; set; }
        public int EnrolledStudents { get; set; }

        // Giữ lại để tương thích với file JSON
        public int SubjectId { get; set; }
    }
}
