using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Application.Pages.Schedules
{
    [IgnoreAntiforgeryToken]
    public class ScheduleModel : PageModel
    {
        private readonly IScheduleService _scheduleService;

        [BindProperty(SupportsGet = true)]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [BindProperty(SupportsGet = true)]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        public ScheduleModel(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        public async Task<IActionResult> OnGetGetSchedulesAsync(string doctorEmail, DateOnly startDate, DateOnly endDate)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var schedules = await _scheduleService.GetSchedulesFromRangeAsync(email, startDate, endDate) ?? new List<ScheduleViewModel>();

            var calendarEvents = schedules.Select(schedule => new
            {
                title = $"Doctor {schedule.DoctorId}",
                start = schedule.ScheduleDate.ToString("yyyy-MM-dd"),
                extendedProps = new
                {
                    slots = schedule.ScheduleSlots.Select(s => new
                    {
                        scheduleId = s.ScheduleId,
                        slotId = s.SlotId,
                        startTime = s.StartTime.ToString("HH:mm"),
                        endTime = s.EndTime.ToString("HH:mm"),
                        status = (bool)s.IsBooked ? "Booked" : "Available"
                    }).ToList()
                }
            }).ToList();

            return new JsonResult(calendarEvents);
        }
        public async Task<IActionResult> OnDeleteDeleteSlotAsync(int scheduleId, int slotId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var success = await _scheduleService.DeleteSlotAsync(scheduleId, slotId);
            if (!success) return BadRequest("Slot not found or deletion failed");
            return new OkResult();
        }
    }
}

