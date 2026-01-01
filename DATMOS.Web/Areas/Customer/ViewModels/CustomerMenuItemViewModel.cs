using System.Collections.Generic;

namespace DATMOS.Web.Areas.Customer.ViewModels
{
    /// <summary>
    /// ViewModel for customer menu item
    /// </summary>
    public class CustomerMenuItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Url { get; set; }
        public List<CustomerMenuItemViewModel>? Children { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; } = true;
    }

    /// <summary>
    /// ViewModel for customer menu statistics
    /// </summary>
    public class CustomerMenuStatisticsViewModel
    {
        public int TotalMenuItems { get; set; }
        public int ActiveMenuItems { get; set; }
        public int MenuItemsWithChildren { get; set; }
        public int MenuItemsWithoutChildren { get; set; }
    }

    /// <summary>
    /// ViewModel for customer menu data
    /// </summary>
    public class CustomerMenuDataViewModel
    {
        public List<CustomerMenuItemViewModel> MenuItems { get; set; } = new List<CustomerMenuItemViewModel>();
        public CustomerMenuMetadataViewModel? Metadata { get; set; }
    }

    /// <summary>
    /// ViewModel for customer menu metadata
    /// </summary>
    public class CustomerMenuMetadataViewModel
    {
        public string Version { get; set; } = string.Empty;
        public string LastUpdated { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
