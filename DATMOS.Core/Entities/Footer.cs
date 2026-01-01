using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATMOS.Core.Entities
{
    public class Footer
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        [StringLength(200)]
        public string Url { get; set; } = string.Empty;

        // Phân loại section: Brand, Links, Support, Contact, Copyright, Social
        [Required]
        [StringLength(50)]
        public string Section { get; set; } = "Links";

        public int Order { get; set; } = 0;
        
        public bool IsVisible { get; set; } = true;

        // Phân loại footer: Admin, Customer, Teacher, Landing
        [Required]
        [StringLength(20)]
        public string FooterType { get; set; } = "Customer";

        // Phân cấp (cho nested items trong section)
        [StringLength(50)]
        public string? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Footer? Parent { get; set; }

        public ICollection<Footer> Children { get; set; } = new List<Footer>();

        // Additional fields for specific types
        [StringLength(50)]
        public string? Area { get; set; }

        [StringLength(50)]
        public string? Controller { get; set; }

        [StringLength(50)]
        public string? Action { get; set; }

        [StringLength(50)]
        public string? Target { get; set; } // _blank, _self, etc.

        // CSS class for styling
        [StringLength(100)]
        public string CssClass { get; set; } = string.Empty;
    }
}
