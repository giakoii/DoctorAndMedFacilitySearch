using DataAccessObject;
using DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class MedicalFacilityService : BaseService<MedicalFacility, int, VwMedicalFacility>, IMedicalFacilityService
{
    public MedicalFacilityService(IBaseRepository<MedicalFacility, int, VwMedicalFacility> repository) : base(repository)
    {
    }

    public async Task<MedicalFacility?> GetFacilityWithDoctorsAsync(int facilityId)
    {
        var query = await FindAsync(x => x.FacilityId == facilityId,
                                    isTracking: false,
                                    x => x.Doctors);
        query = query.Include(f => f.Doctors)
                 .ThenInclude(d => d.Doctor);
        return await query.FirstOrDefaultAsync();
    }
}