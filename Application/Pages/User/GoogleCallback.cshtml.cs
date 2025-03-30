using System.Security.Claims;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Application.Pages.User;

public class GoogleCallback : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public GoogleCallback(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return RedirectToPage("User/Login");
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

        var userExist = await _userService.Find(x => x.Email == email).FirstOrDefaultAsync();
        if (userExist != null)
        {
            await SignInUser(userExist);
            return RedirectToPage("/Index");
        }

        // 
        return RedirectToPage("./Register", new { email, fullName });
    }

    private async Task SignInUser(DataAccessObject.Models.User user)
    {
        var roleName = _roleService.FindView(x => x.RoleId == user.RoleId).Select(x => x.RoleName).FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim("RoleName", roleName)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
    }
}