﻿using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwMedicalFacility
{
    public int FacilityId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? Services { get; set; }

    public string? OpeningHours { get; set; }

    public double? Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;
}
