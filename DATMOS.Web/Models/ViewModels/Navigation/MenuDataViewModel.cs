using System.Collections.Generic;

namespace DATMOS.Web.Models.ViewModels.Navigation
{
    public class MenuDataViewModel
    {
        public List<MenuItemViewModel>? MenuItems { get; set; }
        public MenuMetadataViewModel? Metadata { get; set; }
    }

    public class MenuMetadataViewModel
    {
        public string Version { get; set; } = "1.0.0";
        public string LastUpdated { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
