using DATMOS.Web.Interfaces;
using DATMOS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DATMOS.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public abstract class BaseProfileController : Controller
    {
        protected readonly IProfileService _profileService;
        protected readonly UserManager<DATMOS.Core.Entities.Identity.AddUsers> _userManager;
        protected readonly ILogger<BaseProfileController> _logger;

        protected BaseProfileController(
            IProfileService profileService,
            UserManager<DATMOS.Core.Entities.Identity.AddUsers> userManager,
            ILogger<BaseProfileController> logger)
        {
            _profileService = profileService;
            _userManager = userManager;
            _logger = logger;
        }

        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        protected string CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email);

        // GET: Profile Index (Main page with tabs)
        public virtual async Task<IActionResult> Index()
        {
            try
            {
                var user = await _profileService.GetUserProfileAsync(CurrentUserId);
                if (user == null)
                {
                    return NotFound();
                }

                var viewModel = MapToUserProfileViewModel(user);
                ViewBag.ActiveTab = "account";
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin hồ sơ.";
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        // GET: Profile/Account (Account tab)
        public virtual async Task<IActionResult> Account()
        {
            try
            {
                var user = await _profileService.GetUserProfileAsync(CurrentUserId);
                if (user == null)
                {
                    return NotFound();
                }

                var viewModel = MapToUserProfileViewModel(user);
                ViewBag.ActiveTab = "account";
                
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading account tab for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin tài khoản.";
                return RedirectToAction("Index");
            }
        }

        // POST: Profile/UpdateAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> UpdateAccount(UserProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ActiveTab = "account";
                    return View("Index", model);
                }

                // Check if email is available (if changed)
                var currentUser = await _profileService.GetUserProfileAsync(CurrentUserId);
                if (currentUser == null)
                {
                    return NotFound();
                }

                if (currentUser.Email != model.Email)
                {
                    var isEmailAvailable = await _profileService.IsEmailAvailableAsync(model.Email, CurrentUserId);
                    if (!isEmailAvailable)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác.");
                        ViewBag.ActiveTab = "account";
                        return View("Index", model);
                    }
                }

                // Handle profile picture upload if a file was provided
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    _logger.LogInformation("Processing profile picture upload for user {UserId}. File: {FileName}, Size: {FileSize} bytes", 
                        CurrentUserId, model.ProfilePictureFile.FileName, model.ProfilePictureFile.Length);
                    
                    var imageUrl = await _profileService.UploadProfilePictureAsync(CurrentUserId, model.ProfilePictureFile);
                    
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        _logger.LogInformation("Profile picture uploaded successfully for user {UserId}. URL: {ImageUrl}", 
                            CurrentUserId, imageUrl);
                        
                        // Update the model with the new profile picture URL
                        // This will be saved when UpdateUserProfileAsync is called
                        model.ProfilePictureUrl = imageUrl;
                        TempData["SuccessMessage"] = "Thông tin tài khoản và ảnh đại diện đã được cập nhật thành công.";
                    }
                    else
                    {
                        _logger.LogWarning("Profile picture upload failed for user {UserId}. File: {FileName}", 
                            CurrentUserId, model.ProfilePictureFile.FileName);
                        TempData["ErrorMessage"] = "Không thể tải lên ảnh đại diện. Vui lòng kiểm tra định dạng và kích thước tệp.";
                        ViewBag.ActiveTab = "account";
                        return View("Index", model);
                    }
                }
                else
                {
                    _logger.LogDebug("No profile picture file provided for user {UserId}", CurrentUserId);
                    
                    // Giữ lại đường dẫn ảnh cũ nếu người dùng không tải lên ảnh mới
                    // Điều này ngăn việc đường dẫn ảnh bị ghi đè thành null nếu View không gửi lại giá trị cũ
                    model.ProfilePictureUrl = currentUser.ProfilePictureUrl;
                }

                var success = await _profileService.UpdateUserProfileAsync(CurrentUserId, model);
                if (success)
                {
                    if (string.IsNullOrEmpty(TempData["SuccessMessage"] as string))
                    {
                        TempData["SuccessMessage"] = "Thông tin tài khoản đã được cập nhật thành công.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật thông tin tài khoản.";
                }

                return RedirectToAction("Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật thông tin tài khoản.";
                return RedirectToAction("Account");
            }
        }

        // GET: Profile/Security (Security tab)
        public virtual async Task<IActionResult> Security()
        {
            try
            {
                var user = await _profileService.GetUserProfileAsync(CurrentUserId);
                if (user == null)
                {
                    return NotFound();
                }

                ViewBag.ActiveTab = "security";
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading security tab for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin bảo mật.";
                return RedirectToAction("Index");
            }
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                    return RedirectToAction("Security");
                }

                if (newPassword != confirmPassword)
                {
                    TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                    return RedirectToAction("Security");
                }

                var success = await _profileService.ChangePasswordAsync(CurrentUserId, currentPassword, newPassword);
                if (success)
                {
                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể thay đổi mật khẩu. Vui lòng kiểm tra mật khẩu hiện tại.";
                }

                return RedirectToAction("Security");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thay đổi mật khẩu.";
                return RedirectToAction("Security");
            }
        }

        // GET: Profile/Billing (Billing tab - placeholder)
        public virtual IActionResult Billing()
        {
            ViewBag.ActiveTab = "billing";
            return View("Index");
        }

        // GET: Profile/Notifications (Notifications tab - placeholder)
        public virtual IActionResult Notifications()
        {
            ViewBag.ActiveTab = "notifications";
            return View("Index");
        }

        // GET: Profile/Connections (Connections tab - placeholder)
        public virtual IActionResult Connections()
        {
            ViewBag.ActiveTab = "connections";
            return View("Index");
        }

        // POST: Profile/UploadProfilePicture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            try
            {
                if (profilePicture == null || profilePicture.Length == 0)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn một tệp hình ảnh.";
                    return RedirectToAction("Account");
                }

                var imageUrl = await _profileService.UploadProfilePictureAsync(CurrentUserId, profilePicture);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    TempData["SuccessMessage"] = "Ảnh đại diện đã được cập nhật thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải lên ảnh đại diện. Vui lòng kiểm tra định dạng và kích thước tệp.";
                }

                return RedirectToAction("Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải lên ảnh đại diện.";
                return RedirectToAction("Account");
            }
        }

        // POST: Profile/DeleteProfilePicture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteProfilePicture()
        {
            try
            {
                var success = await _profileService.DeleteProfilePictureAsync(CurrentUserId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Ảnh đại diện đã được xóa thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa ảnh đại diện.";
                }

                return RedirectToAction("Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture for user {UserId}", CurrentUserId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa ảnh đại diện.";
                return RedirectToAction("Account");
            }
        }

        // Helper method to map AddUsers to UserProfileViewModel
        protected virtual UserProfileViewModel MapToUserProfileViewModel(DATMOS.Core.Entities.Identity.AddUsers user)
        {
            return new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                Language = user.Language,
                Timezone = user.Timezone,
                Currency = user.Currency,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Organization = user.Organization,
                JobTitle = user.JobTitle,
                Department = user.Department,
                Bio = user.Bio,
                Website = user.Website,
                SocialLinks = user.SocialLinks,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                LastProfileUpdate = user.LastProfileUpdate,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                UserRole = user.UserRole
            };
        }

        // Helper method to get user role for display
        protected virtual string GetUserRoleDisplayName(string? role)
        {
            return role switch
            {
                "Admin" => "Quản trị viên",
                "Customer" => "Học viên",
                "Teacher" => "Giáo viên",
                _ => "Người dùng"
            };
        }

        // Override this method in derived controllers to add custom logic
        protected virtual void PrepareViewModel(UserProfileViewModel model)
        {
            // Base implementation does nothing
            // Derived controllers can override to add custom data
        }
    }
}
