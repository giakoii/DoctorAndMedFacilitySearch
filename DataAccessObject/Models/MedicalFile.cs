using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class MedicalFile
{
    public int FileId { get; set; }

    public int PatientId { get; set; }

    public string FileName { get; set; } = null!;

    public byte[] FileContent { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public string UploadedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual User Patient { get; set; } = null!;
}
