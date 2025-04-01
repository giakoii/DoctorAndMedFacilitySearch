using BusinessLogic.Services;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.HealthArticles
{

    public class HealthInfoModel : PageModel
    {
        private readonly IHealthInfoService _healthInfoService;

        public HealthInfoModel(IHealthInfoService healthInfoService)
        {
            _healthInfoService = healthInfoService;
        }

        public List<HealthArticle> Articles { get; set; }
        public List<MedicalFacility> CertifiedMedicalFacilities { get; set; } 
        public void OnGet()
        {
            Articles = _healthInfoService.GetActiveHealthArticles();
            CertifiedMedicalFacilities = _healthInfoService.GetCertifiedMedicalFacilities(); 
        }
    }
}
