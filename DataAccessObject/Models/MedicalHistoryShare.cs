using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class MedicalHistoryShare
{
    public int ShareId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime? SharedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual User Patient { get; set; } = null!;
}
