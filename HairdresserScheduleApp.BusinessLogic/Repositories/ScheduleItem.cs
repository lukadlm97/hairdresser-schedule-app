using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairdresserScheduleApp.BusinessLogic.Repositories
{
    public class ScheduleItem : IScheduleItem
    {
        private readonly AppDbContext context;
        private readonly ILogger<ScheduleItem> logger;

        public ScheduleItem(AppDbContext context, ILogger<ScheduleItem> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(int scheduleId)
        {
            return ExecuteInTryCatch<IQueryable<Models.ScheduleItem>>(async () =>
            {
                return this.context.ScheduleItems.Where(x => x.DailyScheduleId == scheduleId);
            }, "GetScheduleItems ScheduleItem");
        }

        private Task<bool> CreateScheduleItem(Models.ScheduleItem scheduleItem)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingSchedule =
                    await this.context.DailySchedules.FirstOrDefaultAsync(x => x.Id == scheduleItem.DailyScheduleId);
                if (existingSchedule == null)
                {
                    throw new Exceptions.NotFoundDailyScheduleException(scheduleItem.StartTime, scheduleItem.DailyScheduleId,
                        "Not found schedule for this day");
                }

                scheduleItem.DailySchedule = existingSchedule;

                await this.context.ScheduleItems.AddAsync(scheduleItem);

                return true;
            }, "GetScheduleItems ScheduleItem");
        }

        public Task<bool> CreateScheduleItems(IEnumerable<Models.ScheduleItem> scheduleItems)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var controleFlag = true;
                foreach (var scheduleItem in scheduleItems)
                {
                    controleFlag &= await CreateScheduleItem(scheduleItem);
                }

                return controleFlag;
            }, "GetScheduleItems ScheduleItem");
        }

        public Task<bool> DeleteScheduleItem(int scheduleItemId)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingSchedule =
                    await this.context.ScheduleItems.FirstOrDefaultAsync(x => x.Id == scheduleItemId);
                if (existingSchedule == null)
                {
                    throw new Exceptions.NotFoundScheduleItemException(scheduleItemId,
                        "Not found schedule for this day");
                }

                this.context.ScheduleItems.Remove(existingSchedule);

                return true;
            }, "GetScheduleItems ScheduleItem");
        }

        public Task<bool> DeleteScheduleItems(int scheduleId)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var existingScheduleItems =
                     this.context.ScheduleItems.Where(x => x.DailyScheduleId == scheduleId);
                if (existingScheduleItems == null)
                {
                    throw new Exceptions.NotFoundDailyScheduleException(DateTime.MinValue, scheduleId,
                        "Not found schedule for this day");
                }

                this.context.ScheduleItems.RemoveRange(existingScheduleItems);

                return true;
            }, "GetScheduleItems ScheduleItem");
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

    public interface IScheduleItem
    {
        Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(int scheduleId);
        
        Task<bool> CreateScheduleItems(IEnumerable<Models.ScheduleItem> scheduleItems);

        Task<bool> DeleteScheduleItem(int scheduleItemId);

        Task<bool> DeleteScheduleItems(int scheduleId);
    }
}