using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.ViewModels;

public class DoctorViewModel
{
    public int DoctorId { get; set; }
    
    [Required(ErrorMessage = "Qualification is required")]
    public string Qualification { get; set; } = null!;
    
    public string? Name { get; set; } = null!;

    [Required(ErrorMessage = "Specialty is required")]
    public string Specialty { get; set; } = null!;

    [Required(ErrorMessage = "Experience years is required")]
    public int ExperienceYears { get; set; }

    [Required(ErrorMessage = "Work schedule is required")]
    public string? WorkSchedule { get; set; }

    [Required(ErrorMessage = "Consultation fee is required")]
    public decimal? ConsultationFee { get; set; }

    [Required(ErrorMessage = "Availability is required")]
    public string? Availability { get; set; }
}
