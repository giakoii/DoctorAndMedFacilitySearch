using System.ComponentModel.DataAnnotations;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Login : PageModel
{
    private readonly IUserService _userService;
    

    public Login(IUserService userService)
    {
        _userService = userService;
    }
    
    [Required(ErrorMessage = "Email is required")]
    [BindProperty] public string email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [BindProperty] public string password { get; set; }
    
    [BindProperty] public string? phoneNumber { get; set; }
    
    [TempData] public string Message { get; set; }
    
    
    public IActionResult OnGet()
    {
        var redirectUrl = Url.Page("/User/GoogleCallback", pageHandler: null, values: null, protocol: Request.Scheme);
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Parameters = { { "prompt", "select_account" } }
        };

        return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
    }
}