using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotFoundDailyScheduleException:Exception
    {
        public DateTime Date { get; set; }
        public int ScheduleId { get; set; }

        public NotFoundDailyScheduleException(DateTime date,int scheduleId)
        {
            Date = date;
            ScheduleId = scheduleId;
        }

        public NotFoundDailyScheduleException(DateTime date, int scheduleId, string message):base(message)
        {
            Date = date;
            ScheduleId = scheduleId;
        }
        public NotFoundDailyScheduleException(DateTime date, int scheduleId, string message,Exception innerException) : base(message, innerException)
        {
            Date = date;
            ScheduleId = scheduleId;
        }
    }
}
