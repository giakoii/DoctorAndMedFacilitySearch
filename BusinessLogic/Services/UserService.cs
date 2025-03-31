using System.Linq.Expressions;
using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class UserService : BaseService<User, int, VwUser>, IUserService
{
    private readonly IRoleService _roleService;
    private readonly IBaseService<DoctorProfile, int, VwDoctorProfile> _doctorProfileService;
    private readonly IBaseService<PatientProfile, int, VwPatientProfile> _patientProfileService;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="roleService"></param>
    public UserService(IBaseRepository<User, int, VwUser> repository, IRoleService roleService, IBaseService<DoctorProfile, int, VwDoctorProfile> doctorProfileService, IBaseService<PatientProfile, int, VwPatientProfile> patientProfileService, IMapper mapper) : base(repository)
    {
        _roleService = roleService;
        _doctorProfileService = doctorProfileService;
        _patientProfileService = patientProfileService;
        _mapper = mapper;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="registerViewModel"></param>
    /// <returns></returns>
    public bool Register(RegisterViewModel registerViewModel)
    {
        return _repository.ExecuteInTransaction( () =>
        {
            // Check if user already exists
            var userExist = _repository
                .Find(x => x.Email == registerViewModel.Email, false)
                .FirstOrDefault();

            if (userExist != null)
            {
                return false;
            }

            // Get role
            var role =  _roleService.GetById(registerViewModel.RoleId);
            if (role == null)
            {
                return false;
            }

            // Add new user
            var newUser = new User
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                RoleId = registerViewModel.RoleId,
            };
            _repository.Add(newUser);
            SaveChanges(registerViewModel.Email);
            return true;
        });
    }

    public DoctorViewModel? GetDoctorProfile(string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        
        var doctorProfile = _doctorProfileService.Find(x => x.DoctorId == user.UserId).FirstOrDefault();
        if (doctorProfile == null)
        {
            return null;
        }

        return _mapper.Map<DoctorViewModel>(doctorProfile);
    }
    
    public PatientProfileViewModel? GetPatientProfile(string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        
        var patientProfile = _patientProfileService.Find(x => x.PatientId == user.UserId).FirstOrDefault();
        if (patientProfile == null)
        {
            return null;
        }

        return _mapper.Map<PatientProfileViewModel>(patientProfile);
    }
}