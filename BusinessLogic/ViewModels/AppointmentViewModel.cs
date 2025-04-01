using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string? Notes { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string FacilityName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public MedicalFacilitiesViewModel Facility { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
