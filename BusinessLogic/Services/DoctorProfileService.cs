using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class DoctorProfileService : BaseService<DoctorProfile, int, VwDoctorProfile>, IDoctorProfileService
{
    public DoctorProfileService(IBaseRepository<DoctorProfile, int, VwDoctorProfile> repository) : base(repository)
    {
    }
}