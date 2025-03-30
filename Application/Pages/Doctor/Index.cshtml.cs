using System.Security.Claims;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Doctor;

public class Index : PageModel
{
    private readonly IUserService _userService;
    
    [BindProperty] public DoctorViewModel DoctorViewModel { get; set; }

    public Index(IUserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        
        var doctor = _userService.FindView(x => x.Email == email).FirstOrDefault();

        if (doctor == null)
        {
            
        }
    }
}