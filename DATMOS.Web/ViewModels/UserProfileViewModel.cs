using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DATMOS.Web.ViewModels
{
    public class UserProfileViewModel
    {
        // Basic Information
        [Display(Name = "Tên học viên")]
        [StringLength(100, ErrorMessage = "Tên học viên không được vượt quá 100 ký tự")]
        public string? FirstName { get; set; }

        [Display(Name = "Họ")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string? LastName { get; set; }

        [Display(Name = "Display Name")]
        [StringLength(100, ErrorMessage = "Tên hiển thị không được vượt quá 100 ký tự")]
        public string? DisplayName { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        // Address Information
        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        [StringLength(50, ErrorMessage = "Thành phố không được vượt quá 50 ký tự")]
        public string? City { get; set; }

        [Display(Name = "State")]
        [StringLength(50, ErrorMessage = "Tỉnh/Thành phố không được vượt quá 50 ký tự")]
        public string? State { get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(20, ErrorMessage = "Mã bưu điện không được vượt quá 20 ký tự")]
        public string? ZipCode { get; set; }

        [Display(Name = "Country")]
        [StringLength(50, ErrorMessage = "Quốc gia không được vượt quá 50 ký tự")]
        public string? Country { get; set; }

        // Preferences
        [Display(Name = "Language")]
        public string? Language { get; set; }

        [Display(Name = "Timezone")]
        public string? Timezone { get; set; }

        [Display(Name = "Currency")]
        public string? Currency { get; set; }

        // Profile Information
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "New Profile Photo")]
        public IFormFile? ProfilePictureFile { get; set; }

        // Additional Fields from AddUsers
        [Display(Name = "Organization")]
        [StringLength(100, ErrorMessage = "Tổ chức không được vượt quá 100 ký tự")]
        public string? Organization { get; set; }

        [Display(Name = "Job Title")]
        [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự")]
        public string? JobTitle { get; set; }

        [Display(Name = "Department")]
        [StringLength(100, ErrorMessage = "Phòng ban không được vượt quá 100 ký tự")]
        public string? Department { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500, ErrorMessage = "Tiểu sử không được vượt quá 500 ký tự")]
        public string? Bio { get; set; }

        [Display(Name = "Website")]
        [Url(ErrorMessage = "URL website không hợp lệ")]
        [StringLength(200, ErrorMessage = "Website không được vượt quá 200 ký tự")]
        public string? Website { get; set; }

        [Display(Name = "Social Links")]
        public string? SocialLinks { get; set; }

        // System Information (Read-only)
        [Display(Name = "User Role")]
        public string? UserRole { get; set; }
        public string Role { get; set; } = "User";
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Login At")]
        public DateTime? LastLoginAt { get; set; }

        [Display(Name = "Last Profile Update")]
        public DateTime? LastProfileUpdate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Phone Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        // Helper Properties
        private string _fullName;
        public string DisplayFullName => string.IsNullOrWhiteSpace(LastName) ? FirstName?.Trim() ?? "" : $"{FirstName} {LastName}".Trim();
        
        // Property to handle FullName binding from form
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string? FullName
        {
            get => _fullName ?? DisplayFullName;
            set
            {
                _fullName = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Phân tích FullName thành FirstName và LastName
                    var parts = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        FirstName = parts[0];
                        if (parts.Length > 1)
                        {
                            LastName = string.Join(" ", parts.Skip(1));
                        }
                        else
                        {
                            LastName = string.Empty;
                        }
                    }
                }
            }
        }
        
        public string Initials
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    // Lấy 2 ký tự đầu của FirstName nếu có
                    var firstName = FirstName.Trim();
                    if (firstName.Length >= 2)
                    {
                        return firstName.Substring(0, 2).ToUpper();
                    }
                    return firstName.Substring(0, 1).ToUpper();
                }
                return "";
            }
        }
    }
}
