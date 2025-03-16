using Application.ViewModels;
using AutoMapper;
using DataAccessObject.Models;

namespace BusinessLogic.Mappings;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
    }
}