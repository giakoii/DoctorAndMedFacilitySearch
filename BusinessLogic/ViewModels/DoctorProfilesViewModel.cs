namespace BusinessLogic.ViewModels
{
    public class DoctorProfilesViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string Qualification { get; set; } = null!;
        public string WorkSchedule { get; set; }
        public string Specialty { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Availability { get; set; }
        public List<MedicalFacilityViewModel> Facilities { get; set; } = new List<MedicalFacilityViewModel>();
    }
}
