using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels;

public class UserViewModel
{
    public string Email { get; set; }
    
    public string FullName { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public int RoleId { get; set; }
    
    public string RoleName { get; set; }
}