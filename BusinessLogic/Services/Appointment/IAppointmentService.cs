using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services.Appointment;

public interface IAppointmentService : IBaseService<DataAccessObject.Models.Appointment, int, VwAppointment>
{
    Task<string> CreateAppointment(string email, int doctorId, DateOnly selectedDate, int slotId, int facilityId);

    Task<List<ScheduleViewModel>> GetSchedulesAsync(int doctorId, DateOnly selectedDate);

    Task<List<DataAccessObject.Models.Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
}