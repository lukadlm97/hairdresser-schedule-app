using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Services
{
    internal class Reservation
    {
    }
    public interface IReservation
    {
        Task<IQueryable<Models.Reservation>> GetAll();
        Task<Models.Reservation> GetToday();
        Task<Models.Reservation> GetById(int id);
        Task<IQueryable<Models.Reservation>> GetForNextDays(int days = 7);
        Task<Models.Reservation> Create(Models.Reservation newReservation);
        Task<bool> Delete(int reservationId);
    }
}
