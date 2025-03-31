using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class DoctorProfileService : BaseService<DoctorProfile, int, VwDoctorProfile>, IDoctorProfileService
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public DoctorProfileService(IBaseRepository<DoctorProfile, int, VwDoctorProfile> repository, IMapper mapper, IUserService userService) : base(repository)
    {
        _mapper = mapper;
        _userService = userService;
    }

    public void AddDoctorProfile(DoctorViewModel doctorProfile, string email)
    {
        _repository.ExecuteInTransaction(() =>
        {
            var doctorId = _userService.Find(x => x.Email == email, false).FirstOrDefault().UserId;

            var newDoctor = _mapper.Map<DoctorProfile>(doctorProfile);
            newDoctor.DoctorId = doctorId;

            _repository.Add(newDoctor);
            _repository.SaveChanges(email);
            return true;
        });
    }
}