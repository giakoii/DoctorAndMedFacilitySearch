using DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IHealthInfoService
    {
        List<HealthArticle> GetActiveHealthArticles();
        List<MedicalFacility> GetCertifiedMedicalFacilities();
        HealthArticle GetHealthArticleById(int id);
    }
}