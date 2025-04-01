using BusinessLogic.ViewModels;
using DataAccessObject.Models.Helper;
using DataAccessObject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppDbContext _context;
        public AppointmentService(IAppointmentRepository appointmentRepository, AppDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _context = context;
        }
        private string GetStartTimeFromSlotId(int slotId)
        {
            var slotMap = new Dictionary<int, string>
            {
                {1, "07:00"}, {2, "07:30"}, {3, "08:00"}, {4, "08:30"},
                {5, "09:00"}, {6, "09:30"}, {7, "10:00"}, {8, "10:30"},
                {9, "11:00"}, {10, "11:30"}, {11, "12:00"}, {12, "12:30"},
                {13, "13:00"}, {14, "13:30"}, {15, "14:00"}, {16, "14:30"},
                {17, "15:00"}, {18, "15:30"}, {19, "16:00"}, {20, "16:30"}
            };
            return slotMap.TryGetValue(slotId, out string startTime) ? startTime : null;
        }
        public Task<string> ChangeAppointmentScheduleAsync(int appointmentId, int doctorId, int scheduleId, int slotId)
        {
            try
            {
                var existingAppointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
                if (existingAppointment == null)
                {
                    return Task.FromResult("Error: Appointment not found");
                }
                var existingSchedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
                if (existingSchedule == null)
                {
                    return Task.FromResult("Error: Schedule not found");
                }
                var existingSlot = _context.ScheduleSlots.FirstOrDefault(ss => ss.SlotId == slotId && ss.ScheduleId == scheduleId && ss.IsBooked == false);
                if (existingSlot == null)
                {
                    return Task.FromResult("Error: Slot not found");
                }
                var existingDoctor = _context.Users.FirstOrDefault(u => u.UserId == doctorId);
                if (existingDoctor == null)
                {
                    return Task.FromResult("Error: Doctor not found");
                }
                var oldSlot = _context.ScheduleSlots.FirstOrDefault(ss => ss.SlotId == existingAppointment.SlotId && ss.ScheduleId == existingAppointment.ScheduleId);
                if (oldSlot != null)
                {
                    oldSlot.IsBooked = false;
                }
                existingAppointment.DoctorId = doctorId;
                existingAppointment.ScheduleId = scheduleId;
                existingAppointment.SlotId = slotId;
                existingAppointment.AppointmentDate = DateTime.Parse(existingSchedule.ScheduleDate.ToString() + " " + GetStartTimeFromSlotId(slotId));
                existingAppointment.UpdatedAt = DateTime.Now;
                existingAppointment.UpdatedBy = "System";
                existingSlot.IsBooked = true;
                _context.SaveChanges();
                return Task.FromResult("Appointment schedule updated successfully");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateAppointmentAsync(string email, int doctorId, DateOnly selectedDate, int slotId, int facilityId)
        {
            try
            {
                return await _appointmentRepository.CreateAppointment(email, doctorId, selectedDate, slotId, facilityId);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<AppointmentViewModel> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(id);
                var appointmentViewModel = new AppointmentViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    Status = appointment.Status ?? "Unknown",
                    PaymentStatus = appointment.PaymentStatus ?? "Unknown",
                    Notes = appointment.Notes,
                    PatientName = _context.Users.Where(u => u.UserId == appointment.PatientId).Select(u => u.FullName + " (" + u.Email + ")").FirstOrDefault() ?? "Unknown Patient",
                    DoctorName = _context.Users.Where(u => u.UserId == appointment.DoctorId).Select(u => u.FullName + " (" + u.Email + ")").FirstOrDefault() ?? "Unknown Doctor",
                    IsActive = appointment.IsActive,
                    Facility = appointment.Facility != null
                       ? new MedicalFacilitiesViewModel
                       {
                           FacilityId = appointment.Facility.FacilityId,
                           Name = appointment.Facility.Name ?? "Unknown Facility",
                           Address = appointment.Facility.Address ?? "",
                           Phone = appointment.Facility.Phone ?? "",
                           Email = appointment.Facility.Email ?? "",
                           Services = appointment.Facility.Services ?? "",
                           OpeningHours = appointment.Facility.OpeningHours ?? "",
                           Rating = appointment.Facility.Rating,
                           IsActive = appointment.Facility.IsActive
                       }
                       : new MedicalFacilitiesViewModel(),
                    CreatedAt = appointment.CreatedAt,
                    UpdatedAt = appointment.UpdatedAt
                };
                return appointmentViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetAppointmentsCount(string email)
        {
            return await _appointmentRepository.GetAppointmentsCount(email);
        }

        public async Task<List<AppointmentViewModel>> GetAppointmentsViewModel(string email, int page, int pageSize)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointments(email, page, pageSize);
                var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status ?? "Unknown",
                    PaymentStatus = a.PaymentStatus ?? "Unknown",
                    Notes = a.Notes,
                    PatientName = _context.Users.Where(u => u.UserId == a.PatientId).Select(u => u.FullName + " (" + u.Email + ")").FirstOrDefault() ?? "Unknown Patient",
                    DoctorName = _context.Users.Where(u => u.UserId == a.DoctorId).Select(u => u.FullName + " (" + u.Email + ")").FirstOrDefault() ?? "Unknown Doctor",
                    IsActive = a.IsActive,
                    Facility = a.Facility != null
                       ? new MedicalFacilitiesViewModel
                       {
                           FacilityId = a.Facility.FacilityId,
                           Name = a.Facility.Name ?? "Unknown Facility",
                           Address = a.Facility.Address ?? "",
                           Phone = a.Facility.Phone ?? "",
                           Email = a.Facility.Email ?? "",
                           Services = a.Facility.Services ?? "",
                           OpeningHours = a.Facility.OpeningHours ?? "",
                           Rating = a.Facility.Rating,
                           IsActive = a.Facility.IsActive
                       }
                       : new MedicalFacilitiesViewModel(),
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();
                return appointmentViewModels;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DateOnly>> GetAvailableDatesAsync(int doctorId, int month, int year)
        {
            try
            {
                return await _appointmentRepository.GetAvailableDates(doctorId, month, year);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<DoctorProfilesViewModel>> GetDoctorsAsync()
        {
            try
            {
                var doctorProfiles = await _appointmentRepository.GetDoctors();
                var doctors = doctorProfiles.Select(dp => new DoctorProfilesViewModel
                {
                    DoctorId = dp.DoctorId,
                    DoctorName = dp.Doctor.FullName,
                    Qualification = dp.Qualification,
                    Specialty = dp.Specialty,
                    ExperienceYears = dp.ExperienceYears,
                    ConsultationFee = dp.ConsultationFee,
                    Availability = dp.Availability,
                    Facilities = dp.Facilities != null
                         ? dp.Facilities.Select(f => new MedicalFacilityViewModel
                         {
                             FacilityId = f.FacilityId,
                             Name = f.Name,
                             Address = f.Address,
                             Phone = f.Phone,
                             Email = f.Email,
                             Services = f.Services,
                             OpeningHours = f.OpeningHours,
                             Rating = f.Rating,
                         }).ToList()
                         : new List<MedicalFacilityViewModel>()
                })
                .ToList();

                return doctors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ScheduleModel>> GetDoctorSchedulesAsync(int doctorId)
        {
            try
            {
                var slotTimes = new Dictionary<int, string>
                {
                    {1, "07:00"}, {2, "07:30"}, {3, "08:00"}, {4, "08:30"},
                    {5, "09:00"}, {6, "09:30"}, {7, "10:00"}, {8, "10:30"},
                    {9, "11:00"}, {10, "11:30"}, {11, "12:00"}, {12, "12:30"},
                    {13, "13:00"}, {14, "13:30"}, {15, "14:00"}, {16, "14:30"},
                    {17, "15:00"}, {18, "15:30"}, {19, "16:00"}, {20, "16:30"}
                };

                var schedules = await _context.Schedules
                    .Where(s => s.DoctorId == doctorId)
                    .Join(_context.ScheduleSlots.Where(slot => slot.IsBooked == false),
                        schedule => schedule.ScheduleId,
                        slot => slot.ScheduleId,
                        (schedule, slot) => new { schedule, slot })
                    .ToListAsync();

                var scheduleModels = schedules.Select(s => new ScheduleModel
                {
                    ScheduleId = s.schedule.ScheduleId,
                    SlotId = s.slot.SlotId,
                    SlotTime = slotTimes.ContainsKey(s.slot.SlotId) ? slotTimes[s.slot.SlotId] : "Unknown",
                    ScheduleDate = s.schedule.ScheduleDate
                }).ToList();

                return scheduleModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MedicalFacilitiesViewModel>> GetMedicalFacilitiesAsync()
        {
            try
            {
                var facilities = await _appointmentRepository.GetMedicalFacilities();
                var medicalFacilities = facilities.Select(mf => new MedicalFacilitiesViewModel
                {
                    FacilityId = mf.FacilityId,
                    Name = mf.Name,
                    Address = mf.Address,
                    Phone = mf.Phone,
                    Email = mf.Email,
                    Services = mf.Services,
                    OpeningHours = mf.OpeningHours,
                    Rating = mf.Rating,
                    IsActive = mf.IsActive
                }).ToList();
                return medicalFacilities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<MedicalFacilityViewModel> GetMedicalFacilityByIdAsync(int id)
        {
            try
            {
                var facility = await _appointmentRepository.GetMedicalFacilitiesById(id);
                var medicalFacility = new MedicalFacilityViewModel
                {
                    FacilityId = facility.FacilityId,
                    Name = facility.Name,
                    Address = facility.Address,
                    Phone = facility.Phone,
                    Email = facility.Email,
                    Services = facility.Services,
                    OpeningHours = facility.OpeningHours,
                    Rating = facility.Rating,
                };
                return medicalFacility;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ScheduleViewModel>> GetSchedulesAsync(int doctorId, DateOnly selectedDate)
        {
            try
            {
                var schedules = await _appointmentRepository.GetSchedules(doctorId, selectedDate);

                var list = schedules.Select(s => new ScheduleViewModel
                {
                    ScheduleId = s.ScheduleId,
                    DoctorId = s.DoctorId,
                    ScheduleDate = s.ScheduleDate,
                    ScheduleSlots = s.ScheduleSlots.Select(ss => new ScheduleSlotViewModel
                    {
                        SlotId = ss.SlotId,
                        StartTime = ss.Slot.StartTime,
                        EndTime = ss.Slot.EndTime
                    }).ToList()
                }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateAppointmentAsync(int id, string notes)
        {
            try
            {
                var existingAppointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == id);
                if (existingAppointment == null)
                {
                    return "Error: Appointment not found";
                }
                existingAppointment.Status = "Cancelled";
                existingAppointment.Notes = $"An appointment on {existingAppointment.AppointmentDate} has been cancelled.\nReason: {notes}\nThank you for using service!";
                existingAppointment.IsActive = false;
                existingAppointment.UpdatedAt = DateTime.Now;
                existingAppointment.UpdatedBy = "System";
                var scheduleSlot = await _context.ScheduleSlots.FirstOrDefaultAsync(ss => ss.SlotId == existingAppointment.SlotId && ss.ScheduleId == existingAppointment.ScheduleId);
                if (scheduleSlot != null)
                {
                    scheduleSlot.IsBooked = false;
                }
                await _context.SaveChangesAsync();
                return "Appointment updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
