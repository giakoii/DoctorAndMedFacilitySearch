using DataAccessObject.Models;

namespace DataAccessObject.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<MedicalFacility>> GetMedicalFacilities();
        Task<MedicalFacility> GetMedicalFacilitiesById(int id);
        Task<List<DoctorProfile>> GetDoctors();
        Task<List<Schedule>> GetSchedules(int doctorId, DateOnly selectedDate);
        Task<List<DateOnly>> GetAvailableDates(int doctorId, int month, int year);
        Task<string> CreateAppointment(string email, int doctorId, DateOnly selectedDate, int slotId, int facilityId);
        Task<List<Appointment>> GetAppointments(string email, int page, int pageSize);
        Task<int> GetAppointmentsCount(string email);
        Task<Appointment> GetAppointmentById(int id);
        Task SaveChange();
    }
}
