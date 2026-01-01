using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using DATMOS.Core.Entities.Identity;
using DATMOS.Web.Areas.Identity.ViewModels;

namespace DATMOS.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<AddUsers> _signInManager;
        private readonly UserManager<AddUsers> _userManager;
        private readonly IUserStore<AddUsers> _userStore;
        private readonly IUserEmailStore<AddUsers> _emailStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<AddUsers> userManager,
            IUserStore<AddUsers> userStore,
            SignInManager<AddUsers> signInManager,
            ILogger<AccountController> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            
            ViewData["ReturnUrl"] = returnUrl;
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;
            
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    
                    // Lấy thông tin user và roles để xác định redirect URL
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        string redirectUrl = GetRedirectUrlBasedOnRoles(roles, returnUrl);
                        _logger.LogInformation("Redirecting user {Email} with roles {Roles} to {Url}", 
                            user.Email, string.Join(", ", roles), redirectUrl);
                        return LocalRedirect(redirectUrl);
                    }
                    
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;
            
            if (!model.AgreeToTerms)
            {
                ModelState.AddModelError(string.Empty, "You must agree to the terms and conditions.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Automatically assign "User" role (Học viên/Khách hàng)
                    var roleName = "User";
                    var roleExists = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        // Create "User" role if it doesn't exist
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                    // Assign "User" role to the new user
                    await _userManager.AddToRoleAsync(user, roleName);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    if (callbackUrl != null)
                    {
                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string GetRedirectUrlBasedOnRoles(IList<string> roles, string defaultReturnUrl)
        {
            // Ưu tiên: Administrator -> User -> Customer -> Teacher -> các role khác
            if (roles.Contains("Administrator", StringComparer.OrdinalIgnoreCase))
            {
                return "/Admin";
            }
            else if (roles.Contains("User", StringComparer.OrdinalIgnoreCase))
            {
                return "/Customer";
            }
            else if (roles.Contains("Customer", StringComparer.OrdinalIgnoreCase))
            {
                return "/Customer";
            }
            else if (roles.Contains("Teacher", StringComparer.OrdinalIgnoreCase))
            {
                return "/Teacher";
            }
            
            // Fallback về returnUrl hoặc trang chủ
            return !string.IsNullOrEmpty(defaultReturnUrl) ? defaultReturnUrl : "/";
        }

        private AddUsers CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AddUsers>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AddUsers)}'. " +
                    $"Ensure that '{nameof(AddUsers)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Views/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AddUsers> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AddUsers>)_userStore;
        }
    }
}
