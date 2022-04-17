using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Exceptions
{
    public class NotExistReservationException : Exception
    {
        public int ReservationId { get; set; }

        public NotExistReservationException(int reservationId)
        {
            ReservationId = reservationId;
        }

        public NotExistReservationException(int reservationId, string message) : base(message)
        {
            ReservationId = reservationId;
        }
        public NotExistReservationException( int reservationId, string message, Exception innerException) : base(message, innerException)
        {
            ReservationId = reservationId;
        }
    }
}
