using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class AppointmentCreationModel
    {
        public int DoctorId { get; set; }
        public string PatientEmail { get; set; }
        public int FacilityId { get; set; }
        public DateOnly SelectedDate { get; set; }
        public int ScheduleId { get; set; }
        public int SlotId { get; set; }
    }
}
