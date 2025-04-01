using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace Application.Pages.Schedules
{
    public class AddModel : PageModel
    {
        private readonly IScheduleService _scheduleService;
        public AddModel(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        [BindProperty]
        public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        [BindProperty]
        public List<int> SelectedSlots { get; set; } = new();
        public List<SlotModel> Slots { get; set; } = new();
        public string Message { get; set; } = "";
        public async Task<IActionResult> OnGetAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Index");

            if (role != "3")
                return RedirectToPage("/Index");

            await LoadSlots(DateOnly.FromDateTime(DateTime.Now));

            return Page();
        }
        private async Task LoadSlots(DateOnly date)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var doctorSlots = await _scheduleService.GetDoctorSlotsAsync(email, date);
                var allSlots = await _scheduleService.GetSlotsAsync();
                Slots = allSlots.Where(s => !doctorSlots.Any(ds => ds.SlotId == s.SlotId)).ToList();
            }
            catch (Exception ex)
            {
                Message = $"Error loading slots: {ex.Message}";
                Slots = new List<SlotModel>();
            }
        }
        public async Task<IActionResult> OnGetSlotsAsync(DateOnly selectedDate)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            var doctorSlots = await _scheduleService.GetDoctorSlotsAsync(email, selectedDate);
            var allSlots = await _scheduleService.GetSlotsAsync();
            var availableSlots = allSlots
                .Where(s => !doctorSlots.Any(ds => ds.SlotId == s.SlotId))
                .Select(s => new { s.SlotId, StartTime = s.StartTime.ToString("hh:mm tt"), EndTime = s.EndTime.ToString("hh:mm tt") })
                .ToList();

            return new JsonResult(new { success = true, slots = availableSlots });
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Index");
            if (SelectedSlots.Count == 0)
            {
                Message = "Please select at least one slot.";
                await LoadSlots(SelectedDate);
                return Page();
            }

            if (SelectedDate < DateOnly.FromDateTime(DateTime.Now))
            {
                Message = "Selected date is in the past.";
                await LoadSlots(SelectedDate);
                return Page();
            }
           
            var response = await _scheduleService.AddScheduleAsync(email, SelectedDate, SelectedSlots);

            Message = response;

            await LoadSlots(SelectedDate);
            return Page();
        }
    }
}
