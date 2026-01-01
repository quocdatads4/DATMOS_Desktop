using System.Collections.Generic;

namespace DATMOS.Web.Models.ViewModels.Navigation
{
    public class MenuItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public bool IsVisible { get; set; } = true;
        public string MenuType { get; set; } = "Customer";
        public List<MenuItemViewModel>? Children { get; set; }
    }
}
