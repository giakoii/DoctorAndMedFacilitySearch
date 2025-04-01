using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.ViewModels;

public class PatientProfileViewModel
{
    public int PatientId { get; set; }
    
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Date of birth is required")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public byte Gender { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Medical history is required")]
    public string? MedicalHistory { get; set; }

    [Required(ErrorMessage = "Allergies is required")]
    public string? Allergies { get; set; }

    [Required(ErrorMessage = "Blood type is required")]
    public string? BloodType { get; set; }

    [Required(ErrorMessage = "Emergency contact is required")]
    public string? EmergencyContact { get; set; }
}
