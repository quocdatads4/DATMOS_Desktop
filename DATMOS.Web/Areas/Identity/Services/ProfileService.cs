using DATMOS.Core.Entities.Identity;
using DATMOS.Web.Interfaces;
using DATMOS.Data;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AddUsers> _userManager;
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProfileService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public ProfileService(
            UserManager<AddUsers> userManager,
            AppDbContext context,
            IMemoryCache cache,
            IWebHostEnvironment environment,
            ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _context = context;
            _cache = cache;
            _environment = environment;
            _logger = logger;
        }

        public async Task<AddUsers?> GetUserProfileAsync(string userId)
        {
            var cacheKey = $"UserProfile_{userId}";
            
            if (!_cache.TryGetValue(cacheKey, out AddUsers? user))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user != null)
                {
                    _cache.Set(cacheKey, user, _cacheDuration);
                }
            }

            return user;
        }

        public async Task<AddUsers?> GetUserProfileByEmailAsync(string email)
        {
            var cacheKey = $"UserProfile_Email_{email}";
            
            if (!_cache.TryGetValue(cacheKey, out AddUsers? user))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                
                if (user != null)
                {
                    _cache.Set(cacheKey, user, _cacheDuration);
                }
            }

            return user;
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UserProfileViewModel model)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                // Update basic information
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DisplayName = model.DisplayName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.City = model.City;
                user.State = model.State;
                user.ZipCode = model.ZipCode;
                user.Country = model.Country;
                user.Language = model.Language;
                user.Timezone = model.Timezone;
                user.Currency = model.Currency;
                
                // Update additional fields
                user.Organization = model.Organization;
                user.JobTitle = model.JobTitle;
                user.Department = model.Department;
                user.Bio = model.Bio;
                user.Website = model.Website;
                user.SocialLinks = model.SocialLinks;
                
                // Update profile picture URL if provided
                if (!string.IsNullOrEmpty(model.ProfilePictureUrl))
                {
                    user.ProfilePictureUrl = model.ProfilePictureUrl;
                }
                
                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("User profile updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when updating user profile for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving user profile to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserProfilePartialAsync(string userId, object updates)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                // This is a simplified implementation
                // In a real scenario, you would use reflection or a more sophisticated approach
                user.LastProfileUpdate = DateTime.UtcNow;
                
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    ClearUserCache(userId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile partially for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                    return true;
                }

                _logger.LogError("Failed to change password for user {UserId}: {Errors}", 
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> VerifyPasswordAsync(string userId, string password)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                return await _userManager.CheckPasswordAsync(user, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await GetUserProfileByEmailAsync(email);
                if (user == null)
                    return false;

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                
                if (result.Succeeded)
                {
                    ClearUserCache(user.Id);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email {Email}", email);
                return false;
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await GetUserProfileByEmailAsync(email);
                if (user == null)
                    return string.Empty;

                return await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating password reset token for email {Email}", email);
                return string.Empty;
            }
        }

        public async Task<string?> UploadProfilePictureAsync(string userId, IFormFile file)
        {
            try
            {
                _logger.LogInformation("Starting profile picture upload for user {UserId}. File: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}", 
                    userId, file.FileName, file.Length, file.ContentType);

                // Validate file using FileValidationHelper
                if (!DATMOS.Web.Utilities.FileValidationHelper.ValidateProfilePicture(file, out string errorMessage))
                {
                    _logger.LogWarning("Profile picture validation failed for user {UserId}: {ErrorMessage}", userId, errorMessage);
                    return null;
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                _logger.LogDebug("Upload folder path: {UploadsFolder}", uploadsFolder);
                
                if (!Directory.Exists(uploadsFolder))
                {
                    _logger.LogInformation("Creating upload directory: {UploadsFolder}", uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate safe filename
                var fileName = DATMOS.Web.Utilities.FileValidationHelper.GenerateProfilePictureFileName(userId, file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                _logger.LogDebug("Generated filename: {FileName}, Full path: {FilePath}", fileName, filePath);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("File saved successfully: {FilePath}", filePath);

                var user = await GetUserProfileAsync(userId);
                if (user == null)
                {
                    _logger.LogError("User not found for ID: {UserId}", userId);
                    return null;
                }

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    _logger.LogDebug("Deleting old profile picture: {OldProfilePictureUrl}", user.ProfilePictureUrl);
                    DATMOS.Web.Utilities.FileValidationHelper.DeleteProfilePictureFile(_environment.WebRootPath, user.ProfilePictureUrl);
                }

                var relativePath = DATMOS.Web.Utilities.FileValidationHelper.GetProfilePictureRelativePath(fileName);
                _logger.LogDebug("Generated relative path: {RelativePath}", relativePath);
                
                user.ProfilePictureUrl = relativePath;
                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("Profile picture uploaded and saved to database successfully for user {UserId}. URL: {ProfilePictureUrl}, Rows affected: {RowsAffected}", 
                            userId, relativePath, rowsAffected);
                        return relativePath;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when saving profile picture for user {UserId}", userId);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving profile picture to database for user {UserId}", userId);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(string userId)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.ProfilePictureUrl))
                    return false;

                // Delete file using FileValidationHelper
                DATMOS.Web.Utilities.FileValidationHelper.DeleteProfilePictureFile(_environment.WebRootPath, user.ProfilePictureUrl);

                user.ProfilePictureUrl = null;
                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("Profile picture deleted successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when deleting profile picture for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving profile picture deletion to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture for user {UserId}", userId);
                return false;
            }
        }

        public async Task<string?> GetProfilePictureUrlAsync(string userId)
        {
            var user = await GetUserProfileAsync(userId);
            return user?.ProfilePictureUrl;
        }

        public async Task<bool> UpdateUserPreferencesAsync(string userId, string language, string timezone, string currency)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                user.Language = language;
                user.Timezone = timezone;
                user.Currency = currency;
                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("User preferences updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when updating user preferences for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving user preferences to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user preferences for user {UserId}", userId);
                return false;
            }
        }

        public async Task<object> GetUserPreferencesAsync(string userId)
        {
            var user = await GetUserProfileAsync(userId);
            if (user == null)
                return new { };

            return new
            {
                Language = user.Language,
                Timezone = user.Timezone,
                Currency = user.Currency
            };
        }

        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                user.LastLoginAt = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("Last login updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when updating last login for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving last login to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateLastProfileUpdateAsync(string userId)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("Last profile update timestamp updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when updating last profile update for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving last profile update to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last profile update for user {UserId}", userId);
                return false;
            }
        }

        public async Task<string?> GetUserRoleAsync(string userId)
        {
            var user = await GetUserProfileAsync(userId);
            return user?.UserRole;
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            try
            {
                var user = await GetUserProfileAsync(userId);
                if (user == null)
                    return false;

                user.UserRole = role;
                user.LastProfileUpdate = DateTime.UtcNow;

                // Use DbContext to save changes for custom properties
                // UserManager.UpdateAsync may not detect changes to custom properties
                try
                {
                    // Check if the entity is already being tracked
                    var trackedUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
                    if (trackedUser != null)
                    {
                        // If already tracked, update the tracked entity
                        _context.Entry(trackedUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // If not tracked, attach and mark as modified
                        _context.Attach(user);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    
                    // Save changes to database
                    var rowsAffected = await _context.SaveChangesAsync();
                    
                    if (rowsAffected > 0)
                    {
                        ClearUserCache(userId);
                        _logger.LogInformation("User role updated successfully for user {UserId}. Rows affected: {RowsAffected}", 
                            userId, rowsAffected);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No rows affected when updating user role for user {UserId}", userId);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving user role to database for user {UserId}", userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email, string? excludeUserId = null)
        {
            var query = _userManager.Users.Where(u => u.Email == email);
            
            if (!string.IsNullOrEmpty(excludeUserId))
                query = query.Where(u => u.Id != excludeUserId);

            var user = await query.FirstOrDefaultAsync();
            return user == null;
        }

        public async Task<bool> IsUsernameAvailableAsync(string username, string? excludeUserId = null)
        {
            var query = _userManager.Users.Where(u => u.UserName == username);
            
            if (!string.IsNullOrEmpty(excludeUserId))
                query = query.Where(u => u.Id != excludeUserId);

            var user = await query.FirstOrDefaultAsync();
            return user == null;
        }

        public async Task<int> DeactivateInactiveUsersAsync(int daysInactive)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysInactive);
                var inactiveUsers = await _userManager.Users
                    .Where(u => u.LastLoginAt < cutoffDate && u.IsActive)
                    .ToListAsync();

                foreach (var user in inactiveUsers)
                {
                    user.IsActive = false;
                    user.LastProfileUpdate = DateTime.UtcNow;
                }

                if (inactiveUsers.Any())
                {
                    await _context.SaveChangesAsync();
                    foreach (var user in inactiveUsers)
                        ClearUserCache(user.Id);
                }

                return inactiveUsers.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating inactive users");
                return 0;
            }
        }

        public async Task<int> CleanupOrphanedProfilesAsync()
        {
            // This is a placeholder implementation
            // In a real scenario, you would implement logic to clean up orphaned profiles
            return await Task.FromResult(0);
        }

        public async Task<object> GetUserStatisticsAsync(string userId)
        {
            var user = await GetUserProfileAsync(userId);
            if (user == null)
                return new { };

            return new
            {
                UserId = user.Id,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                LastProfileUpdate = user.LastProfileUpdate,
                IsActive = user.IsActive,
                HasProfilePicture = !string.IsNullOrEmpty(user.ProfilePictureUrl)
            };
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userManager.Users.CountAsync(u => u.IsActive);
        }

        public Task ClearUserCacheAsync(string userId)
        {
            ClearUserCache(userId);
            return Task.CompletedTask;
        }

        public Task ClearAllProfilesCacheAsync()
        {
            // Clear all user profile cache entries
            // This is a simplified implementation
            return Task.CompletedTask;
        }

        private void ClearUserCache(string userId)
        {
            var cacheKeys = new[]
            {
                $"UserProfile_{userId}",
                $"UserProfile_Email_*" // Note: This pattern might not work with IMemoryCache
            };

            foreach (var key in cacheKeys)
            {
                _cache.Remove(key);
            }
        }
    }
}
