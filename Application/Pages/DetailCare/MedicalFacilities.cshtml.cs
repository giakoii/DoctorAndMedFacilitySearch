using AutoMapper;
using BusinessLogic.Services;
using BusinessLogic.ViewModels;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Pages.DetailCare
{
    public class MedicalFacilitiesModel : PageModel
    {
        private readonly IMedicalFacilityService _medicalFacilityService;
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public MedicalFacilitiesModel(IMedicalFacilityService medicalFacilityService, IReviewService reviewService, IUserService userService, IMapper mapper)
        {
            _medicalFacilityService = medicalFacilityService;
            _reviewService = reviewService;
            _userService = userService;
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
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (NewReview.Rating < 1 || NewReview.Rating > 5)
            {
                ModelState.AddModelError("NewReview.Rating", "Please choose star");
                await OnGetAsync(NewReview.FacilityId);
                return Page();
            }
            var userObj = await _userService.FindAsync(x => x.Email == email, false, x => x.PatientProfile).Result.FirstOrDefaultAsync();
            if (userObj == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                await OnGetAsync(NewReview.FacilityId);
                return Page();
            }
            NewReview.PatientId = userObj.PatientProfile.PatientId;
            NewReview.PatientName = userObj.FullName;
            NewReview.PatientEmail = userObj.Email;
            var reviewEntity = _mapper.Map<Review>(NewReview);
            await _reviewService.AddAsyncReview(reviewEntity);
            return RedirectToPage(new { id = NewReview.FacilityId });
        }
    }
}
