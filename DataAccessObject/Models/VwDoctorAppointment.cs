using System;
using System.Collections.Generic;

namespace DataAccessObject.Models;

public partial class VwDoctorAppointment
{
    public int AppointmentId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime AppointmentCreatedAt { get; set; }

    public bool AppointmentIsActive { get; set; }

    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string PatientEmail { get; set; } = null!;

    public DateOnly PatientDob { get; set; }

    public byte PatientGender { get; set; }

    public string? PatientAddress { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public string? BloodType { get; set; }

    public string? EmergencyContact { get; set; }

    public int DoctorId { get; set; }

    public string DoctorName { get; set; } = null!;

    public string DoctorEmail { get; set; } = null!;

    public string DoctorSpecialty { get; set; } = null!;

    public string DoctorQualification { get; set; } = null!;

    public int DoctorExperienceYears { get; set; }

    public int SlotId { get; set; }

    public TimeOnly SlotStartTime { get; set; }

    public TimeOnly SlotEndTime { get; set; }

    public int ScheduleId { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public int? FacilityId { get; set; }

    public string? FacilityName { get; set; }

    public string? FacilityAddress { get; set; }

    public string? FacilityPhone { get; set; }

    public string? FacilityEmail { get; set; }
}
