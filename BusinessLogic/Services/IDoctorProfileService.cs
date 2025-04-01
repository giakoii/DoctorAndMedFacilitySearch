using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IDoctorProfileService : IBaseService<DoctorProfile, int, VwDoctorProfile>
{
    DoctorViewModel? GetDoctorProfile(string email);

    Task AddDoctorProfile(DoctorViewModel doctorProfile, string email);

    Task UpdateDoctorProfile(DoctorViewModel doctorProfile, string email);
    Task<DoctorProfile> GetDoctorsById(int id);
}