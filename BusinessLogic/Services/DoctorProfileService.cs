using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class DoctorProfileService : BaseService<DoctorProfile, int, VwDoctorProfile>, IDoctorProfileService
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="mapper"></param>
    public DoctorProfileService(IBaseRepository<DoctorProfile, int, VwDoctorProfile> repository, IMapper mapper, IUserService userService) : base(repository)
    {
        _mapper = mapper;
        _userService = userService;
    }
    
    /// <summary>
    /// Get doctor profile
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public DoctorViewModel? GetDoctorProfile(string email)
    {
        var doctorProfile = _repository.FindView(x => x.Email == email).FirstOrDefault();
        
        if (doctorProfile == null)
        {
            return null;
        }

        return _mapper.Map<DoctorViewModel>(doctorProfile);
    }
    
    /// <summary>
    /// Add doctor profile
    /// </summary>
    /// <param name="doctorProfile"></param>
    /// <param name="email"></param>
    public async Task AddDoctorProfile(DoctorViewModel doctorProfile, string email)
    {
        // Check if doctor already exists
        var doctor = await _userService.Find(x => x.UserId == doctorProfile.DoctorId, false).FirstOrDefaultAsync();
        if (doctor == null)
            return;
        
        // Add doctor profile
        await _repository.ExecuteInTransactionAsync(async () =>
        {
            var newDoctor = _mapper.Map<DoctorProfile>(doctorProfile);
            newDoctor.DoctorId = doctor.UserId;

            await _repository.AddAsync(newDoctor);
            await _repository.SaveChangesAsync(email);
            return true;
        });
    }

    /// <summary>
    /// Update doctor profile
    /// </summary>
    /// <param name="doctorProfile"></param>
    /// <param name="email"></param>
    public async Task UpdateDoctorProfile(DoctorViewModel doctorProfile, string email)
    {
        var doctorSelect = await _repository
            .Find(x => x.DoctorId == doctorProfile.DoctorId && x.IsActive == true, false)
            .FirstOrDefaultAsync();
        if (doctorSelect == null)
            return;
        
        await _repository.ExecuteInTransactionAsync(async () =>
        {
            _mapper.Map(doctorProfile, doctorSelect);
            await _repository.UpdateAsync(doctorSelect);
            await _repository.SaveChangesAsync(email);
            return true;
        });
    }

    public async Task<DoctorProfile> GetDoctorsById(int id)
    {
        var doctorEntity = await base.GetByIdAsync(id);
        return doctorEntity;
    }
}