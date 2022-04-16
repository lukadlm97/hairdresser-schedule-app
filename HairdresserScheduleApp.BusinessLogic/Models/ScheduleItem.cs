using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public string Start { get; set; }
        public DateTime StartTime { get; set; }
        public string End { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public int DailyScheduleId { get; set; }
        [ForeignKey(nameof(DailyScheduleId))]
        public virtual DailySchedule DailySchedule { get; set; }
    }
}
