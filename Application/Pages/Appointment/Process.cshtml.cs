using System.Globalization;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Appointment
{
    public class ProcessModel : PageModel
    {
        private readonly BusinessLogic.Services.Appointment.IAppointmentService _appointmentService;
        public ProcessModel(BusinessLogic.Services.Appointment.IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [BindProperty]
        public int FacilityId { get; set; }

        [BindProperty]
        public int DoctorId { get; set; }

        [BindProperty]
        public string SelectedDate { get; set; }

        [BindProperty]
        public string SelectedTime { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        public string ResultMessage { get; set; }

        private int GetSlotIdFromStartTime(string startTime)
        {
            var slotMap = new Dictionary<string, int>
            {
                {"07:00", 1}, {"07:30", 2}, {"08:00", 3}, {"08:30", 4},
                {"09:00", 5}, {"09:30", 6}, {"10:00", 7}, {"10:30", 8},
                {"11:00", 9}, {"11:30", 10}, {"12:00", 11}, {"12:30", 12},
                {"13:00", 13}, {"13:30", 14}, {"14:00", 15}, {"14:30", 16},
                {"15:00", 17}, {"15:30", 18}, {"16:00", 19}, {"16:30", 20}
            };

            return slotMap.TryGetValue(startTime, out int slotId) ? slotId : -1;
        }

        public async Task<IActionResult> OnGetAsync(int facilityId, string selectedDate, int doctorId, string selectedTime, string? email = null)
        {
            FacilityId = facilityId;
            SelectedDate = selectedDate;
            DoctorId = doctorId;
            SelectedTime = selectedTime;
            Email = email;

            if (FacilityId == 0 || DoctorId == 0 || string.IsNullOrEmpty(SelectedDate) || string.IsNullOrEmpty(SelectedTime))
            {
                ResultMessage = "Invalid appointment details.";
                return Page();
            }


            int slotId = GetSlotIdFromStartTime(SelectedTime);
            if (slotId == -1)
            {
                ResultMessage = "Invalid time slot.";
                return Page();
            }
            if (!DateOnly.TryParseExact(selectedDate, "yyyy-MM-dd", null, DateTimeStyles.None, out DateOnly parsedDate))
            {
                ResultMessage = "Invalid date format.";
                return Page();
            }

            ResultMessage = await _appointmentService.CreateAppointment(Email, DoctorId, parsedDate, slotId, FacilityId);
            
            return Redirect(ResultMessage);
        }
    }
}
