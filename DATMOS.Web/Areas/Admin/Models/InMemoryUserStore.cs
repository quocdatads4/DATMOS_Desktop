using System.Collections.Generic;
using System.Linq;
using DATMOS.Web.Areas.Admin.ViewModels;

namespace DATMOS.Web.Areas.Admin.Models
{
    public static class InMemoryUserStore
    {
        private static List<UserViewModel> _Users = new List<UserViewModel>
        {
            new UserViewModel { Id = 1, UserName = "admin", Email = "admin@example.com", Role = "Admin", Status = "Active" },
            new UserViewModel { Id = 2, UserName = "teacher1", Email = "teacher1@example.com", Role = "Staff", Status = "Active" },
            new UserViewModel { Id = 3, UserName = "staff2", Email = "staff2@example.com", Role = "Staff", Status = "Locked" }
        };

        public static List<UserViewModel> GetAll() => _Users;

        public static UserViewModel? Get(int id) => _Users.FirstOrDefault(u => u.Id == id);

        public static void Add(UserViewModel user)
        {
            user.Id = _Users.Count > 0 ? _Users.Max(u => u.Id) + 1 : 1;
            _Users.Add(user);
        }

        public static void Update(UserViewModel user)
        {
            var existing = Get(user.Id);
            if (existing != null)
            {
                existing.UserName = user.UserName;
                existing.Email = user.Email;
                existing.Role = user.Role;
                existing.Status = user.Status;
            }
        }

        public static void Delete(int id)
        {
            var u = Get(id);
            if (u != null) _Users.Remove(u);
        }
    }
}
