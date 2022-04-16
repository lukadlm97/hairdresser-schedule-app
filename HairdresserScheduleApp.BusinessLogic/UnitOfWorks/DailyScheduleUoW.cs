using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HairdresserScheduleApp.BusinessLogic.Models;
using HairdresserScheduleApp.BusinessLogic.Repositories;

namespace HairdresserScheduleApp.BusinessLogic.UnitOfWorks
{
    public class DailyScheduleUoW:IDailyScheduleUnitOfWork
    {
        private readonly AppDbContext context;

        public DailyScheduleUoW(AppDbContext dbContext, Repositories.IDailySchedule dailySchedule)
        {
            this.context = dbContext;
            DailySchedule = dailySchedule;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public IDailySchedule DailySchedule { get; set; }
        public async Task<int> Commit()
        {
            return await context.SaveChangesAsync();
        }
    }

    public  interface IDailyScheduleUnitOfWork : IDisposable
    {
        Repositories.IDailySchedule DailySchedule { get; set; }

        Task<int> Commit();
    }
}
