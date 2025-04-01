using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using DataAccessObject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<string> AddScheduleAsync(string doctorEmail, DateOnly selectedDate, List<int> slots)
        {
            try
            {
                return await _scheduleRepository.AddSchedule(doctorEmail, selectedDate, slots);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"{ex.Message}");
            }
        }

        public async Task<bool> DeleteSlotAsync(int scheduleId, int slotId)
        {
            try
            {
                return await _scheduleRepository.DeleteSlot(scheduleId, slotId);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<List<SlotModel>> GetDoctorSlotsAsync(string doctorEmail, DateOnly date)
        {
            try
            {
                var slots = await _scheduleRepository.GetDoctorSlots(doctorEmail, date);
                var slotModels = slots.Select(slot => new SlotModel
                {
                    SlotId = slot.SlotId,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime
                }).ToList();
                return slotModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ScheduleViewModel>> GetSchedulesFromRangeAsync(string doctorEmail, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var schedules = await _scheduleRepository.GetSchedulesFromRange(doctorEmail, startDate, endDate);

                var scheduleViewModels = schedules.Select(schedule => new ScheduleViewModel
                {
                    ScheduleId = schedule.ScheduleId,
                    ScheduleDate = schedule.ScheduleDate,
                    DoctorId = schedule.DoctorId,
                    ScheduleSlots = schedule.ScheduleSlots.Select(ss => new ScheduleSlotViewModel
                    {
                        SlotId = ss.SlotId,
                        ScheduleId = ss.ScheduleId,
                        IsBooked = ss.IsBooked,
                        StartTime = ss.Slot.StartTime,
                        EndTime = ss.Slot.EndTime
                    }).ToList()
                }).ToList();


                return scheduleViewModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<SlotModel>> GetSlotsAsync()
        {
            try
            {
                var slotModels = new List<SlotModel>();
                var slots = await _scheduleRepository.GetSlots();
                foreach (var slot in slots)
                {
                    var addSlot = new SlotModel
                    {
                        SlotId = slot.SlotId,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime
                    };
                    slotModels.Add(addSlot);
                }
                return slotModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
