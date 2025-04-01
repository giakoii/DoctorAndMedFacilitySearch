using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class PatientProfileService : BaseService<PatientProfile, int, VwPatientProfile>, IPatientProfileService
{
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="mapper"></param>
    public PatientProfileService(IBaseRepository<PatientProfile, int, VwPatientProfile> repository, IMapper mapper) : base(repository)
    {
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get patient profile
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public PatientProfileViewModel? GetPatientProfile(string email)
    {
        // Select patient profile
        var patientProfile = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (patientProfile == null)
        {
            return null;
        }

        return _mapper.Map<PatientProfileViewModel>(patientProfile);
    }

    /// <summary>
    /// Update patient profile
    /// </summary>
    /// <param name="patientProfile"></param>
    /// <param name="email"></param>
    public async Task UpdatePatientProfile(PatientProfileViewModel patientProfile, string email)
    {
        // Find patient profile
        var patientSelect = await _repository.Find(x => x.PatientId == patientProfile.PatientId && x.IsActive == true, false).FirstOrDefaultAsync();
        if (patientSelect == null)
            return;
        
        // Update patient profile
        await _repository.ExecuteInTransactionAsync(async () =>
        { 
            _mapper.Map(patientProfile, patientSelect);
            await _repository.UpdateAsync(patientSelect);
            await _repository.SaveChangesAsync(email);
            return true;
        });
    }
}