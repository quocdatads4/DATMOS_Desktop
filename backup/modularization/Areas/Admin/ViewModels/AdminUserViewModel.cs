using System;
using System.Collections.Generic;
using System.Linq;
using DATMOS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DATMOS.Web.Areas.Admin.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? JoinDate { get; set; }
        public string Status { get; set; } = "Active";
        public string AvatarUrl { get; set; } = string.Empty;
        
        // Helper properties for display
        public string DisplayName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        
        // Static method to convert from AddUsers
        public static AdminUserViewModel FromUser(AddUsers user, IList<string> roles)
        {
            if (user == null) return new AdminUserViewModel();
            
            return new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roles?.FirstOrDefault() ?? "User",
                IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.Now,
                JoinDate = user.LockoutEnd.HasValue ? null : DateTimeOffset.Now, // Using LockoutEnd as placeholder, should have CreatedDate property
                Status = (!user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.Now) ? "Active" : "Locked",
                AvatarUrl = string.Empty,
                DisplayName = user.UserName ?? user.Email ?? string.Empty,
                Initials = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Substring(0, 1).ToUpper() : 
                          !string.IsNullOrEmpty(user.Email) ? user.Email.Substring(0, 1).ToUpper() : "?"
            };
        }
    }
}
