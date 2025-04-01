using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class ScheduleSlot
{
    public int ScheduleId { get; set; }

    public int SlotId { get; set; }

    public bool? IsBooked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;
}
