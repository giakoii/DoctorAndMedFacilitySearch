using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace Application.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IBaseService<MedicalFacility, int, VwMedicalFacility> _medicalFacilityService;
    private readonly IBaseService<DoctorProfile, int, VwDoctorProfile> _doctorProfileService;

    public IndexModel(HttpClient httpClient, IBaseService<MedicalFacility, int, VwMedicalFacility> medicalFacilityService, IBaseService<DoctorProfile, int, VwDoctorProfile> doctorProfileService)
    {
        _httpClient = httpClient;
        _medicalFacilityService = medicalFacilityService;
        _doctorProfileService = doctorProfileService;
    }

    [HttpGet]
    public IActionResult OnGetAsync(bool IsSearch = false, string Keyword = null, string Specialty = null, double? Rating = null, decimal? Fee = null, string Services = null)
    {
        if (!IsSearch)
        {
            ViewData["MedicalFacilities"] = new List<MedicalFacilityViewModel>();
            ViewData["Doctors"] = new List<DoctorViewModel>();
            return Page();
        }

        var doctorsQuery = _doctorProfileService.FindView();
        var medicalFacilitiesQuery = _medicalFacilityService.FindView();

        if (!string.IsNullOrEmpty(Keyword))
        {
            doctorsQuery = doctorsQuery.Where(x => x.Name.Contains(Keyword));
            medicalFacilitiesQuery = medicalFacilitiesQuery.Where(x => x.Name.Contains(Keyword));
        }

        if (!string.IsNullOrEmpty(Specialty))
        {
            doctorsQuery = doctorsQuery.Where(d => d.Specialty == Specialty);
        }

        if (Rating.HasValue)
        {
            medicalFacilitiesQuery = medicalFacilitiesQuery.Where(d => d.Rating == Rating);
        }

        if (Fee.HasValue)
        {
            doctorsQuery = doctorsQuery.Where(d => d.ConsultationFee <= Fee);
        }

        if (!string.IsNullOrEmpty(Services))
        {
            medicalFacilitiesQuery = medicalFacilitiesQuery.Where(d => d.Services == Services);
        }

        ViewData["MedicalFacilities"] = medicalFacilitiesQuery.Select(x => new MedicalFacilityViewModel
        {
            FacilityId = x.FacilityId,
            Name = x.Name,
            Address = x.Address,
            Phone = x.Phone,
            Email = x.Email,
            Services = x.Services,
            OpeningHours = x.OpeningHours,
            Rating = x.Rating
        }).ToList();

        ViewData["Doctors"] = doctorsQuery.Select(x => new DoctorViewModel
        {
            Name = x.Name,
            Specialty = x.Specialty,
            ExperienceYears = x.ExperienceYears,
            WorkSchedule = x.WorkSchedule,
            ConsultationFee = x.ConsultationFee,
            Availability = x.Availability
        }).ToList();

        return Page();
    }
}