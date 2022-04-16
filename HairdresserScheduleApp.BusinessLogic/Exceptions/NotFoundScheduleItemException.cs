using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotFoundScheduleItemException:Exception
    {
        public int ScheduleItemId { get; set; }

        public NotFoundScheduleItemException( int scheduleId)
        {
            ScheduleItemId = scheduleId;
        }

        public NotFoundScheduleItemException(int scheduleId, string message) : base(message)
        {
            ScheduleItemId = scheduleId;
        }
        public NotFoundScheduleItemException(DateTime date, int scheduleId, string message, Exception innerException) 
            : base(message, innerException)
        {
            ScheduleItemId = scheduleId;
        }
    }
}
