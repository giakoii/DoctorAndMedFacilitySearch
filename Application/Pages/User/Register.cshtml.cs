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
    private readonly IMapper _mapper;
    
    [BindProperty] public RegisterViewModel RegisterViewModel { get; set; }
    
    [TempData] public string Message { get; set; }

    public Register(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
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
        
        // Đăng nhập ngay sau khi tạo tài khoản
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, RegisterViewModel.FullName),
            new Claim(ClaimTypes.Email, RegisterViewModel.Email),
            new Claim(ClaimTypes.Role, RegisterViewModel.RoleId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), authProperties);

        if (RegisterViewModel.RoleId == ((byte) ConstantEnum.Role.MedicalExpert))
        {
            return RedirectToPage("/Doctor");

        }
        return RedirectToPage("/Index");
    }
}