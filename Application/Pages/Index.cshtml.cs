using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace Application.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IMedicalFacilityService _medicalFacilityService;
    private readonly IDoctorProfileService _doctorProfileService;
    private readonly IUserService _userService;

    public IndexModel(HttpClient httpClient, IMedicalFacilityService medicalFacilityService,
        IDoctorProfileService doctorProfileService, IUserService userService)
    {
        _httpClient = httpClient;
        _medicalFacilityService = medicalFacilityService;
        _doctorProfileService = doctorProfileService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult OnGet(bool IsSearch = false, string Keyword = null, string Specialty = null,
        double? Rating = null, decimal? Fee = null, string Services = null, int PageNumberDoctors = 1,
        int PageSizeDoctors = 4, int PageNumberMedicalFacilities = 1, int PageSizeMedicalFacilities = 4)
    {
        if (User.FindFirst(c => c.Type == "RoleName")?.Value == ConstantEnum.Role.MedicalExpert.ToString())
        {
            return RedirectToPage("/Doctor/Index");
        }

        if (User.FindFirst(c => c.Type == "RoleName")?.Value == ConstantEnum.Role.Patient.ToString())
        {
            
        }

        if (!IsSearch)
        {
            ViewData["MedicalFacilities"] = new List<MedicalFacilityViewModel>();
            ViewData["Doctors"] = new List<DoctorViewModel>();
            return Page();
        }

        ViewData["IsSearch"] = IsSearch;
        ViewData["Keyword"] = Keyword;

        var doctorsQuery = _doctorProfileService.FindView();
        var medicalFacilitiesQuery = _medicalFacilityService.FindView();

        if (!string.IsNullOrEmpty(Keyword))
        {
            doctorsQuery = doctorsQuery.Where(x => x.FullName.Contains(Keyword));
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

        // Total records
        int totalDoctors = doctorsQuery.Count();
        int totalMedicalFacilities = medicalFacilitiesQuery.Count();

        // Total pages
        int totalPagesDoctors = (int)Math.Ceiling((double)totalDoctors / PageSizeDoctors);
        int totalPagesMedicalFacilities = (int)Math.Ceiling((double)totalMedicalFacilities / PageSizeMedicalFacilities);

        // Paging for data Doctors
        var pagedDoctors = doctorsQuery
            .Skip((PageNumberDoctors - 1) * PageSizeDoctors)
            .Take(PageSizeDoctors)
            .Select(x => new DoctorViewModel
            {
                Name = x.FullName,
                Specialty = x.Specialty,
                ExperienceYears = x.ExperienceYears,
                WorkSchedule = x.WorkSchedule,
                ConsultationFee = x.ConsultationFee,
                Availability = x.Availability
            }).ToList();

        // Paging for data MedicalFacilities
        var pagedMedicalFacilities = medicalFacilitiesQuery
            .Skip((PageNumberMedicalFacilities - 1) * PageSizeMedicalFacilities)
            .Take(PageSizeMedicalFacilities)
            .Select(x => new MedicalFacilityViewModel
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

        ViewData["MedicalFacilities"] = pagedMedicalFacilities;
        ViewData["Doctors"] = pagedDoctors;

        ViewData["TotalDoctors"] = totalDoctors;
        ViewData["TotalMedicalFacilities"] = totalMedicalFacilities;

        ViewData["PageNumberDoctors"] = PageNumberDoctors;
        ViewData["PageNumberMedicalFacilities"] = PageNumberMedicalFacilities;

        ViewData["PageSizeDoctors"] = PageSizeDoctors;
        ViewData["PageSizeMedicalFacilities"] = PageSizeMedicalFacilities;

        ViewData["TotalPagesDoctors"] = totalPagesDoctors;
        ViewData["TotalPagesMedicalFacilities"] = totalPagesMedicalFacilities;

        return Page();
    }

    [HttpGet]
    public async Task<IActionResult> OnGetNearbyAsync(double lat, double lon)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "YourAppName/1.0 (your@email.com)");

        using var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Failed to retrieve address");
        }

        var responseData = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(responseData))
        {
            return BadRequest("Empty response from Nominatim API");
        }

        var json = JObject.Parse(responseData);
        string address = json["display_name"]?.ToString() ?? "Can't find address";

        var medicals = FindMedicalFacilitiesNearby(address);
        return new JsonResult(new { address, medicals });
    }

    private List<MedicalFacilityViewModel> FindMedicalFacilitiesNearby(string address)
    {
        // Normalize address
        var keywords = address.Split(new[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(k => NormalizeText(k))
            .Where(k => k.Length > 2)
            .ToList();

        if (keywords.Contains("viet nam"))
        {
            keywords.Remove("viet nam");
        }

        var hospitals = _medicalFacilityService
            .FindView()
            .AsEnumerable()
            .Where(x => keywords.Any(k => NormalizeText(x.Address).Contains(NormalizeText(k))))
            .Select(x => new MedicalFacilityViewModel
            {
                FacilityId = x.FacilityId,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Services = x.Services,
                OpeningHours = x.OpeningHours,
                Rating = x.Rating
            })
            .ToList();
        return hospitals;
    }

    private string NormalizeText(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        input = input.ToLower().Trim();
        input = input.Normalize(NormalizationForm.FormD);
        input = Regex.Replace(input, @"\p{Mn}", "");
        input = Regex.Replace(input, @"[^\w\s]", "");
        input = Regex.Replace(input, @"\s+", " ");
        return input.Trim();
    }
}