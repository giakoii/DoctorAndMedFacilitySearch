﻿namespace DataAccessObject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public virtual DoctorProfile? DoctorProfile { get; set; }

    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    public virtual ICollection<MedicalFile> MedicalFiles { get; set; } = new List<MedicalFile>();

    public virtual ICollection<MedicalHistoryShare> MedicalHistoryShareDoctors { get; set; } = new List<MedicalHistoryShare>();

    public virtual ICollection<MedicalHistoryShare> MedicalHistorySharePatients { get; set; } = new List<MedicalHistoryShare>();

    public virtual PatientProfile? PatientProfile { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
