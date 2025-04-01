namespace BusinessLogic.ViewModels
{
    public class MedicalHistoryViewModel
    {
        public int HistoryId { get; set; }
        public int PatientId { get; set; }
        public string Diagnosis { get; set; } = null!;
        public string? Treatment { get; set; }
        public string? Notes { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Now;
    }

}
