using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
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
            Appointments = medicines ?? new List<AppointmentViewModel>();
        }
    }
}