using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    
    public class NotCreatedNewDailyScheduleException : Exception
    {
        public DateTime Date { get; set; }

        public NotCreatedNewDailyScheduleException(DateTime date)
        {
            Date = date;
        }

        public NotCreatedNewDailyScheduleException(DateTime date, string message) : base(message)
        {
            Date = date;
        }
        public NotCreatedNewDailyScheduleException(DateTime date, string message, Exception innerException) : base(message, innerException)
        {
            Date = date;
        }
    }
}
