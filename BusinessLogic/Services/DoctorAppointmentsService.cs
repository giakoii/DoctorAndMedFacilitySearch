using DataAccessObject;
using DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class DoctorAppointmentsService : BaseService<DataAccessObject.Models.Appointment, int, VwDoctorAppointment>, IDoctorAppointmentsService
{
    public DoctorAppointmentsService(IBaseRepository<DataAccessObject.Models.Appointment, int, VwDoctorAppointment> repository) : base(repository)
    {
    }

    public async Task<List<VwDoctorAppointment>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        var result = await FindView(x => x.DoctorId == doctorId).ToListAsync();
        return result;
    }
    public async Task<bool> FinishAppointmentAsync(int appointmentId)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId);
        if (appointment == null)
            return false;

        appointment.Status = ConstantEnum.AppointmentStatus.Confirmed.ToString();
        _repository.Update(appointment);
        await _repository.SaveChangesAsync("system", false);
        return true;
    }

}