using System.Collections.Generic;

namespace DATMOS.Web.Models.ViewModels.Navigation
{
    public class FooterDataViewModel
    {
        public string FooterType { get; set; } = "Customer";
        public string? Area { get; set; }
        public Dictionary<string, List<FooterItemViewModel>> FooterBySections { get; set; } = new Dictionary<string, List<FooterItemViewModel>>();
    }
}
