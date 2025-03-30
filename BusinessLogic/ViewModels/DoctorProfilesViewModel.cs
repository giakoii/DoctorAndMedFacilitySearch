using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class DoctorProfilesViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string Qualification { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Availability { get; set; }
    }
}
