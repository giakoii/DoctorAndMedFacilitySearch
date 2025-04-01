using DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Repositories
{
    public interface IScheduleRepository
    {
        Task<string> AddSchedule(string doctorEmail, DateOnly selectedDate, List<int> slots);
        Task<bool> DeleteSlot(int scheduleId, int slotId);
        Task<List<Schedule>> GetSchedulesFromRange(string doctorEmail, DateOnly startDate, DateOnly endDate);
        Task<List<Slot>> GetSlots();
        Task<List<Slot>> GetDoctorSlots(string doctorEmail, DateOnly date);
    }
}
