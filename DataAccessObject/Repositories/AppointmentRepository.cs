using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
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

                var existingAppointment = await _context.Appointments.FirstOrDefaultAsync(a => a.ScheduleId == schedule.ScheduleId && a.SlotId == slotId);
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
                    AppointmentDate = DateTime.UtcNow,
                    Status = "Confirmed",
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
                return await _context.DoctorProfiles.Include(d => d.Doctor).ToListAsync();
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

    }
}
