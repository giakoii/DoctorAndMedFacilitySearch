using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Application.Components.Search;

public class SearchViewComponent : ViewComponent
{
    private readonly HttpClient _httpClient;
    private readonly IBaseService<MedicalFacility, int, VwMedicalFacility> _medicalFacilityService;

    public SearchViewComponent(HttpClient httpClient, IBaseService<MedicalFacility, int, VwMedicalFacility> medicalFacilityService)
    {
        _httpClient = httpClient;
        _medicalFacilityService = medicalFacilityService;
    }

    
}