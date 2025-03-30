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
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
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
    }
}
