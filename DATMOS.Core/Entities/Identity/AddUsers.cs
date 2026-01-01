using Microsoft.AspNetCore.Identity;
using System;

namespace DATMOS.Core.Entities.Identity
{
    /// <summary>
    /// Custom user class for DATMOS application that extends IdentityUser
    /// </summary>
    public class AddUsers : IdentityUser
    {
        // Personal Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        
        // Contact Information
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        
        // Preferences
        public string? Language { get; set; } = "en";
        public string? Timezone { get; set; }
        public string? Currency { get; set; } = "usd";
        
        // Professional Information
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? SocialLinks { get; set; } // JSON string for social media links
        
        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastProfileUpdate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? UserRole { get; set; } // "Admin", "Customer", "Teacher"
        
        // Navigation properties can be added here if needed
        // Example: public virtual ICollection<Course> CreatedCourses { get; set; }
    }
}
