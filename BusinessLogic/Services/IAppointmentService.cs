using BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IAppointmentService
    {
        Task<List<MedicalFacilitiesViewModel>> GetMedicalFacilitiesAsync();
        Task<List<DoctorProfilesViewModel>> GetDoctorsAsync();
        Task<MedicalFacilityViewModel> GetMedicalFacilityByIdAsync(int id);
        Task<List<ScheduleViewModel>> GetSchedulesAsync(int doctorId, DateOnly selectedDate);
        Task<List<DateOnly>> GetAvailableDatesAsync(int doctorId, int month, int year);
        Task<string> CreateAppointmentAsync(string email, int doctorId, DateOnly selectedDate, int slotId, int facilityId);
        Task<List<AppointmentViewModel>> GetAppointmentsViewModel(string email, int page, int pageSize);
        Task<int> GetAppointmentsCount(string email);
        Task<string> UpdateAppointmentAsync(int id, string notes);
    }
}
