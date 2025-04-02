using BusinessLogic.Services;
using DataAccessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages.HealthArticles
{
    public class DetailsModel : PageModel
    {
        private readonly IHealthInfoService _healthInfoService;

        public DetailsModel(IHealthInfoService healthInfoService)
        {
            _healthInfoService = healthInfoService;
        }

        public HealthArticle Article { get; set; }

        public IActionResult OnGet(int id)
        {
            Article = _healthInfoService.GetHealthArticleById(id);
            if (Article == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
