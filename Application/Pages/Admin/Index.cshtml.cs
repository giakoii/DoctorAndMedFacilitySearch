using System.Security.Claims;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Admin;

public class Index : PageModel
{
    private readonly IAdminService _adminService;

    public List<DoctorsWithoutFacilityViewModel> DoctorsWithoutFacilities { get; set; }
    
    public List<MedicalFacilitiesViewModel> MedicalFacilities { get; set; }
    
    [BindProperty] public MedicalFacilitiesViewModel MedicalFacility { get; set; }
    
    public Index(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public void OnGet()
    {
        DoctorsWithoutFacilities = _adminService.GetDoctorsWithoutFacility();
        MedicalFacilities = _adminService.GetMedicalFacilities();
    }
    
    /// <summary>
    /// Assign a doctor to a facility
    /// </summary>
    /// <param name="doctorId"></param>
    /// <param name="facilityId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> OnPostAssignDoctorToFacilityAsync(int doctorId, int facilityId)
    {
        var adminEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var result = await _adminService.AssignDoctorToFacility(doctorId, facilityId, adminEmail);
        if (result)
        {
            return RedirectToPage();
        }
        return Page();
    }
    
    /// <summary>
    /// Insert a medical facility
    /// </summary>
    /// <param name="medicalFacility"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> OnPostInsertMedicalFacilityAsync()
    {
        var adminEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        var result = await _adminService.InsertMedicalFacility(MedicalFacility, adminEmail);
        if (result)
        {
            return RedirectToPage();
        }
        return Page();
    }
}