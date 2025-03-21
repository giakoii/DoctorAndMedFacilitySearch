using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.ViewModels;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    
    [TempData] public string Message { get; set; }
    
    /// <summary>
    /// Login
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostLogin()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please enter Email and Password";
            return Page();
        }

        var userLogin = _userService.Login(email, password);
        if (userLogin == null)
        {
            Message = "Invalid username or password";
            return Page();
        }
        
        // Set Claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userLogin.FullName),
            new Claim(ClaimTypes.Email, userLogin.Email),
            new Claim(ClaimTypes.Role, userLogin.RoleId.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("/Index");
    }
}