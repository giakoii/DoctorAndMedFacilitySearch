namespace BusinessLogic.ViewModels
{
    public class AppointmentCreationModel
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewCreatedAt { get; set; }
        public bool ReviewIsActive { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public int FacilityId { get; set; }
    }
}
