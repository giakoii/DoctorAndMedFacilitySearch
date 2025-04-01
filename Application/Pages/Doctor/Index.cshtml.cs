using System.Security.Claims;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Doctor;

public class Index : PageModel
{
    private readonly IDoctorProfileService _doctorProfileService;
    
    [BindProperty] public DoctorViewModel DoctorViewModel { get; set; }
    
    public bool ShowModal { get; set; } = false;
    
    [TempData] public string Message { get; set; }

    public Index(IDoctorProfileService doctorProfileService)
    {
        _doctorProfileService = doctorProfileService;
    }

    public void OnGet()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        var doctor = _doctorProfileService.FindView(x => x.Email == email).FirstOrDefault();

        if (doctor == null)
        {
            Message = "Please complete your profile";
            ShowModal = true;
        }
    }
    
    public IActionResult OnPostSaveProfile()
    {
       
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;

        _doctorProfileService.AddDoctorProfile(DoctorViewModel, email);
        Message = "Profile updated successfully!";
        ShowModal = false;

        return RedirectToPage();
    }
}