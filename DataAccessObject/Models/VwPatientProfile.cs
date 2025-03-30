using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwPatientProfile
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
}
