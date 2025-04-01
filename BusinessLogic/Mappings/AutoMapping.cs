using Application.ViewModels;
using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Mappings;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<DoctorSchedule, DoctorScheduleViewModel>().ReverseMap();
        CreateMap<DoctorViewModel, DoctorProfile>();
        CreateMap<DoctorProfile, DoctorViewModel>();

        /*CreateMap<PatientProfile, PatientProfileViewModel>()
                .ReverseMap();

        CreateMap<Appointment, AppointmentViewModel>()
            .ReverseMap();*/
        CreateMap<PatientProfile, PatientProfileViewModel>()
            .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments.Where(a => a.IsActive)))
            .ReverseMap();

        CreateMap<MedicalFacility, MedicalFacilitiesViewModel>();

        // C?u hình mapping cho Appointment
        CreateMap<Appointment, AppointmentViewModel>()
    .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Facility != null ? src.Facility.Name : "N/A"))
    .ForMember(dest => dest.Facility, opt => opt.Ignore());

        CreateMap<MedicalFile, MedicalFileViewModel>()
            .ReverseMap();

    }
}