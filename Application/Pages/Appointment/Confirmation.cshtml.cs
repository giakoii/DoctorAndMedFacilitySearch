using System.Security.Claims;
using System.Text.Json;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.Appointment
{
    public class ConfirmationModel : PageModel
    {
        private readonly BusinessLogic.Services.Appointment.IAppointmentService _appointmentService;

        public ConfirmationModel(BusinessLogic.Services.Appointment.IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        [BindProperty(SupportsGet = true)]
        public FacilityModel Facility { get; set; }

        public DoctorScheduleModel DoctorSchedule { get; set; }

        [BindProperty]
        public string SelectedDate { get; set; }

        [BindProperty]
        public string SelectedTime { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentMonth { get; set; } = DateTime.Now.Month;

        [BindProperty(SupportsGet = true)]
        public int CurrentYear { get; set; } = DateTime.Now.Year;

        [BindProperty(SupportsGet = true)]
        public string FacilityData { get; set; }

        public List<ScheduleSlotModel> AvailableSlots { get; set; } = new List<ScheduleSlotModel>();

        public Dictionary<string, List<ScheduleSlotModel>> AvailableSchedules { get; set; } = new Dictionary<string, List<ScheduleSlotModel>>();

        public async Task<IActionResult> OnGetAsync(string facilityData, int? currentMonth, int? currentYear, string selectedDate)
        {
            FacilityData = facilityData;

            if (string.IsNullOrEmpty(FacilityData))
            {
                return RedirectToPage("Booking");
            }

            CurrentMonth = currentMonth.HasValue && currentMonth.Value >= 1 && currentMonth.Value <= 12
                ? currentMonth.Value
                : DateTime.Now.Month;
            CurrentYear = currentYear.HasValue && currentYear.Value >= 1 && currentYear.Value <= 9999
                ? currentYear.Value
                : DateTime.Now.Year;

            try
            {
                var appointmentData = JsonSerializer.Deserialize<AppointmentDataModel>(FacilityData);
                Facility = appointmentData?.Facility;
                DoctorSchedule = appointmentData?.DoctorSchedule;

                if (DoctorSchedule == null || DoctorSchedule.DoctorId == 0)
                {
                    ModelState.AddModelError("", "Doctor information is missing or invalid.");
                    return Page();
                }
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError("", "Invalid facility data provided.");
                return Page();
            }

            await LoadAllSchedulesAsync();

            if (!string.IsNullOrEmpty(selectedDate) && AvailableSchedules.ContainsKey(selectedDate))
            {
                SelectedDate = selectedDate;
            }
            else if (AvailableSchedules.Any())
            {
                SelectedDate = AvailableSchedules.Keys.First();
            }
            else
            {
                SelectedDate = null;
            }

            if (!string.IsNullOrEmpty(SelectedDate) && AvailableSchedules.ContainsKey(SelectedDate))
            {
                AvailableSlots = AvailableSchedules[SelectedDate];
            }

            return Page();
        }
        public async Task<IActionResult> OnPostBookAppointmentAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                DateOnly appointmentDate = DateOnly.ParseExact(SelectedDate, "yyyy-MM-dd");

                int slotId = GetSlotIdFromStartTime(SelectedTime);

                if (slotId == 0)
                {
                    ModelState.AddModelError("", "Invalid time slot selected");
                    return Page();
                }

                string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

                if (!int.TryParse(FacilityData, out int facilityId))
                {
                    ModelState.AddModelError("", "Invalid facility selected");
                    return Page();
                }

                int doctorId = DoctorSchedule?.DoctorId ?? 0;

                if (doctorId == 0)
                {
                    ModelState.AddModelError("", "Invalid doctor selection");
                    return Page();
                }

                string result = await _appointmentService.CreateAppointment(userEmail, doctorId, appointmentDate, slotId, facilityId);
                
                if (result.StartsWith("Error:"))
                {
                    ModelState.AddModelError("", result);
                    return Page();
                }

                TempData["SuccessMessage"] = "Appointment booked successfully!";
                return RedirectToPage("/Appointment/Success", new { bookingId = result });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while booking the appointment: {ex.Message}");
                return Page();
            }
        }
        private async Task LoadAllSchedulesAsync()
        {
            if (DoctorSchedule == null || DoctorSchedule.DoctorId == 0)
            {
                return;
            }

            AvailableSchedules.Clear();
            int daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(CurrentYear, CurrentMonth, day);
                var schedules = await _appointmentService.GetSchedulesAsync(DoctorSchedule.DoctorId, date);

                if (schedules == null || !schedules.Any())
                {
                    continue;
                }

                var slots = schedules
                    .SelectMany(s => s.ScheduleSlots.Select(slot => new ScheduleSlotModel
                    {
                        ScheduleId = s.ScheduleId,
                        SlotId = slot.SlotId,
                        StartTime = slot.StartTime.ToString("HH:mm"),
                        EndTime = slot.EndTime.ToString("HH:mm")
                    }))
                    .ToList();

                if (slots.Any())
                {
                    AvailableSchedules[date.ToString("yyyy-MM-dd")] = slots;
                }
            }
        }
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
        public IActionResult OnPostPreviousMonth(string facilityData)
        {
            FacilityData = facilityData;
            CurrentMonth--;
            if (CurrentMonth < 1)
            {
                CurrentMonth = 12;
                CurrentYear--;
                if (CurrentYear < 1)
                {
                    CurrentYear = 1;
                    CurrentMonth = 1;
                }
            }
            return RedirectToPage(new
            {
                facilityData = FacilityData,
                currentMonth = CurrentMonth,
                currentYear = CurrentYear
            });
        }
        public IActionResult OnPostNextMonth(string facilityData)
        {
            FacilityData = facilityData;
            CurrentMonth++;
            if (CurrentMonth > 12)
            {
                CurrentMonth = 1;
                CurrentYear++;
                if (CurrentYear > 9999)
                {
                    CurrentYear = 9999;
                    CurrentMonth = 12;
                }
            }
            return RedirectToPage(new
            {
                facilityData = FacilityData,
                currentMonth = CurrentMonth,
                currentYear = CurrentYear
            });
        }
        public class AppointmentDataModel
        {
            public FacilityModel Facility { get; set; }
            public DoctorScheduleModel DoctorSchedule { get; set; }
        }

        public class ScheduleSlotModel
        {
            public int ScheduleId { get; set; }
            public int SlotId { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }

        public class FacilityModel
        {
            public int FacilityId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
        }

        public class DoctorScheduleModel
        {
            public int DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Schedule { get; set; }
        }
    }
}