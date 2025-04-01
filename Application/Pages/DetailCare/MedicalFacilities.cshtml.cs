using AutoMapper;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.DetailCare
{
    public class MedicalFacilitiesModel : PageModel
    {
        private readonly IMedicalFacilityService _medicalFacilityService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public MedicalFacilitiesModel(IMedicalFacilityService medicalFacilityService, IReviewService reviewService, IMapper mapper)
        {
            _medicalFacilityService = medicalFacilityService;
            _reviewService = reviewService;
            _mapper = mapper;
        }
        [BindProperty]
        public ReviewViewModel NewReview { get; set; } = new ReviewViewModel();
        public MedicalFacilitiesViewModel MedicalFacility { get; set; } = new MedicalFacilitiesViewModel();

        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var reviewsData = _reviewService.FindView().Where(r => r.FacilityId == id).OrderByDescending(a => a.ReviewCreatedAt);

            var medicalFacility = await _medicalFacilityService.GetFacilityWithDoctorsAsync(id.Value);
            Reviews = _mapper.Map<List<ReviewViewModel>>(reviewsData);
            if (medicalFacility == null)
            {
                return NotFound();
            }
            MedicalFacility = _mapper.Map<MedicalFacilitiesViewModel>(medicalFacility);

            return Page();
        }
        public async Task<IActionResult> OnPostReviewAsync()
        {
            NewReview.ReviewCreatedAt = DateTime.UtcNow;
            NewReview.ReviewIsActive = true;
            var reviewEntity = _mapper.Map<Review>(NewReview);
            await _reviewService.AddAsyncReview(reviewEntity);
            return RedirectToPage(new { id = NewReview.FacilityId });
        }
    }
}
