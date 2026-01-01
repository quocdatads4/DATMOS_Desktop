using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATMOS.Core.Entities
{
    public class Menu
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Text { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(50)]
        public string Area { get; set; } = string.Empty;

        [StringLength(50)]
        public string Controller { get; set; } = string.Empty;

        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        [StringLength(200)]
        public string Url { get; set; } = string.Empty;

        public int Order { get; set; } = 0;
        
        public bool IsVisible { get; set; } = true;

        // Phân loại menu: Admin, Customer, Teacher, Landing
        [Required]
        [StringLength(20)]
        public string MenuType { get; set; } = "Customer";

        // Self-referential relationship for parent-child
        [StringLength(50)]
        public string? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Menu? Parent { get; set; }

        public ICollection<Menu> Children { get; set; } = new List<Menu>();
    }
}
