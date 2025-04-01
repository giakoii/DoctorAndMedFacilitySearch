using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IDoctorAppointmentsService : IBaseService<DataAccessObject.Models.Appointment, int, VwDoctorAppointment>
{
    Task<List<VwDoctorAppointment>> GetAppointmentsByDoctorAsync(int doctorId);
    Task<bool> FinishAppointmentAsync(int appointmentId);
}