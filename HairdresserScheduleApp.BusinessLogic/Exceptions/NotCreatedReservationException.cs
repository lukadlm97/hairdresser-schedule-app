using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotCreatedReservationException : Exception
    {
        public int UserId { get; set; }
        public int ScheduleItemId { get; set; }

        public NotCreatedReservationException(int userId, int scheduleItemId)
        {
            UserId = userId;
            ScheduleItemId = scheduleItemId;
        }

        public NotCreatedReservationException(int userId, int scheduleItemId, string message) : base(message)
        {
            UserId = userId;
            ScheduleItemId = scheduleItemId;
        }
        public NotCreatedReservationException(int userId, int scheduleItemId, string message, Exception innerException) : base(message, innerException)
        {
            UserId = userId;
            ScheduleItemId = scheduleItemId;
        }
    }
}
