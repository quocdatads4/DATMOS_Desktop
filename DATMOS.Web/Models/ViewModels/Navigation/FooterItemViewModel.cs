using System.Collections.Generic;

namespace DATMOS.Web.Models.ViewModels.Navigation
{
    public class FooterItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Section { get; set; } = "Links";
        public int Order { get; set; } = 0;
        public bool IsVisible { get; set; } = true;
        public string FooterType { get; set; } = "Customer";
        public string? ParentId { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Target { get; set; }
        public string CssClass { get; set; } = string.Empty;
        
        public FooterItemViewModel? Parent { get; set; }
        public List<FooterItemViewModel> Children { get; set; } = new List<FooterItemViewModel>();
    }
}
