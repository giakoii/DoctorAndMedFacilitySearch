using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

            Slots = await _scheduleService.GetSlotsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Index");
            var response = await _scheduleService.AddScheduleAsync(email, SelectedDate, SelectedSlots);
            Message = response;
            Slots = await _scheduleService.GetSlotsAsync();
            return Page();
        }
    }
}
