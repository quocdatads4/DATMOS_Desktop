using DATMOS.Web.Models.ViewModels.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DATMOS.Web.Services.Navigation
{
    public interface IFooterService
    {
        /// <summary>
        /// Get all footer items for a specific footer type and area
        /// </summary>
        Task<List<FooterItemViewModel>> GetFooterAsync(string footerType, string? area = null);

        /// <summary>
        /// Get a specific footer item by ID
        /// </summary>
        Task<FooterItemViewModel?> GetFooterItemByIdAsync(string id);

        /// <summary>
        /// Get footer items grouped by sections
        /// </summary>
        Task<Dictionary<string, List<FooterItemViewModel>>> GetFooterBySectionsAsync(string footerType, string? area = null);

        /// <summary>
        /// Get complete footer data including grouped sections
        /// </summary>
        Task<FooterDataViewModel> GetFooterDataAsync(string footerType, string? area = null);

        /// <summary>
        /// Clear cache for a specific footer type
        /// </summary>
        Task ClearCacheAsync(string footerType);
    }
}
