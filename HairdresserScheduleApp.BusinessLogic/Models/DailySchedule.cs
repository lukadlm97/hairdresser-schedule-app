using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Models
{
    public class DailySchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<ScheduleItem> ScheduleItems { get; set; }
    }
}
