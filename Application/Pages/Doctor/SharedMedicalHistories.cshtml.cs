using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Application.Pages.Doctor
{
    public class SharedMedicalHistories : PageModel
    {
        private readonly IUserService _userService;

        public List<SharedMedicalHistoryViewModel> SharedHistories { get; set; }

        public SharedMedicalHistories(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            

            var doctorEmail = User.FindFirstValue(ClaimTypes.Email);
            SharedHistories = _userService.GetSharedMedicalHistories(doctorEmail);

            return Page();
        }
    }

 
}


