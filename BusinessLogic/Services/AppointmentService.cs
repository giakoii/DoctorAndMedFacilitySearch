using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using DataAccessObject.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Availability = dp.Availability
                })
                .ToList();

                return doctors;
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
                existingAppointment.Notes = notes;
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
