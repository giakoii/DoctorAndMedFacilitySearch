using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IMedicalFacilityService : IBaseService<MedicalFacility, int, VwMedicalFacility>
{
    Task<MedicalFacility?> GetFacilityWithDoctorsAsync(int facilityId);
}