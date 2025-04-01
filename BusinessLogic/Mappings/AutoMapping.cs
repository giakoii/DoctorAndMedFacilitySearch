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
        CreateMap<DoctorProfile, DoctorViewModel>();

        CreateMap<DoctorViewModel, DoctorProfile>();

        CreateMap<VwDoctorProfile, DoctorViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
        
        CreateMap<VwPatientProfile, PatientProfileViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

    }
}