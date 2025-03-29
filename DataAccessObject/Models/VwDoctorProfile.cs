using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwDoctorProfile
{
    public int DoctorId { get; set; }

    public string Qualification { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public int ExperienceYears { get; set; }

    public string? WorkSchedule { get; set; }

    public decimal? ConsultationFee { get; set; }

    public string? Availability { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? Name { get; set; }
}
