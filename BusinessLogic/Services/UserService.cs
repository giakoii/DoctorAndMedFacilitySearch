using System.Linq.Expressions;
using System.Security.Cryptography;
using Application.ViewModels;
using AutoMapper;
using BusinessLogic.Mappings;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class UserService : BaseService<User, int, VwUser>, IUserService
{
    private readonly IRoleService _roleService;
    private readonly IBaseService<DoctorProfile, int, VwDoctorProfile> _doctorProfileService;
    private readonly IBaseService<PatientProfile, int, VwPatientProfile> _patientProfileService;
    private readonly IBaseRepository<MedicalFile, int, MedicalFile> _medicalFileRepository;
    private readonly AppDbContext _context;
    private readonly byte[] _encryptionKey = new byte[32];
    private readonly byte[] _encryptionIV = new byte[16];
    private readonly IMapper _mapper;

    public UserService(
        IBaseRepository<User, int, VwUser> repository,
        IRoleService roleService,
        IBaseService<DoctorProfile, int, VwDoctorProfile> doctorProfileService,
        IBaseService<PatientProfile, int, VwPatientProfile> patientProfileService,
        IMapper mapper,
        AppDbContext context,
        IBaseRepository<MedicalFile, int, MedicalFile> medicalFileRepository) : base(repository)
    {
        _roleService = roleService;
        _doctorProfileService = doctorProfileService;
        _patientProfileService = patientProfileService;
        _mapper = mapper;
        _context = context;
        _medicalFileRepository = medicalFileRepository;

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(_encryptionKey);
            rng.GetBytes(_encryptionIV);
        }
    }

    public bool Register(RegisterViewModel registerViewModel)
    {
        return _repository.ExecuteInTransaction(() =>
        {
            // Check if user already exists
            var userExist = _repository
                .Find(x => x.Email == registerViewModel.Email, false)
                .FirstOrDefault();

            if (userExist != null)
            {
                return false;
            }

            // Get role
            var role = _roleService.GetById(registerViewModel.RoleId);
            if (role == null)
            {
                return false;
            }

            // Add new user
            var newUser = new User
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                RoleId = registerViewModel.RoleId,
                CreatedAt = DateTime.UtcNow,
               CreatedBy  =registerViewModel.Email,
               UpdatedAt = DateTime.UtcNow,
               UpdatedBy =registerViewModel.Email,
            };
            _repository.Add(newUser);
            SaveChanges(registerViewModel.Email);
            return true;
        });
    }

    public DoctorViewModel? GetDoctorProfile(string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (user == null) return null;

        var doctorProfile = _doctorProfileService.Find(x => x.DoctorId == user.UserId).FirstOrDefault();
        return doctorProfile == null ? null : _mapper.Map<DoctorViewModel>(doctorProfile);
    }

    public PatientProfileViewModel? GetPatientProfile(string email)
    {
        try
        {
            var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
            if (user == null) return null;

         
            var patientProfile = _context.PatientProfiles
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Facility)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.Doctor)
                .FirstOrDefault(p => p.PatientId == user.UserId);

            if (patientProfile == null) return null;

          
            var viewModel = new PatientProfileViewModel
            {
                PatientId = patientProfile.PatientId,
                DateOfBirth = patientProfile.DateOfBirth,
                Gender = patientProfile.Gender,
                Address = patientProfile.Address,
                MedicalHistory = patientProfile.MedicalHistory,
                Allergies = patientProfile.Allergies,
                BloodType = patientProfile.BloodType,
                EmergencyContact = patientProfile.EmergencyContact,
                Appointments = new List<AppointmentViewModel>(),
                MedicalFiles = new List<MedicalFileViewModel>()
            };

          
            if (patientProfile.Appointments != null)
            {
                foreach (var appointment in patientProfile.Appointments.Where(a => a.IsActive))
                {
                    var appointmentViewModel = new AppointmentViewModel
                    {
                        AppointmentId = appointment.AppointmentId,
                        AppointmentDate = appointment.AppointmentDate,
                        Status = appointment.Status,
                        PaymentStatus = appointment.PaymentStatus,
                        Notes = appointment.Notes,
                        PatientName = user != null ? user.FullName : "N/A", 
                        DoctorName = appointment.Doctor != null && appointment.Doctor.Doctor != null
                            ? appointment.Doctor.Doctor.FullName
                            : "N/A",
                        DoctorId = appointment.DoctorId,
                        FacilityId = appointment.FacilityId,
                        FacilityName = appointment.Facility != null
                            ? appointment.Facility.Name
                            : "N/A",
                        IsActive = appointment.IsActive,
                        CreatedAt = appointment.CreatedAt,
                        UpdatedAt = appointment.UpdatedAt,
                        Facility = null
                    };
                    viewModel.Appointments.Add(appointmentViewModel);
                }
            }

          
            viewModel.MedicalFiles = GetMedicalFiles(email);

            return viewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPatientProfile: {ex.Message}");
            throw;
        }
    }
    public List<AppointmentViewModel> GetPatientMedicalHistory(string email)
    {
        try
        {
            Console.WriteLine($"Getting medical history for: {email}");

            var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("User not found");
                return new List<AppointmentViewModel>();
            }

            var appointments = _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == user.UserId && a.IsActive)
            .Include(a => a.Facility)
            .Include(a => a.Patient)
            .Include(a => a.Doctor) 
            .OrderByDescending(a => a.AppointmentDate)
            .ToList();

            Console.WriteLine($"Found {appointments.Count} appointments");

            return _mapper.Map<List<AppointmentViewModel>>(appointments);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPatientMedicalHistory: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return new List<AppointmentViewModel>();
        }
    }

    public bool ShareMedicalHistory(string patientEmail, string doctorEmail)
    {
        var patient = _repository.FindView(x => x.Email == patientEmail).FirstOrDefault();
        var doctor = _repository.FindView(x => x.Email == doctorEmail && x.RoleId == (int)ConstantEnum.Role.MedicalExpert).FirstOrDefault();

        if (patient == null || doctor == null) return false;

        // Check if already shared
        var existingShare = _context.MedicalHistoryShares
            .FirstOrDefault(s => s.PatientId == patient.UserId && s.DoctorId == doctor.UserId);

        if (existingShare != null)
        {
            if (existingShare.IsActive.HasValue && !existingShare.IsActive.Value)
            {
                existingShare.IsActive = true;
                existingShare.SharedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return true; // Already shared
        }

        // Create new share record
        var newShare = new MedicalHistoryShare
        {
            PatientId = patient.UserId,
            DoctorId = doctor.UserId,
            SharedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.MedicalHistoryShares.Add(newShare);
        _context.SaveChanges();

        return true;
    }
    public bool UploadMedicalFile(string email, IFormFile file)
    {
        if (file == null || file.Length == 0) return false;

        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (user == null) return false;

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);

        var medicalFile = new MedicalFile
        {
            PatientId = user.UserId,
            FileName = file.FileName,
            FileContent = EncryptFile(memoryStream.ToArray()),
            UploadedAt = DateTime.Now,
            UploadedBy = email,
            IsActive = true
        };

        _medicalFileRepository.Add(medicalFile);
        SaveChanges(email);
        return true;
    }

    public List<MedicalFileViewModel> GetMedicalFiles(string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (user == null) return new List<MedicalFileViewModel>();

     
        var medicalFiles = _medicalFileRepository.Find(
            x => x.PatientId == user.UserId && x.IsActive,
            false)
            .AsNoTracking() 
            .ToList();

        
        return medicalFiles.Select(f => new MedicalFileViewModel
        {
            FileId = f.FileId,
            FileName = f.FileName,
            UploadedAt = f.UploadedAt,
            FileContent = DecryptFile(f.FileContent) 
        }).ToList();
    }

    public byte[] DecryptFile(byte[] encryptedBytes)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _encryptionIV;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private byte[] EncryptFile(byte[] fileBytes)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _encryptionIV;
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(fileBytes, 0, fileBytes.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    public List<UserViewModel> GetSharedDoctors(string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (user == null) return new List<UserViewModel>();

        return _repository.Find(
            x => x.UserId == user.UserId && x.IsActive,
            false)
            .Select(s => new UserViewModel
            {
                FullName = s.FullName,
                Email = s.Email
            }).ToList();
    }

    public MedicalFileViewModel GetMedicalFileById(int fileId, string email)
    {
        var user = _repository.FindView(x => x.Email == email).FirstOrDefault();
        if (user == null) return null;

       
        var medicalFile = _medicalFileRepository.Find(
            x => x.FileId == fileId && x.PatientId == user.UserId && x.IsActive,
            false)
            .AsNoTracking() 
            .FirstOrDefault();

       
        if (medicalFile == null) return null;

        return new MedicalFileViewModel
        {
            FileId = medicalFile.FileId,
            FileName = medicalFile.FileName,
            UploadedAt = medicalFile.UploadedAt,
            FileContent = DecryptFile(medicalFile.FileContent) 
        };
    }
    public List<SharedMedicalHistoryViewModel> GetSharedMedicalHistories(string doctorEmail)
    {
        // First verify the doctor exists
        var doctor = _repository.FindView(x => x.Email == doctorEmail && x.RoleId == (int)ConstantEnum.Role.MedicalExpert)
            .FirstOrDefault();

        if (doctor == null) return new List<SharedMedicalHistoryViewModel>();

        // In a real implementation, you would have a sharing table in your database
        // For this example, we'll simulate it by getting all patients who have shared with this doctor

        // Get all patients who have shared with this doctor
        var sharedPatients = _context.MedicalHistoryShares
            .Where(s => s.DoctorId == doctor.UserId && s.IsActive == true)
            .Select(s => s.PatientId)
            .ToList();

        if (!sharedPatients.Any()) return new List<SharedMedicalHistoryViewModel>();

        // Get the patient profiles and related data
        var histories = _context.PatientProfiles
            .Where(p => sharedPatients.Contains(p.PatientId))
            .Include(p => p.Patient)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Facility)
            .Select(p => new SharedMedicalHistoryViewModel
            {
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                PatientEmail = p.Patient.Email,
                DateOfBirth = p.DateOfBirth != default ?
        p.DateOfBirth.ToDateTime(TimeOnly.MinValue) :
        (DateTime?)null,
                Gender = p.Gender,
                BloodType = p.BloodType,
                Allergies = p.Allergies,
                MedicalHistory = p.MedicalHistory,
                Appointments = p.Appointments
                    .Where(a => a.IsActive)
                    .Select(a => new AppointmentViewModel
                    {
                        AppointmentId = a.AppointmentId,
                        AppointmentDate = a.AppointmentDate,
                        FacilityName = a.Facility.Name,
                        Status = a.Status
                    }).ToList(),
                MedicalFiles = _context.MedicalFiles
                    .Where(f => f.PatientId == p.PatientId && f.IsActive)
                    .Select(f => new MedicalFileViewModel
                    {
                        FileId = f.FileId,
                        FileName = f.FileName,
                        UploadedAt = f.UploadedAt
                    }).ToList()
            })
            .ToList();

        return histories;
    }
}