using System.Security.Claims;
using Application.ViewModels;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.User;

public class Profile : PageModel
{
    [BindProperty] public DoctorViewModel? DoctorProfileViewModel { get; set; }
    [BindProperty] public PatientProfileViewModel? PatientProfileViewModel { get; set; }
    public List<UserViewModel> SharedDoctors { get; set; }

    private readonly IUserService _userService;

    public Profile(IUserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        Console.WriteLine("Entering OnGet method.");

        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        Console.WriteLine($"User email: {userEmail}");

        if (string.IsNullOrEmpty(userEmail))
        {
            Console.WriteLine("User email is null or empty.");
            return;
        }

        try
        {
            Console.WriteLine("User claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            // Check role specifically
           
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim != null && roleClaim == ((int)ConstantEnum.Role.Patient).ToString())
                
            {
                // Load patient profile with appointments included
                PatientProfileViewModel = _userService.GetPatientProfile(userEmail);

                if (PatientProfileViewModel != null)
                {
                    Console.WriteLine($"Patient profile loaded for {userEmail}");

           
                   

                   
                    Console.WriteLine($"Found {PatientProfileViewModel.Appointments?.Count ?? 0} appointments");

                    PatientProfileViewModel.MedicalFiles = _userService.GetMedicalFiles(userEmail);
                    SharedDoctors = _userService.GetSharedDoctors(userEmail);
                }
                else
                {
                    Console.WriteLine($"No patient profile found for {userEmail}");
                }
                if (PatientProfileViewModel.Appointments == null || !PatientProfileViewModel.Appointments.Any())
                {
                    Console.WriteLine("Loading appointments separately...");
                    PatientProfileViewModel.Appointments = _userService.GetPatientMedicalHistory(userEmail);
                }
            }
            else if (User.IsInRole(ConstantEnum.Role.MedicalExpert.ToString()))
            {
                DoctorProfileViewModel = _userService.GetDoctorProfile(userEmail);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnGet: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

       
            PatientProfileViewModel ??= new PatientProfileViewModel();
            PatientProfileViewModel.Appointments ??= new List<AppointmentViewModel>();
            PatientProfileViewModel.MedicalFiles ??= new List<MedicalFileViewModel>();
            SharedDoctors ??= new List<UserViewModel>();
        }

        Console.WriteLine("Exiting OnGet method.");
    }
    public IActionResult OnPostShareMedicalHistory(string doctorEmail)
    {
        string patientEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        var result = _userService.ShareMedicalHistory(patientEmail, doctorEmail);
        TempData["Message"] = result ? "Medical history shared successfully!" : "Failed to share medical history.";
        return RedirectToPage();
    }

    public IActionResult OnPostUploadMedicalFile(IFormFile file)
    {
        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        var result = _userService.UploadMedicalFile(userEmail, file);
        TempData["Message"] = result ? "File uploaded successfully!" : "Failed to upload file.";
        return RedirectToPage();
    }

    public IActionResult OnGetDownloadMedicalFile(int fileId)
    {
        string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        var medicalFile = _userService.GetMedicalFileById(fileId, userEmail);
        if (medicalFile == null)
        {
            return NotFound();
        }

        return File(medicalFile.FileContent, "application/octet-stream", medicalFile.FileName);
    }
}