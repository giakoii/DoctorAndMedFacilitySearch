using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class ScheduleModel
    {
        public int ScheduleId { get; set; }
        public int SlotId { get; set; }
        public string SlotTime { get; set; }
        public DateOnly ScheduleDate { get; set; }
    }
}
