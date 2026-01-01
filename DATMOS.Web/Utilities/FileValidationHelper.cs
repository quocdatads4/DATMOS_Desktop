using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DATMOS.Web.Utilities
{
    public static class FileValidationHelper
    {
        // Allowed file extensions for profile pictures
        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
        };

        // Allowed MIME types for profile pictures
        private static readonly HashSet<string> AllowedImageMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
            "image/webp"
        };

        // Maximum file size in bytes (5MB)
        public const long MaxFileSize = 5 * 1024 * 1024;

        /// <summary>
        /// Validates a profile picture file
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <returns>True if file is valid, false otherwise</returns>
        public static bool ValidateProfilePicture(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if file is null or empty
            if (file == null || file.Length == 0)
            {
                errorMessage = "Vui lòng chọn một tệp hình ảnh.";
                return false;
            }

            // Check file size
            if (file.Length > MaxFileSize)
            {
                errorMessage = $"Kích thước tệp quá lớn. Tối đa {MaxFileSize / (1024 * 1024)}MB.";
                return false;
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension))
            {
                var allowedExtensions = string.Join(", ", AllowedImageExtensions);
                errorMessage = $"Định dạng tệp không được hỗ trợ. Chỉ chấp nhận: {allowedExtensions}";
                return false;
            }

            // Check MIME type (additional security check)
            if (!AllowedImageMimeTypes.Contains(file.ContentType))
            {
                errorMessage = "Loại tệp không được hỗ trợ. Vui lòng chọn tệp hình ảnh hợp lệ.";
                return false;
            }

            // Check if file is actually an image by reading header
            if (!IsValidImageFile(file))
            {
                errorMessage = "Tệp không phải là hình ảnh hợp lệ.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a file is a valid image by reading its header
        /// </summary>
        private static bool IsValidImageFile(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    // Read first few bytes to check image signature
                    var buffer = new byte[8];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Check for common image signatures
                    // JPEG: FF D8 FF
                    if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                        return true;

                    // PNG: 89 50 4E 47 0D 0A 1A 0A
                    if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                        buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
                        return true;

                    // GIF: "GIF87a" or "GIF89a"
                    if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38 &&
                        (buffer[4] == 0x37 || buffer[4] == 0x39) && buffer[5] == 0x61)
                        return true;

                    // BMP: "BM"
                    if (buffer[0] == 0x42 && buffer[1] == 0x4D)
                        return true;

                    // WEBP: "RIFF" followed by "WEBP"
                    if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
                        buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)
                        return true;
                }
            }
            catch
            {
                // If we can't read the file, it's not a valid image
                return false;
            }

            return false;
        }

        /// <summary>
        /// Generates a safe filename for uploaded profile pictures
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="originalFileName">Original filename</param>
        /// <returns>Safe filename with timestamp</returns>
        public static string GenerateProfilePictureFileName(string userId, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var safeUserId = Path.GetFileNameWithoutExtension(userId).Replace(" ", "_").Replace(":", "_");
            
            return $"{safeUserId}_{timestamp}{extension}";
        }

        /// <summary>
        /// Gets the relative path for profile pictures
        /// </summary>
        /// <param name="fileName">Generated filename</param>
        /// <returns>Relative path for the profile picture</returns>
        public static string GetProfilePictureRelativePath(string fileName)
        {
            return $"/uploads/profiles/{fileName}";
        }

        /// <summary>
        /// Gets the physical path for profile pictures
        /// </summary>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="fileName">Generated filename</param>
        /// <returns>Physical path for the profile picture</returns>
        public static string GetProfilePicturePhysicalPath(string webRootPath, string fileName)
        {
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");
            return Path.Combine(uploadsFolder, fileName);
        }

        /// <summary>
        /// Deletes a profile picture file if it exists
        /// </summary>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="relativePath">Relative path of the profile picture</param>
        /// <returns>True if file was deleted or didn't exist, false if deletion failed</returns>
        public static bool DeleteProfilePictureFile(string webRootPath, string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return true;

                var physicalPath = Path.Combine(webRootPath, relativePath.TrimStart('/'));
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    return true;
                }

                return true; // File doesn't exist, which is fine
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates file name for security
        /// </summary>
        /// <param name="fileName">File name to validate</param>
        /// <returns>True if file name is safe, false otherwise</returns>
        public static bool IsSafeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // Check for path traversal attempts
            var invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidChars.Contains(c)))
                return false;

            // Check for directory traversal
            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return false;

            return true;
        }
    }
}
