using DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class HealthInfoService : IHealthInfoService
    {
        private readonly DoctorAndMedFacilitySearchContext _context;

        public HealthInfoService(DoctorAndMedFacilitySearchContext context)
        {
            _context = context;
        }

        public List<HealthArticle> GetActiveHealthArticles()
        {
            return _context.HealthArticles
                .Where(a => a.IsActive.HasValue && a.IsActive.Value) 
                .ToList();
        }

        public List<MedicalFacility> GetCertifiedMedicalFacilities()
        {
            return _context.MedicalFacilities
                .Where(f => f.IsActive )
                .ToList();
        }
        public HealthArticle GetHealthArticleById(int id)
        {
            return _context.HealthArticles
                .FirstOrDefault(a => a.ArticleId == id && (a.IsActive == true));
        }
    }
}
