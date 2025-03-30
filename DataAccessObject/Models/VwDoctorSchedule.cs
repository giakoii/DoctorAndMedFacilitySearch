using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwDoctorSchedule
{
    public int DoctorScheduleId { get; set; }

    public int DoctorId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int MaxPatients { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }
}
