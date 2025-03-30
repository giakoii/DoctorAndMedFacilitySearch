using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class ScheduleSlot
{
    public int ScheduleId { get; set; }

    public int SlotId { get; set; }

    public bool? IsBooked { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;
}
