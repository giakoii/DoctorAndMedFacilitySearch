using System.Security.Claims;
using Application.ViewModels;
using AutoMapper;
using BusinessLogic;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Register : PageModel
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    [BindProperty] public RegisterViewModel RegisterViewModel { get; set; }
    
    [TempData] public string Message { get; set; }

    public Register(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<IActionResult> OnPostRegister()
    {
        // Check validation
        if (!ModelState.IsValid)
        {
            Message = "Please check your input";
            return Page();
        }
        
        // Add user
        var newUser = await _userService.Register(RegisterViewModel);
        if (!newUser)
        {
            Message = "Register failed, User already exists";
            return Page();
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, RegisterViewModel.FullName),
            new Claim(ClaimTypes.Email, RegisterViewModel.Email),
            new Claim(ClaimTypes.Role, RegisterViewModel.RoleId.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var redirectPages = new Dictionary<byte, string>
        {
            { (byte)ConstantEnum.Role.Patient, "/Index" },
            { (byte)ConstantEnum.Role.MedicalExpert, "/MedicalExpert/Index" }
        };

        return RedirectToPage(redirectPages.GetValueOrDefault(RegisterViewModel.RoleId, "/Index"));

    }
}