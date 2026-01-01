using DATMOS.Core.Entities.Identity;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DATMOS.Web.Interfaces
{
    public interface IProfileService
    {
        // User profile operations
        Task<AddUsers?> GetUserProfileAsync(string userId);
        Task<AddUsers?> GetUserProfileByEmailAsync(string email);
        Task<bool> UpdateUserProfileAsync(string userId, UserProfileViewModel model);
        Task<bool> UpdateUserProfilePartialAsync(string userId, object updates);
        
        // Security operations
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> VerifyPasswordAsync(string userId, string password);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        
        // Profile picture operations
        Task<string?> UploadProfilePictureAsync(string userId, IFormFile file);
        Task<bool> DeleteProfilePictureAsync(string userId);
        Task<string?> GetProfilePictureUrlAsync(string userId);
        
        // User preferences
        Task<bool> UpdateUserPreferencesAsync(string userId, string language, string timezone, string currency);
        Task<object> GetUserPreferencesAsync(string userId);
        
        // User activity tracking
        Task<bool> UpdateLastLoginAsync(string userId);
        Task<bool> UpdateLastProfileUpdateAsync(string userId);
        
        // User role management
        Task<string?> GetUserRoleAsync(string userId);
        Task<bool> UpdateUserRoleAsync(string userId, string role);
        
        // Validation and verification
        Task<bool> IsEmailAvailableAsync(string email, string? excludeUserId = null);
        Task<bool> IsUsernameAvailableAsync(string username, string? excludeUserId = null);
        
        // Bulk operations
        Task<int> DeactivateInactiveUsersAsync(int daysInactive);
        Task<int> CleanupOrphanedProfilesAsync();
        
        // Statistics
        Task<object> GetUserStatisticsAsync(string userId);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        
        // Cache management
        Task ClearUserCacheAsync(string userId);
        Task ClearAllProfilesCacheAsync();
    }
}
