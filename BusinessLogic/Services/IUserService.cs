using Application.ViewModels;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services;

public interface IUserService : IBaseService<User, int, VwUser>
{
    bool Register(RegisterViewModel registerViewModel);

    PatientProfileViewModel? GetPatientProfile(string email);
    
    DoctorViewModel? GetDoctorProfile(string email);
    List<AppointmentViewModel> GetPatientMedicalHistory(string email);
    bool ShareMedicalHistory(string patientEmail, string doctorEmail);
    bool UploadMedicalFile(string email, IFormFile file);
    List<MedicalFileViewModel> GetMedicalFiles(string email);
    List<UserViewModel> GetSharedDoctors(string email);
    MedicalFileViewModel GetMedicalFileById(int fileId, string email);

}