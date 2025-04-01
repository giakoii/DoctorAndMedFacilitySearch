using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IScheduleService
    {
        Task<string> AddScheduleAsync(string doctorEmail, DateOnly selectedDate, List<int> slots);
        Task<bool> DeleteSlotAsync(int scheduleId, int slotId);
        Task<List<SlotModel>> GetSlotsAsync();
        Task<List<ScheduleViewModel>> GetSchedulesFromRangeAsync(string doctorEmail, DateOnly startDate, DateOnly endDate);
        Task<List<SlotModel>> GetDoctorSlotsAsync(string doctorEmail, DateOnly date);
    }
}
