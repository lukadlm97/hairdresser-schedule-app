using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(ScheduleItemId))]
        public ScheduleItem ScheduleItem { get; set; }
        public int ScheduleItemId { get; set; }
    }
}
