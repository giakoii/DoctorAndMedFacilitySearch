using BusinessLogic.Logics.MomoLogics;
using BusinessLogic.ViewModels;
using Client.Logics.Commons.MomoLogics;
using DataAccessObject;
using DataAccessObject.Models;
using DataAccessObject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Appointment;

public class AppointmentService : BaseService<DataAccessObject.Models.Appointment, int, VwAppointment>, IAppointmentService
{
    private readonly IBaseRepository<Schedule, int, VwDoctorSchedule> _scheduleRepository;
    private readonly IBaseRepository<MedicalFacility, int, VwMedicalFacility> _medicalFacilityRepository;
    private readonly IUserService _userService;
    private readonly IMomoService _momoLogic;
    private readonly IAppointmentRepository _appointmentRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="scheduleRepository"></param>
    /// <param name="userService"></param>
    /// <param name="medicalFacilityRepository"></param>
    /// <param name="momoLogic"></param>
    public AppointmentService(IBaseRepository<DataAccessObject.Models.Appointment, int, VwAppointment> repository,
        IBaseRepository<Schedule, int, VwDoctorSchedule> scheduleRepository, IUserService userService,
        IBaseRepository<MedicalFacility, int, VwMedicalFacility> medicalFacilityRepository, IMomoService momoLogic, IAppointmentRepository appointmentRepository) : base(repository)
    {
        _scheduleRepository = scheduleRepository;
        _userService = userService;
        _medicalFacilityRepository = medicalFacilityRepository;
        _momoLogic = momoLogic;
        _appointmentRepository = appointmentRepository;
    }

    /// <summary>
    /// Get start time from slot id
    /// </summary>
    /// <param name="email"></param>
    /// <param name="doctorId"></param>
    /// <param name="selectedDate"></param>
    /// <param name="slotId"></param>
    /// <param name="facilityId"></param>
    /// <returns></returns>
    public async Task<string> CreateAppointment(string email, int doctorId, DateOnly selectedDate, int slotId,
        int facilityId)
    {
        string momoPaymentUrl = string.Empty;
        // Check if patient exists
        var patient = await _userService.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (patient == null)
            return "Patient not found.";

        // Check if doctor exists
        var doctor = await _userService.Find(u => u.UserId == doctorId, true, x => x.DoctorProfile).FirstOrDefaultAsync();
        if (doctor == null)
            return "Doctor not found.";

        // Check if doctor has schedule for selected date
        var schedule = await _scheduleRepository
            .Find(s => s.DoctorId == doctorId && s.ScheduleDate == selectedDate,
                true,
                s => s.ScheduleSlots)
            .FirstOrDefaultAsync();

        if (schedule == null)
            return "Schedule not found.";

        // Check if slot is already booked
        var existingAppointment = await _repository.Find(a => a.ScheduleId == schedule.ScheduleId && a.SlotId == slotId)
            .FirstOrDefaultAsync();

        if (existingAppointment != null)
            return "Slot already booked.";

        // Check if facility exists
        var facility = await _medicalFacilityRepository.Find(mf => mf.FacilityId == facilityId).FirstOrDefaultAsync();
        if (facility == null) return "Facility not found.";

        // Check if slot exists in schedule
        var slotToBook = schedule.ScheduleSlots.FirstOrDefault(s => s.SlotId == slotId && s.ScheduleId == schedule.ScheduleId);
        if (slotToBook == null) return "Slot not found in schedule.";

        await _repository.ExecuteInTransactionAsync(async () =>
        {
            var appointment = new DataAccessObject.Models.Appointment
            {
                PatientId = patient.UserId,
                DoctorId = doctorId,
                ScheduleId = schedule.ScheduleId,
                SlotId = slotToBook.SlotId,
                FacilityId = facility.FacilityId,
                AppointmentDate = DateTime.Parse(selectedDate.ToString() + " " + GetStartTimeFromSlotId(slotId)),
                Status = ConstantEnum.AppointmentStatus.Pending.ToString(),
                PaymentStatus = "Paid",
                Notes = null,
            };
            slotToBook.IsBooked = true;
            _repository.Add(appointment);
            _repository.SaveChanges(email);

            var momoExcuteResponseModel = new MomoExecuteResponseModel
            {
                FullName = $"{patient.FullName}",
                Amount = ((Int64)doctor.DoctorProfile.ConsultationFee).ToString(),
                OrderId = $"{appointment.AppointmentId}-{Guid.NewGuid().ToString()}",
                OrderInfo = $"{patient.FullName} Payment for AppointmentID {appointment.AppointmentId}",
            };

            var momoPayment = await _momoLogic.CreatePaymentOrderAsync(momoExcuteResponseModel);

            if (momoPayment.ErrorCode != (byte)ConstantEnum.PaymentStatus.Success ||
                string.IsNullOrEmpty(momoPayment.PayUrl) ||
                string.IsNullOrEmpty(momoPayment.QrCodeUrl) ||
                string.IsNullOrEmpty(momoPayment?.DeeplinkWebInApp) ||
                string.IsNullOrEmpty(momoPayment?.Deeplink))
            {
                return false;
            }

            momoPaymentUrl = momoPayment.PayUrl;
            return true;
        });
        return momoPaymentUrl;
    }

    public async Task<List<ScheduleViewModel>> GetSchedulesAsync(int doctorId, DateOnly selectedDate)
    {
        var schedules = await _appointmentRepository.GetSchedules(doctorId, selectedDate);

        var list = schedules.Select(s => new ScheduleViewModel
        {
            ScheduleId = s.ScheduleId,
            DoctorId = s.DoctorId,
            ScheduleDate = s.ScheduleDate,
            ScheduleSlots = s.ScheduleSlots.Select(ss => new ScheduleSlotViewModel
            {
                SlotId = ss.SlotId,
                StartTime = ss.Slot.StartTime,
                EndTime = ss.Slot.EndTime
            }).ToList()
        }).ToList();

        return list;
    }


    private string GetStartTimeFromSlotId(int slotId)
    {
        var slotMap = new Dictionary<int, string>
        {
            { 1, "07:00" }, { 2, "07:30" }, { 3, "08:00" }, { 4, "08:30" },
            { 5, "09:00" }, { 6, "09:30" }, { 7, "10:00" }, { 8, "10:30" },
            { 9, "11:00" }, { 10, "11:30" }, { 11, "12:00" }, { 12, "12:30" },
            { 13, "13:00" }, { 14, "13:30" }, { 15, "14:00" }, { 16, "14:30" },
            { 17, "15:00" }, { 18, "15:30" }, { 19, "16:00" }, { 20, "16:30" }
        };

        return slotMap.TryGetValue(slotId, out string startTime) ? startTime : null;
    }
    public async Task<List<DataAccessObject.Models.Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = await _repository
        .Find(a => a.DoctorId == doctorId, false, a => a.Doctor, a => a.Patient, a => a.Facility)
        .ToListAsync();
        return appointments;
    }

}