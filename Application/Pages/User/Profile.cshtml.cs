using System.Security.Claims;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Profile : PageModel
{
    [BindProperty] public DoctorViewModel? DoctorProfileViewModel { get; set; }
    
    [BindProperty] public PatientProfileViewModel? PatientProfileViewModel { get; set; }
    
    private readonly IUserService _userService;

    public Profile(IUserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        if (User.IsInRole(ConstantEnum.Role.Patient.ToString()))
        {
            PatientProfileViewModel = _userService.GetPatientProfile(userEmail!);
        }
        else if (User.IsInRole(ConstantEnum.Role.MedicalExpert.ToString()))
        {
            DoctorProfileViewModel  = _userService.GetDoctorProfile(userEmail!);
        }
    }
}