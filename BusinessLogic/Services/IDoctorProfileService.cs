using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IDoctorProfileService : IBaseService<DoctorProfile, int, VwDoctorProfile>
{
    void AddDoctorProfile(DoctorViewModel doctorProfile, string email);
}