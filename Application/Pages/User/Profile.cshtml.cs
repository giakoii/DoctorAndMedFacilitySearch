using System.Security.Claims;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Profile : PageModel
{
    [BindProperty(SupportsGet = true)]
    public DoctorViewModel? DoctorProfileViewModel { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public PatientProfileViewModel? PatientProfileViewModel { get; set; }
    
    private readonly IUserService _userService;
    private readonly IDoctorProfileService _doctorProfileService;
    private readonly IPatientProfileService _patientProfileService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="doctorProfileService"></param>
    /// <param name="patientProfileService"></param>
    public Profile(IUserService userService, IDoctorProfileService doctorProfileService, IPatientProfileService patientProfileService)
    {
        _userService = userService;
        _doctorProfileService = doctorProfileService;
        _patientProfileService = patientProfileService;
    }

    public void OnGet()
    {
        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        if (User.IsInRole(( (byte) ConstantEnum.Role.Patient).ToString()))
        {
            PatientProfileViewModel = _patientProfileService.GetPatientProfile(userEmail!);
        }
        else if (User.IsInRole(((byte) ConstantEnum.Role.MedicalExpert).ToString()))
        {
            DoctorProfileViewModel  = _doctorProfileService.GetDoctorProfile(userEmail!);
        }
    }

    /// <summary>
    /// Edit profile
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostEditProfile()
    {
        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        if (User.IsInRole(( (byte) ConstantEnum.Role.MedicalExpert).ToString()))
        {
            await _doctorProfileService.UpdateDoctorProfile(DoctorProfileViewModel!, userEmail);
        }
        else if (User.IsInRole(((byte) ConstantEnum.Role.Patient).ToString()))
        {
           await _patientProfileService.UpdatePatientProfile(PatientProfileViewModel!, userEmail);
        }

        return RedirectToPage();
    }
}