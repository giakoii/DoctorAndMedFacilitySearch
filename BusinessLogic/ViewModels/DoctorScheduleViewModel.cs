namespace BusinessLogic.ViewModels;

public class DoctorScheduleViewModel
{
    public int DoctorScheduleId { get; set; }

    public int DoctorId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int MaxPatients { get; set; }
}