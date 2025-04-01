using BusinessLogic.ViewModels;

namespace BusinessLogic.Services;

public interface IAdminService
{
    List<DoctorsWithoutFacilityViewModel> GetDoctorsWithoutFacility();
    
    List<MedicalFacilitiesViewModel> GetMedicalFacilities();
    
    Task<bool> AssignDoctorToFacility(int doctorId, int facilityId, string adminEmail);
    
    Task<bool> InsertMedicalFacility(MedicalFacilitiesViewModel medicalFacility, string adminEmail);
    
}