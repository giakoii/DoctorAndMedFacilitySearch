using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class MedicalHistory
{
    public int HistoryId { get; set; }

    public int PatientId { get; set; }

    public string Diagnosis { get; set; } = null!;

    public string? Treatment { get; set; }

    public string? Notes { get; set; }

    public DateTime RecordDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;
}
