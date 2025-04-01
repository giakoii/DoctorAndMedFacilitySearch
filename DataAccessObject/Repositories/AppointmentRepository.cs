using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
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
        public async Task<string> CreateAppointment(string email, int doctorId, DateOnly selectedDate, int slotId, int facilityId)
        {
            try
            {
                var patient = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (patient == null) return "Patient not found.";

                var doctor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == doctorId);
                if (doctor == null) return "Doctor not found.";

                var schedule = await _context.Schedules.Include(s => s.ScheduleSlots).FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.ScheduleDate == selectedDate);
                if (schedule == null) return "Schedule not found.";

                var existingAppointment = await _context.Appointments.FirstOrDefaultAsync(a => (a.ScheduleId == schedule.ScheduleId && a.SlotId == slotId) && (a.IsActive == true));
                if (existingAppointment != null) return "Slot already booked.";

                var facility = await _context.MedicalFacilities.FirstOrDefaultAsync(mf => mf.FacilityId == facilityId);
                if (facility == null) return "Facility not found.";

                var slotToBook = schedule.ScheduleSlots.FirstOrDefault(s => s.SlotId == slotId && s.ScheduleId == schedule.ScheduleId);
                if (slotToBook == null) return "Slot not found in schedule.";

                var appointment = new Appointment
                {
                    PatientId = patient.UserId,
                    DoctorId = doctorId,
                    ScheduleId = schedule.ScheduleId,
                    SlotId = slotToBook.SlotId,
                    FacilityId = facility.FacilityId,
                    AppointmentDate = DateTime.Parse(selectedDate.ToString() + " " + GetStartTimeFromSlotId(slotId)),
                    Status = "Pending",
                    PaymentStatus = "Paid",
                    Notes = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = email,
                    IsActive = true,
                    CreatedBy = email
                };
                await _context.Appointments.AddAsync(appointment);
                slotToBook.IsBooked = true;
                await _context.SaveChangesAsync();
                return "Appointment created successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<List<Appointment>> GetAppointments(string email, int page, int pageSize)
        {
            try
            {
                var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new List<Appointment>();
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.Facility)
                    .Include(a => a.ScheduleSlot)
                    .Where(a => a.PatientId == user.UserId || a.DoctorId == user.UserId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetAppointmentsCount(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return 0;
                }

                return await _context.Appointments
                    .Where(a => a.PatientId == user.UserId || a.DoctorId == user.UserId)
                    .CountAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DateOnly>> GetAvailableDates(int doctorId, int month, int year)
        {
            try
            {
                return await _context.Schedules
                    .Where(s => s.DoctorId == doctorId && s.ScheduleDate.Month == month && s.ScheduleDate.Year == year && s.ScheduleSlots.Any(slot => slot.IsBooked == false || slot.IsBooked == null))
                    .Select(s => s.ScheduleDate)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DoctorProfile>> GetDoctors()
        {
            try
            {
                return await _context.DoctorProfiles.Include(d => d.Doctor)
                    .Include(d => d.Facilities)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MedicalFacility>> GetMedicalFacilities()
        {
            try
            {
                return await _context.MedicalFacilities.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MedicalFacility> GetMedicalFacilitiesById(int id)
        {
            try
            {
                var facility = await _context.MedicalFacilities.FirstOrDefaultAsync(mf => mf.FacilityId == id);
                if (facility == null)
                {
                    throw new Exception($"Medical facility with ID {id} not found.");
                }
                return facility;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Schedule>> GetSchedules(int doctorId, DateOnly selectedDate)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Include(s => s.Doctor)
                    .Include(s => s.ScheduleSlots)
                        .ThenInclude(ss => ss.Slot)
                    .Where(s => s.DoctorId == doctorId && s.ScheduleDate == selectedDate)
                    .Select(s => new Schedule
                    {
                        ScheduleId = s.ScheduleId,
                        DoctorId = s.DoctorId,
                        ScheduleDate = s.ScheduleDate,
                        Doctor = s.Doctor,
                        ScheduleSlots = s.ScheduleSlots
                            .Where(ss => (bool)!ss.IsBooked)
                            .ToList()
                    })
                    .ToListAsync();

                return schedules;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching schedules: {ex.Message}");
            }
        }
        public async Task<Appointment> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.Facility)
                    .Include(a => a.ScheduleSlot)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);
                if (appointment == null) return new Appointment();
                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }
    }
}
