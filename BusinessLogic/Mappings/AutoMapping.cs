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
    }
}