using AutoMapper;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.DetailCare
{
    public class DoctorModel : PageModel
    {
        private readonly IDoctorProfileService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public DoctorModel(IDoctorProfileService doctorService, IAppointmentService appointmentService, IMapper mapper)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        public DoctorProfilesViewModel Doctor { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorProfileVm = await _doctorService.GetDoctorsById(id.Value);

            if (doctorProfileVm == null)
            {
                return NotFound();
            }

            Doctor = _mapper.Map<DoctorProfilesViewModel>(doctorProfileVm);

            return Page();
        }

        public async Task<JsonResult> OnGetDoctorSchedulesAsync(int doctorId)
        {

            var schedules = await _appointmentService.GetDoctorSchedulesAsync(doctorId);
            return new JsonResult(schedules);
        }
    }
}
