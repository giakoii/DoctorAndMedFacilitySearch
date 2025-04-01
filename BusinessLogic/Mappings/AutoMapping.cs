using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Mappings;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<MedicalFacility, MedicalFacilitiesViewModel>()
            .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
            .ReverseMap();
        CreateMap<DoctorSchedule, DoctorScheduleViewModel>().ReverseMap();
        CreateMap<DoctorProfile, DoctorViewModel>();

        CreateMap<DoctorViewModel, DoctorProfile>();
        
        CreateMap<PatientProfile, PatientProfileViewModel>();

        CreateMap<PatientProfileViewModel, PatientProfile>();

        CreateMap<VwDoctorProfile, DoctorViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
        
        CreateMap<VwPatientProfile, PatientProfileViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<DoctorProfile, DoctorViewModel>();
        CreateMap<DoctorProfile, DoctorProfilesViewModel>()
             .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
             .ForMember(dest => dest.Facilities, opt => opt.Ignore());

        CreateMap<DoctorProfilesViewModel, DoctorProfile>()
            .ForPath(dest => dest.Doctor.FullName, opt => opt.MapFrom(src => src.DoctorName))
            .ForMember(dest => dest.Facilities, opt => opt.Ignore());
        CreateMap<VwFacilityReviewsDetail, ReviewViewModel>().ReverseMap();
        CreateMap<ReviewViewModel, Review>().ReverseMap();


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