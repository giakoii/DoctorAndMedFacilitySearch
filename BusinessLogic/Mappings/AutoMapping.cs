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
        
        CreateMap<VwAppointment, AppointmentViewModel>().ReverseMap();
        
        CreateMap<VwDoctorAppointment, AppointmentViewModel>().ReverseMap();
        CreateMap<MedicalHistory, MedicalHistoryViewModel>().ReverseMap();
        CreateMap<DoctorsWithoutFacilityViewModel, VwDoctorsWithoutFacility>();
        
        CreateMap<VwDoctorsWithoutFacility, DoctorsWithoutFacilityViewModel>();
        
        CreateMap<MedicalFacilitiesViewModel, VwMedicalFacility>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ReverseMap();    }
}