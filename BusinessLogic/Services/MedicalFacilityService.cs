using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class MedicalFacilityService : BaseService<MedicalFacility, int, VwMedicalFacility>, IMedicalFacilityService
{
    public MedicalFacilityService(IBaseRepository<MedicalFacility, int, VwMedicalFacility> repository) : base(repository)
    {
    }
}