using HairdresserScheduleApp.BusinessLogic.Models;

namespace HairdresserScheduleApp.BusinessLogic.UnitOfWorks
{
    internal class ReservationUoW : IReservationUnitOfWork
    {
        private readonly AppDbContext context;

        public ReservationUoW(AppDbContext dbContext, Repositories.IReservation reservation)
        {
            this.context = dbContext;
            Reservation = reservation;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public Repositories.IReservation Reservation { get; set; }

        public async Task<int> Commit()
        {
            return await context.SaveChangesAsync();
        }
    }

    public interface IReservationUnitOfWork : IDisposable
    {
        Repositories.IReservation Reservation { get; set; }

        Task<int> Commit();
    }
}