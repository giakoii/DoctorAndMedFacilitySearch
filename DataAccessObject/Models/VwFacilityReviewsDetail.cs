using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwFacilityReviewsDetail
{
    public int ReviewId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewCreatedAt { get; set; }

    public bool ReviewIsActive { get; set; }

    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string PatientEmail { get; set; } = null!;

    public DateOnly PatientDob { get; set; }

    public byte PatientGender { get; set; }

    public string? PatientAddress { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public string? BloodType { get; set; }

    public string? EmergencyContact { get; set; }

    public int FacilityId { get; set; }

    public string FacilityName { get; set; } = null!;

    public string FacilityAddress { get; set; } = null!;

    public string FacilityPhone { get; set; } = null!;

    public string? FacilityEmail { get; set; }

    public string? FacilityServices { get; set; }

    public string? FacilityOpeningHours { get; set; }

    public double? FacilityRating { get; set; }
}
