using HairdresserScheduleApp.BusinessLogic.Utilities;

namespace HairdresserScheduleApp.BusinessLogic.Services
{
    public class Reservation : IReservation
    {
        private readonly UnitOfWorks.IReservationUnitOfWork reservationUnitOfWork;
        private readonly UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork;
        private readonly UnitOfWorks.IScheduleItemUnitOfWork scheduleItemUnitOfWork;

        public Reservation(UnitOfWorks.IReservationUnitOfWork reservationUnitOfWork, UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork, UnitOfWorks.IScheduleItemUnitOfWork scheduleItemUnitOfWork)
        {
            this.reservationUnitOfWork = reservationUnitOfWork;
            this.dailyScheduleUnitOfWork = dailyScheduleUnitOfWork;
            this.scheduleItemUnitOfWork = scheduleItemUnitOfWork;
        }

        public async Task<IQueryable<Models.Reservation>> GetAll()
        {
            return await this.reservationUnitOfWork.Reservation.GetAll();
        }

        public async Task<IQueryable<Models.Reservation>> GetToday()
        {
            var todaySchedule = await this.dailyScheduleUnitOfWork.DailySchedule.Get(DateTime.Now.Date);
            var scheduleItems = 
                await this.scheduleItemUnitOfWork.ScheduleItem.GetScheduleItems(todaySchedule.Id);
            List<Models.Reservation> reservations = new List<Models.Reservation>();

            foreach (var item in scheduleItems)
            {
                reservations.AddRange(await this.reservationUnitOfWork.Reservation.GetAll(item.Id));
            }

            return reservations.AsQueryable();

        }

        public async Task<Models.Reservation> GetById(int id)
        {
            return await this.reservationUnitOfWork.Reservation.Get(id);
        }

        public async Task<IEnumerable<(Models.DailySchedule, IQueryable<Models.Reservation>)>> GetForNextDays(int days = 7)
        {
            List<Models.DailySchedule> dailySchedules = new List<Models.DailySchedule>();

            for (int i = 0; i < days; i++)
            {
                dailySchedules.Add(await this.dailyScheduleUnitOfWork.DailySchedule.Get(DateTime.Now.AddDays(i).Date));
            }

            List<(Models.DailySchedule, IQueryable<Models.Reservation>)> collection =
                new List<(Models.DailySchedule, IQueryable<Models.Reservation>)>();
            foreach (var dailySchedule in dailySchedules)
            {
                var scheduleItems = await this.scheduleItemUnitOfWork.ScheduleItem.GetScheduleItems(dailySchedule.Id);
                List<Models.Reservation> reservationsForDailySchedule = new List<Models.Reservation>();
                foreach (var scheduleItem in scheduleItems)
                {
                    reservationsForDailySchedule.AddRange(
                        await this.reservationUnitOfWork.Reservation.GetAllEnumerable(scheduleItem.Id));
                }

                collection.Add((dailySchedule,reservationsForDailySchedule.AsQueryable()));
            }

            return collection;
        }

        public async Task<Models.Reservation> Create(Models.Reservation newReservation)
        {
            if (await this.reservationUnitOfWork.Reservation.Create(newReservation))
            {
                 if (await this.reservationUnitOfWork.Commit() != 0)
                 {
                     return newReservation;
                 }
                 throw new Exceptions.NotCreatedReservationException(newReservation.UserId, newReservation.ScheduleItemId,
                     "Not created reservation");
            }

            throw new Exceptions.NotCreatedReservationException(newReservation.UserId,newReservation.ScheduleItemId,
                "Not created reservation");
        }

        public async Task<bool> Delete(int reservationId)
        {
            if (await this.reservationUnitOfWork.Reservation.DoesExist(reservationId))
            {
                 await this.reservationUnitOfWork.Reservation.Delete(reservationId);
                 return  await this.reservationUnitOfWork.Commit()!= 0;
            }

            throw new Exceptions.NotFoundReservationException(reservationId, "Not found reservation for delete");
        }
       
    }


    public interface IReservation
    {
        Task<IQueryable<Models.Reservation>> GetAll();

        Task<IQueryable<Models.Reservation>> GetToday();

        Task<Models.Reservation> GetById(int id);

        Task<IEnumerable<(Models.DailySchedule, IQueryable<Models.Reservation>)>> GetForNextDays(int days = 7);

        Task<Models.Reservation> Create(Models.Reservation newReservation);

        Task<bool> Delete(int reservationId);
    }
}