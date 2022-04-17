using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotFoundReservationException : Exception
    {
        public int ReservationId { get; set; }

        public NotFoundReservationException(int reservationId)
        {
            ReservationId = reservationId;
        }

        public NotFoundReservationException(int scheduleId, string message) : base(message)
        {
            ReservationId = scheduleId;
        }
        public NotFoundReservationException(int scheduleId, string message, Exception innerException)
            : base(message, innerException)
        {
            ReservationId = scheduleId;
        }
    }
}
