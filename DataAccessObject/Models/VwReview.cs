using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwReview
{
    public int ReviewId { get; set; }

    public int PatientId { get; set; }

    public int? DoctorId { get; set; }

    public int? FacilityId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;
}
