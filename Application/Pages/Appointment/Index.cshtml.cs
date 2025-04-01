using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Application.Pages.Appointment
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;

        public IndexModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }
        public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
        public string Message { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public async Task<IActionResult> OnGetAsync(int? num = 1)
        {
            await LoadAppointment(num);
            return Page();
        }
        private async Task LoadAppointment(int? num)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var totalCount = await _appointmentService.GetAppointmentsCount(email);
            CurrentPage = num ?? 1;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            var medicines = await _appointmentService.GetAppointmentsViewModel(email, CurrentPage, PageSize);
            Appointments = (medicines?.OrderByDescending(a => a.CreatedAt).ToList())
               ?? new List<AppointmentViewModel>();
        }
        public async Task<IActionResult> OnPostCancelAsync(int id, string cancelNotes)
        {
            if (string.IsNullOrEmpty(cancelNotes))
            {
                Message = "Please provide a cancellation reason.";
                return Page();
            }

            try
            {
                await _appointmentService.UpdateAppointmentAsync(id, cancelNotes);
                Message = $"Appointment {id} has been cancelled successfully.";
            }
            catch (Exception ex)
            {
                Message = $"Failed to cancel appointment: {ex.Message}";
                return Page();
            }

            return RedirectToPage();
        }
    }
}