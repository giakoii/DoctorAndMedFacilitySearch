using AutoMapper;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Pages.DoctorWork
{
    public class IndexModel : PageModel
    {
        private readonly IDoctorAppointmentsService _appointmentService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IMedicalHistoryService _medicalHistoryService;

        public IndexModel(IDoctorAppointmentsService appointmentService, IUserService userService, IMapper mapper, IMedicalHistoryService medicalHistoryService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _mapper = mapper;
            _medicalHistoryService = medicalHistoryService;
        }



        // Danh sách các appointment hiển thị trong view
        public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }
        public string Message { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        [BindProperty]
        public MedicalHistoryViewModel MedicalHistory { get; set; } = new MedicalHistoryViewModel();
        public async Task<IActionResult> OnGetAsync(int? num = 1)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                Message = "You must login to view this page";
                return Page();
            }
            var userQuery = _userService.FindAsync(x => x.Email == email, false, x => x.DoctorProfile);
            var userObj = await userQuery.Result.FirstOrDefaultAsync();

            if (userObj == null || userObj.DoctorProfile == null)
            {
                Message = "Doctor profile not found for this user.";
                return Page();
            }

            await LoadAppointment(userObj.DoctorProfile.DoctorId, num);
            return Page();
        }

        private async Task LoadAppointment(int doctorId, int? num)
        {
            var vwAppointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            var filteredAppointments = vwAppointments
                .Where(x => x.Status == ConstantEnum.AppointmentStatus.Pending.ToString());

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.PatientName != null && a.PatientName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (SearchDate.HasValue)
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.AppointmentDate.Date == SearchDate.Value.Date);
            }

            filteredAppointments = filteredAppointments.OrderByDescending(a => a.AppointmentCreatedAt);

            int totalCount = filteredAppointments.Count();
            CurrentPage = num ?? 1;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var pagedAppointments = filteredAppointments
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Appointments = _mapper.Map<List<AppointmentViewModel>>(pagedAppointments);
        }
        public async Task<IActionResult> OnPostFinishAsync(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToPage();
            }
            var medicalHistoryEntity = _mapper.Map<MedicalHistory>(MedicalHistory);
            medicalHistoryEntity.PatientId = (int)appointment.PatientId;
            medicalHistoryEntity.RecordDate = MedicalHistory.RecordDate; // value từ form (datetime-local)
            medicalHistoryEntity.CreatedAt = DateTime.Now;
            medicalHistoryEntity.UpdatedAt = DateTime.Now;
            medicalHistoryEntity.CreatedBy = User.Identity?.Name ?? "system";
            medicalHistoryEntity.UpdatedBy = User.Identity?.Name ?? "system";
            medicalHistoryEntity.IsActive = true;
            await _medicalHistoryService.AddMedicalHistoryAsync(medicalHistoryEntity);
            bool updateResult = await _appointmentService.FinishAppointmentAsync(id);

            if (updateResult)
            {
                TempData["Debug"] = "Appointment has been confirmed successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update appointment status.";
            }
            return RedirectToPage();
        }

    }
}
