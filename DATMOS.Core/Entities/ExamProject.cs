using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATMOS.Core.Entities
{
    public class ExamProject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamListId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public int TotalTasks { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("ExamListId")]
        public ExamList? ExamList { get; set; }
    }
}
