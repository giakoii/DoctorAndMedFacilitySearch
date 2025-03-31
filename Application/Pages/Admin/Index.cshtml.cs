using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Admin;

public class Index : PageModel
{
    private readonly IBaseService<MedicalFacility, int, VwMedicalFacility> _medicalFacilityService;
    
    [BindProperty] public List<MedicalFacilitiesViewModel> MedicalFacility { get; set; }

    public Index(IBaseService<MedicalFacility, int, VwMedicalFacility> medicalFacilityService)
    {
        _medicalFacilityService = medicalFacilityService;
    }

    public void OnGet()
    {
        
    }
}