using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using DataAccessObject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class AdminService : IAdminService
{
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IMedicalFacilityService _medicalFacilityService;
    private readonly IDoctorProfileService _doctorProfileService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="adminRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="medicalFacilityService"></param>
    /// <param name="doctorProfileService"></param>
    public AdminService(IAdminRepository adminRepository, IMapper mapper,
        IMedicalFacilityService medicalFacilityService, IDoctorProfileService doctorProfileService)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _medicalFacilityService = medicalFacilityService;
        _doctorProfileService = doctorProfileService;
    }

    /// <summary>
    /// Get all doctors without a facility
    /// </summary>
    /// <returns></returns>
    public List<DoctorsWithoutFacilityViewModel> GetDoctorsWithoutFacility()
    {
        var doctorsWithoutFacility = _adminRepository.FindView();
        return _mapper.Map<List<DoctorsWithoutFacilityViewModel>>(doctorsWithoutFacility);
    }

    /// <summary>
    /// Get all medical facilities
    /// </summary>
    /// <returns></returns>
    public List<MedicalFacilitiesViewModel> GetMedicalFacilities()
    {
        var medicalFacilities = _medicalFacilityService.FindView();
        return _mapper.Map<List<MedicalFacilitiesViewModel>>(medicalFacilities);
    }

    /// <summary>
    /// Assign a doctor to a facility
    /// </summary>
    /// <param name="doctorId"></param>
    /// <param name="facilityId"></param>
    /// <param name="adminEmail"></param>
    /// <returns></returns>
    public async Task<bool> AssignDoctorToFacility(int doctorId, int facilityId, string adminEmail)
    {
        return await _adminRepository.ExecuteInTransactionAsync(async () =>
        {
            // Find doctor and facility
            var doctor = await _doctorProfileService
                .Find(x => x.DoctorId == doctorId,
                    true,
                    f => f.Facilities)
                .FirstOrDefaultAsync();

            // Find facility
            var facility = await _medicalFacilityService
                .Find(x => x.FacilityId == facilityId,
                    true,
                    f => f.Doctors)
                .FirstOrDefaultAsync();

            if (doctor == null && facility == null)
            {
                return false;
            }

            _adminRepository.AddDoctorToFacility(doctorId, facilityId);
            return true;
        });
    }

    /// <summary>
    /// Insert a new medical facility
    /// </summary>
    /// <param name="medicalFacility"></param>
    /// <param name="adminEmail"></param>
    /// <returns></returns>
    public async Task<bool> InsertMedicalFacility(MedicalFacilitiesViewModel medicalFacility, string adminEmail)
    {
        await _medicalFacilityService.AddAsync(_mapper.Map<MedicalFacility>(medicalFacility));
        _medicalFacilityService.SaveChanges(adminEmail);
        return true;
    }
}