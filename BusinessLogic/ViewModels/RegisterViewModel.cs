using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    public byte RoleId { get; set; }
}