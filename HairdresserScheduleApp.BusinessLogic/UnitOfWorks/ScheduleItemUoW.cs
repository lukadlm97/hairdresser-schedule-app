using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HairdresserScheduleApp.BusinessLogic.Models;
using HairdresserScheduleApp.BusinessLogic.Repositories;
using DailySchedule = HairdresserScheduleApp.BusinessLogic.Repositories.DailySchedule;

namespace HairdresserScheduleApp.BusinessLogic.UnitOfWorks
{
    public class ScheduleItemUoW:IScheduleItemUnitOfWork
    {
        private readonly AppDbContext context;

        public ScheduleItemUoW(AppDbContext dbContext, Repositories.IScheduleItem scheduleItem)
        {
            this.context = dbContext;
            ScheduleItem = scheduleItem;
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public IScheduleItem ScheduleItem { get; set; }
        public async Task<int> Commit()
        {
            return await context.SaveChangesAsync();
        }
    }
    public interface IScheduleItemUnitOfWork : IDisposable
    {
        Repositories.IScheduleItem ScheduleItem { get; set; }

        Task<int> Commit();
    }
}
