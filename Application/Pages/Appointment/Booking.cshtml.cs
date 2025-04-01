using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace Application.Pages.Appointment
{
    public class BookingModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;

        public BookingModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [BindProperty(SupportsGet = true)]
        public string SelectedLocation { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedDoctor { get; set; }

        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Doctors { get; set; }
        public string DoctorsJson { get; set; }
        public string FacilitiesJson { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var facilities = await _appointmentService.GetMedicalFacilitiesAsync();
            var doctors = await _appointmentService.GetDoctorsAsync();

            var selectedFacility = facilities.FirstOrDefault(f => f.FacilityId.ToString() == SelectedLocation);

            var selectedDoctor = doctors.FirstOrDefault(d => d.DoctorId.ToString() == SelectedDoctor);

            if (selectedFacility != null && selectedDoctor != null)
            {
                var appointmentData = new
                {
                    Facility = selectedFacility,
                    DoctorSchedule = new
                    {
                        DoctorId = selectedDoctor.DoctorId,
                        DoctorName = selectedDoctor.DoctorName,
                        Schedule = selectedDoctor.Availability ?? "Not specified"
                    }
                };

                return RedirectToPage("Confirmation", new
                {
                    facilityData = JsonSerializer.Serialize(appointmentData)
                });
            }

            await LoadDataAsync();
            return Page();
        }

        private async Task LoadDataAsync()
        {
            var facilities = await _appointmentService.GetMedicalFacilitiesAsync();
            var doctors = await _appointmentService.GetDoctorsAsync();

            Locations = facilities.Select(f => new SelectListItem
            {
                Value = f.FacilityId.ToString(),
                Text = f.Name,
                Selected = f.FacilityId.ToString() == SelectedLocation
            }).ToList();

            Doctors = doctors.Select(d => new SelectListItem
            {
                Value = d.DoctorId.ToString(), // Sử dụng ID làm giá trị
                Text = d.DoctorName,             // Hiển thị tên bác sĩ
                Selected = d.DoctorId.ToString() == SelectedDoctor  // So sánh với query string
            }).ToList();



            var doctorDetails = doctors.Select(d => new
            {
                DoctorId = d.DoctorId.ToString(),
                DoctorName = d.DoctorName,
                Qualification = d.Qualification,
                Specialty = d.Specialty,
                ExperienceYears = d.ExperienceYears,
                ConsultationFee = d.ConsultationFee,
                Availability = d.Availability ?? "Not specified",
                FacilityIds = d.Facilities.Select(f => f.FacilityId.ToString()).ToList()

            }).ToList();

            var facilityDetails = facilities.Select(f => new
            {
                FacilityId = f.FacilityId.ToString(),
                Name = f.Name,
                Address = f.Address
            }).ToList();

            DoctorsJson = JsonSerializer.Serialize(doctorDetails);
            FacilitiesJson = JsonSerializer.Serialize(facilityDetails);
        }
    }
}
