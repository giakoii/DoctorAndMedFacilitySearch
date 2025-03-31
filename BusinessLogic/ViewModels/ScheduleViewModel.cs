using DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public DateOnly ScheduleDate { get; set; }

        public virtual User Doctor { get; set; } = null!;

        public ICollection<ScheduleSlotViewModel> ScheduleSlots { get; set; } = new List<ScheduleSlotViewModel>();
    }
}
