namespace BusinessLogic.ViewModels;

public class DoctorsWithoutFacilityViewModel
{
    public int DoctorId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Qualification { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public int ExperienceYears { get; set; }

    public string? WorkSchedule { get; set; }
}