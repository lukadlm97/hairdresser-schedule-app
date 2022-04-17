using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairdresserScheduleApp.BusinessLogic.Repositories
{
    public class Reservation:IReservation
    {
        private readonly AppDbContext context;
        private readonly ILogger<Reservation> logger;

        public Reservation(AppDbContext context, ILogger<Reservation> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<IQueryable<Models.Reservation>> GetAll()
        {
            return ExecuteInTryCatch<IQueryable<Models.Reservation>>(async () =>
            {
                return this.context.Reservations;
            }, "GetAll DailySchedules");
        }

        public Task<IQueryable<Models.Reservation>> GetAll(int scheduleItem)
        {
            return ExecuteInTryCatch<IQueryable<Models.Reservation>>(async () =>
            {
                return this.context.Reservations.Where(x=>x.ScheduleItemId==scheduleItem);
            }, "GetAll DailySchedules");
        }

        public Task<IEnumerable<Models.Reservation>> GetAllEnumerable(int scheduleItem)
        {
            return ExecuteInTryCatch<IEnumerable<Models.Reservation>>(async () =>
            {
                return this.context.Reservations.Where(x => x.ScheduleItemId == scheduleItem);
            }, "GetAll DailySchedules");
        }

        public Task<Models.Reservation> Get(int id)
        {
            return ExecuteInTryCatch<Models.Reservation>(async () =>
            {
                return await this.context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
            }, "GetById DailySchedules");
        }
        

        public Task<bool> Delete(int id)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingReservation = await this.context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
                if (existingReservation != null)
                {
                   
                    this.context.Reservations.Remove(existingReservation);
                    return true;
                }

                return true;
            }, "Delete DailySchedules");
        }
        
        
        public Task<bool> DoesExist(int reservationId)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingReservatione = await this.context.Reservations
                    .FirstOrDefaultAsync(x => x.Id == reservationId);
                if (existingReservatione != null)
                {
                    return true;
                }

                return false;
            }, "DoesExist DailySchedules");
        }

        public Task<bool> Create(Models.Reservation newReservation)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == newReservation.UserId);
                var scheduleItem = await context.ScheduleItems.FirstOrDefaultAsync(x => x.Id == newReservation.ScheduleItemId);

                if (user == null || scheduleItem == null)
                {
                    throw new Exception();
                }

                if (await context.Reservations.AnyAsync(x => x.ScheduleItemId == newReservation.ScheduleItemId))
                {
                    throw new Exceptions.NotAvailableScheduleItemException("For selected schedule item reservation exist");
                }

                newReservation.User = user;
                newReservation.ScheduleItem = scheduleItem;
                await this.context.Reservations.AddAsync(newReservation);

                return true;
            }, "DoesExist DailySchedules");
        }
        private Task<T> ExecuteInTryCatch<T>(Func<Task<T>> databaseFunction, string errorMessage)
        {
            try
            {
                return databaseFunction();
            }
            catch (Exception e)
            {
                logger.LogError(e, errorMessage);
                throw;
            }
        }
    }

    public interface IReservation
    {
        Task<IQueryable<Models.Reservation>> GetAll();
        Task<IQueryable<Models.Reservation>> GetAll(int scheduleItem);
        Task<IEnumerable<Models.Reservation>> GetAllEnumerable(int scheduleItem);

        Task<Models.Reservation> Get(int id);
        
        Task<bool> Delete(int id);
        

        Task<bool> DoesExist(int reservationId);

        Task<bool> Create(Models.Reservation newReservation);
    }
}
