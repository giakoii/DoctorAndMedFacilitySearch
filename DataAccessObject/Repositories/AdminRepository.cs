using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repositories;

public class AdminRepository : BaseRepository<User, int, VwDoctorsWithoutFacility>, IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public void AddDoctorToFacility(int doctorId, int facilityId)
    {
        var doctor = _context.DoctorProfiles.Include(d => d.Facilities).FirstOrDefault(d => d.DoctorId == doctorId);
        var facility = _context.MedicalFacilities.FirstOrDefault(m => m.FacilityId == facilityId);

        if (doctor != null && facility != null)
        {
            doctor.Facilities.Add(facility);
            facility.Doctors.Add(doctor);
            _context.SaveChanges();
        }
    }
}