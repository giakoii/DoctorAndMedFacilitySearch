using DataAccessObject.Models;

namespace BusinessLogic.Services.Appointment;

public interface IAppointmentService : IBaseService<DataAccessObject.Models.Appointment, int, VwAppointment>
{
    
}