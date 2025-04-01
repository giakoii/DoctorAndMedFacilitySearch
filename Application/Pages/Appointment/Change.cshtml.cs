using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Pages.Appointment
{
    public class ChangeModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        public ChangeModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }
        public AppointmentViewModel Appointment { get; set; }
        public List<SelectListItem> AvailableDoctors { get; set; }
        [BindProperty]
        public int SelectedDoctorId { get; set; }

        [BindProperty]
        public int SelectedScheduleId { get; set; }

        [BindProperty]
        public int SelectedSlotId { get; set; }
        [BindProperty]
        public int SelectedAppointmentId { get; set; }
        public async Task OnGetAsync(int id)
        {
            Appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            SelectedAppointmentId = id;
            var doctors = await _appointmentService.GetDoctorsAsync();

            AvailableDoctors = doctors.Select(d => new SelectListItem
            {
                Value = d.DoctorId.ToString(),
                Text = d.DoctorName
            }).ToList();
        }
        public async Task<JsonResult> OnGetDoctorSchedulesAsync(int doctorId)
        {
            
            var schedules = await _appointmentService.GetDoctorSchedulesAsync(doctorId);
            return new JsonResult(schedules);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedDoctorId == 0 || SelectedSlotId == 0 || SelectedScheduleId == 0 || SelectedAppointmentId == 0)
            {
                ModelState.AddModelError("", "Please select a doctor, a valid time slot, and ensure the appointment is valid.");
                return Page();
            }

            var response = await _appointmentService.ChangeAppointmentScheduleAsync(
                SelectedAppointmentId, SelectedDoctorId, SelectedScheduleId, SelectedSlotId);

            TempData["AppointmentMessage"] = response;

            return RedirectToPage("/Appointment/Change", new { id = SelectedAppointmentId });
        }
    }
}
