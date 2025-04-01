using System.Security.Claims;
using AutoMapper;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Application.Pages.User;

public class Register : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;
    
    [BindProperty] public RegisterViewModel RegisterViewModel { get; set; }
    
    [TempData] public string Message { get; set; }

    public Register(IUserService userService, IMapper mapper, IRoleService roleService)
    {
        _userService = userService;
        _mapper = mapper;
        _roleService = roleService;
    }

    public async Task<IActionResult> OnGet(string email, string fullName)
    {
        RegisterViewModel = new RegisterViewModel { FullName = fullName, Email = email };
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        foreach (var modelState in ModelState)
        {
            foreach (var error in modelState.Value.Errors)
            {
                Console.WriteLine($"- {modelState.Key}: {error.ErrorMessage}");
            }
        }
        
        if (!ModelState.IsValid) 
            return Page();

        _userService.Register(RegisterViewModel);
        
        var roleName = _roleService.Find(x => x.RoleId == RegisterViewModel.RoleId).Select(x => x.RoleName).FirstOrDefault();
        
        // Login user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, RegisterViewModel.FullName),
            new Claim(ClaimTypes.Email, RegisterViewModel.Email),
            new Claim(ClaimTypes.Role, RegisterViewModel.RoleId.ToString()),
            new Claim("RoleName", roleName),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), authProperties);

        if (RegisterViewModel.RoleId == ((byte) ConstantEnum.Role.MedicalExpert))
        {
            return RedirectToPage("/Doctor/Index");

        }
        return RedirectToPage("/Care");
    }
}