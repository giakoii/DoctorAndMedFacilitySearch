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
        CreateMap<DoctorViewModel, DoctorProfile>();
        CreateMap<DoctorProfile, DoctorViewModel>();
        CreateMap<DoctorProfile, DoctorProfilesViewModel>()
             .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
             .ForMember(dest => dest.Facilities, opt => opt.Ignore());

        CreateMap<DoctorProfilesViewModel, DoctorProfile>()
            .ForPath(dest => dest.Doctor.FullName, opt => opt.MapFrom(src => src.DoctorName))
            .ForMember(dest => dest.Facilities, opt => opt.Ignore());
        CreateMap<VwFacilityReviewsDetail, ReviewViewModel>().ReverseMap();
        CreateMap<ReviewViewModel, Review>().ReverseMap();

    }
}