using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class SystemConfig
{
    public string Id { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public string ScreenName { get; set; } = null!;
}
