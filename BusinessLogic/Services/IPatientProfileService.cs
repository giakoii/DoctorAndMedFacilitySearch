using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IPatientProfileService : IBaseService<PatientProfile, int, VwPatientProfile>
{
    PatientProfileViewModel? GetPatientProfile(string email);
    
    Task UpdatePatientProfile(PatientProfileViewModel patientProfile, string email);
}