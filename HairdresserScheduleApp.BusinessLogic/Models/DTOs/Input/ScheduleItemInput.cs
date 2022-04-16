using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Models.DTOs.Input
{
    public class ScheduleItemInput
    {
        public string Start { get; set; }
        public string End { get; set; }
        public int Duration { get; set; }
        public int DailyScheduleId { get; set; }
    }
}
