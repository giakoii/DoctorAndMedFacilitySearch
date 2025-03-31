using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class MedicalFacilitiesViewModel
    {
        public int FacilityId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Services { get; set; }
        public string? OpeningHours { get; set; }
        public double? Rating { get; set; }
        public bool IsActive { get; set; }
    }
}
