namespace DataAccessObject.Models;

public partial class PatientProfile
{
    public int PatientId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public byte Gender { get; set; }

    public string? Address { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public string? BloodType { get; set; }

    public string? EmergencyContact { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual User Patient { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
