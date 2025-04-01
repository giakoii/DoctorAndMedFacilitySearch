using DataAccessObject.Models;

namespace DataAccessObject.Repositories;

public interface IAdminRepository : IBaseRepository<User, int, VwDoctorsWithoutFacility>
{
    void AddDoctorToFacility(int doctorId, int facilityId);
}