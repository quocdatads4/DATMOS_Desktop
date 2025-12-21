using System.ComponentModel.DataAnnotations;

namespace DATMOS.Web.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin or Staff
        public string Status { get; set; } = string.Empty; // Active or Locked
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
