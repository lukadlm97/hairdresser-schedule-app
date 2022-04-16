using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairdresserScheduleApp.BusinessLogic.Repositories
{
    public class DailySchedule : IDailySchedule
    {
        private readonly AppDbContext context;
        private readonly ILogger<DailySchedule> logger;

        public DailySchedule(AppDbContext context, ILogger<DailySchedule> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Task<IQueryable<Models.DailySchedule>> GetAll()
        {
            return ExecuteInTryCatch<IQueryable<Models.DailySchedule>>(async () =>
            {
                return this.context.DailySchedules;
            }, "GetAll DailySchedules");
        }

        public Task<Models.DailySchedule> Get(int id)
        {
            return ExecuteInTryCatch<Models.DailySchedule>(async () =>
            {
                return await this.context.DailySchedules.FirstOrDefaultAsync(x => x.Id == id);
            }, "GetById DailySchedules");
        }

        public Task<Models.DailySchedule> Get(DateTime pingDateTime)
        {
            return ExecuteInTryCatch<Models.DailySchedule>(async () =>
            {
                return await this.context.DailySchedules.FirstOrDefaultAsync(x => x.Date == pingDateTime);
            }, "GetById DailySchedules");
        }

        public Task<bool> Delete(int id)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingSchedule = await this.context.DailySchedules.FirstOrDefaultAsync(x => x.Id == id);
                if (existingSchedule != null)
                {
                    var scheduleItems =
                          this.context.ScheduleItems.Where(x => x.DailyScheduleId == id);
                    var scheduleItemsIds = scheduleItems.Select(si => si.Id);
                    var reservations = this.context.Reservations.Where(x => scheduleItemsIds.Contains(x.ScheduleItemId));
                    this.context.Reservations.RemoveRange(reservations);
                    this.context.ScheduleItems.RemoveRange(scheduleItems);
                    this.context.DailySchedules.Remove(existingSchedule);
                    return true;
                }

                return true;
            }, "Delete DailySchedules");
        }

        public Task<IQueryable<Models.DailySchedule>> GetFromRange(DateTime fromDay, DateTime toDay)
        {
            return ExecuteInTryCatch<IQueryable<Models.DailySchedule>>(async () =>
            {
                return this.context.DailySchedules.Where(x => x.Date > fromDay && x.Date < toDay);
            }, "GetAll DailySchedules");
        }

        public Task<bool> DoesExist(DateTime pingDateTime)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingSchedule = await this.context.DailySchedules
                    .FirstOrDefaultAsync(x => x.Date == pingDateTime);
                if (existingSchedule != null)
                {
                    return true;
                }

                return false;
            }, "DoesExist DailySchedules");
        }

        public Task<bool> DoesExist(int scheduleId)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingSchedule = await this.context.DailySchedules
                    .FirstOrDefaultAsync(x => x.Id == scheduleId);
                if (existingSchedule != null)
                {
                    return true;
                }

                return false;
            }, "DoesExist DailySchedules");
        }

        public Task<bool> Create(Models.DailySchedule newDateTime)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                await this.context.DailySchedules.AddAsync(newDateTime);

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

    public interface IDailySchedule
    {
        Task<IQueryable<Models.DailySchedule>> GetAll();

        Task<Models.DailySchedule> Get(int id);

        Task<Models.DailySchedule> Get(DateTime pingDateTime);

        Task<bool> Delete(int id);

        Task<IQueryable<Models.DailySchedule>> GetFromRange(DateTime fromDay, DateTime toDay);

        Task<bool> DoesExist(DateTime pingDateTime);

        Task<bool> DoesExist(int scheduleId);

        Task<bool> Create(Models.DailySchedule newDateTime);
    }
}