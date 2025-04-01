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
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;
        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> AddSchedule(string doctorEmail, DateOnly selectedDate, List<int> slots)
        {
            try
            {
                if (string.IsNullOrEmpty(doctorEmail))
                    return await Task.FromResult("Invalid Doctor Email.");

                if (selectedDate == default)
                    return await Task.FromResult("Invalid Date.");

                if (slots == null || slots.Count == 0)
                    return await Task.FromResult("No slots selected.");

                var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Email == doctorEmail);

                if (doctor == null)
                    return await Task.FromResult("Doctor not found.");

                if (doctor.RoleId != 3)
                    return await Task.FromResult("User is not a doctor.");

                if (selectedDate.ToDateTime(TimeOnly.MinValue) < DateTime.Now.Date)
                    return await Task.FromResult("Selected date is in the past.");

                var existingSchedule = await _context.Schedules.FirstOrDefaultAsync(s => s.DoctorId == doctor.UserId && s.ScheduleDate == selectedDate);
                if (existingSchedule != null)
                {
                    var existingScheduleSlotIds = await _context.ScheduleSlots.Where(ss => ss.ScheduleId == existingSchedule.ScheduleId).Select(ss => ss.SlotId).ToListAsync();
                    foreach (var slotId in slots)
                    {
                        if (existingScheduleSlotIds.Contains(slotId))
                            return await Task.FromResult("Slot already exists in the schedule.");
                    }
                }

                var newSchedule = new Schedule
                {
                    DoctorId = doctor.UserId,
                    ScheduleDate = selectedDate
                };

                await _context.Schedules.AddAsync(newSchedule);
                await _context.SaveChangesAsync();

                foreach (var slotId in slots)
                {
                    var slot = await _context.Slots.FirstOrDefaultAsync(s => s.SlotId == slotId);
                    if (slot == null)
                        return await Task.FromResult("Invalid slot ID.");
                    var scheduleSlot = new ScheduleSlot
                    {
                        ScheduleId = newSchedule.ScheduleId,
                        SlotId = slotId,
                        IsBooked = false
                    };
                    await _context.ScheduleSlots.AddAsync(scheduleSlot);
                }

                await _context.SaveChangesAsync();

                return await Task.FromResult("Schedule added successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteSlot(int scheduleId, int slotId)
        {
            try
            {
                var schedule = await _context.ScheduleSlots.FirstOrDefaultAsync(ss => ss.ScheduleId == scheduleId && ss.SlotId == slotId);
                if (schedule == null)
                    return await Task.FromResult(false);
                _context.ScheduleSlots.Remove(schedule);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<List<Slot>> GetDoctorSlots(string doctorEmail, DateOnly date)
        {
            try
            {
                var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Email == doctorEmail);
                if (doctor == null)
                    return await Task.FromResult(new List<Slot>());
                var doctorSlots = await _context.Schedules
                    .Include(s => s.ScheduleSlots)
                    .ThenInclude(ss => ss.Slot)
                    .Where(s => s.DoctorId == doctor.UserId && s.ScheduleDate == date)
                    .SelectMany(s => s.ScheduleSlots)
                    .Select(ss => ss.Slot)
                    .ToListAsync();
                return doctorSlots;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Schedule>> GetSchedulesFromRange(string doctorEmail, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                return await _context.Schedules
                    .Include(s => s.ScheduleSlots)
                    .ThenInclude(ss => ss.Slot)
                    .Include(s => s.Doctor)
                    .Where(s => s.Doctor.Email == doctorEmail && s.ScheduleDate >= startDate && s.ScheduleDate <= endDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Slot>> GetSlots()
        {
            try
            {
                var slots = await _context.Slots.ToListAsync();
                return slots;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
