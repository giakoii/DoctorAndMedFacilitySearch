using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Logout : PageModel
{
    /// <summary>
    /// Logout action
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Sign out of Cookie Authentication
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirect to HomePage
        return RedirectToPage("/Index");
    }
}