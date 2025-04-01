using System.Security.Claims;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Doctor;

public class Index : PageModel
{
    private readonly IDoctorProfileService _doctorProfileService;
    private readonly IUserService _userService;
    
    [BindProperty(SupportsGet = true)] public DoctorViewModel DoctorViewModel { get; set; }
    
    public bool ShowModal { get; set; } = false;
    
    [TempData] public string Message { get; set; }

    public Index(IDoctorProfileService doctorProfileService, IUserService userService)
    {
        _doctorProfileService = doctorProfileService;
        _userService = userService;
    }

    public void OnGet()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        var doctor = _doctorProfileService.FindView(x => x.Email == email).FirstOrDefault();

        if (doctor == null)
        {
            Message = "Please complete your profile";
            ShowModal = true;
            DoctorViewModel = new DoctorViewModel();
            DoctorViewModel.DoctorId = _userService.FindView(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
        }
    }
    
    public async Task<IActionResult> OnPostSaveProfile()
    {
        if (!ModelState.IsValid)
        {
            ShowModal = true;
            return Page();
        }
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;

        await _doctorProfileService.AddDoctorProfile(DoctorViewModel, email);
        Message = "Profile updated successfully!";
        ShowModal = false;

        return RedirectToPage();
    }
}