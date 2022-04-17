using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotAvailableScheduleItemException : Exception
    {
        public NotAvailableScheduleItemException()
        {
        }

        public NotAvailableScheduleItemException( string message) : base(message)
        {
        }
        public NotAvailableScheduleItemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
