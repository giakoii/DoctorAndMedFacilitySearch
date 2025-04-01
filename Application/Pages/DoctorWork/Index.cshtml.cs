using AutoMapper;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
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

        public IndexModel(IDoctorAppointmentsService appointmentService, IUserService userService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _mapper = mapper;
        }

        // Danh sách các appointment hiển thị trong view
        public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();

        public string Message { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

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
            // Lấy tất cả các appointment của bác sĩ theo doctorId từ service
            var vwAppointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            var filteredAppointments = vwAppointments
                .Where(x => x.Status == ConstantEnum.AppointmentStatus.Pending.ToString())
                .OrderByDescending(a => a.AppointmentCreatedAt)
                .ToList();

            // Tính totalCount và totalPages dựa trên danh sách đã lọc
            int totalCount = filteredAppointments.Count;
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
