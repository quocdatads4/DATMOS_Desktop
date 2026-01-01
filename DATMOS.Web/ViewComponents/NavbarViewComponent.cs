using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DATMOS.Web.ViewComponents
{
    /// <summary>
    /// View Component này chịu trách nhiệm hiển thị thanh điều hướng (Navbar) chung của trang web.
    /// Nó hỗ trợ nhiều loại navbar khác nhau: Default, Customer, Admin, Teacher.
    /// </summary>
    public class NavbarViewComponent : ViewComponent
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<DATMOS.Core.Entities.Identity.AddUsers> _userManager;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NavbarViewComponent> _logger;

        public NavbarViewComponent(Microsoft.AspNetCore.Identity.UserManager<DATMOS.Core.Entities.Identity.AddUsers> userManager,
                                  Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor,
                                  ILogger<NavbarViewComponent> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync(string type = "Default")
        {
            // Xác định view name dựa trên type
            string viewName = GetViewName(type);

            // Nếu người dùng đã đăng nhập, lấy thông tin người dùng
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    // Tạo DisplayName với logic fallback: ưu tiên user.DisplayName, sau đó FirstName + LastName, cuối cùng là Email
                    string displayName = !string.IsNullOrWhiteSpace(user.DisplayName)
                        ? user.DisplayName
                        : $"{user.FirstName} {user.LastName}".Trim();

                    // Nếu vẫn trống, sử dụng Email làm fallback
                    bool usedFallback = false;
                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = user.Email ?? user.UserName ?? "Người dùng";
                        usedFallback = true;
                    }

                    var model = new DATMOS.Web.ViewModels.UserProfileViewModel
                    {
                        DisplayName = displayName,
                        Email = user.Email,
                        ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatars/1.png",
                        Role = roles.Count > 0 ? roles[0] : "User"
                    };
                    // Log the user info for debugging
                    _logger.LogInformation("Navbar dropdown model - DisplayName: {DisplayName}, Email: {Email}, Role: {Role}, UsedFallback: {UsedFallback}",
                                            model.DisplayName, model.Email, model.Role, usedFallback);
                    // Warn if model still contains default/empty values
                    if (string.IsNullOrWhiteSpace(model.DisplayName))
                    {
                        _logger.LogWarning("Navbar dropdown still shows static data (DisplayName empty).");
                    }
                    if (string.IsNullOrWhiteSpace(model.Email))
                    {
                        _logger.LogWarning("Navbar dropdown still shows static data (Email empty).");
                    }
                    return View(viewName, model);
                }
            }

            // Người chưa đăng nhập → trả về view không model
            return View(viewName);
        }

        private string GetViewName(string type)
        {
            // Map type to view name
            return type?.ToLower() switch
            {
                "customer" => "Customer",
                "admin" => "Admin",
                "teacher" => "Teacher",
                _ => "Default"
            };
        }
    }
}
