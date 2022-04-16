namespace HairdresserScheduleApp.BusinessLogic.Services
{
    public class DailySchedule : IDailySchedule
    {
        private readonly UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork;

        public DailySchedule(UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork)
        {
            this.dailyScheduleUnitOfWork = dailyScheduleUnitOfWork;
        }

        public async Task<IQueryable<Models.DailySchedule>> GetAll()
        {
            return await this.dailyScheduleUnitOfWork.DailySchedule.GetAll();
        }

        public async Task<Models.DailySchedule> GetToday()
        {
            if (await this.dailyScheduleUnitOfWork.DailySchedule.DoesExist(DateTime.Now.Date))
            {
                return await this.dailyScheduleUnitOfWork.DailySchedule.Get(DateTime.Now.Date);
            }

            throw new Exceptions.NotFoundDailyScheduleException(DateTime.Now.Date, -1,"For today we haven\'t any schedule");
        }

        public async Task<Models.DailySchedule> GetById(int id)
        {
            return (await GetAll()).FirstOrDefault(x => x.Id == id);
        }

        public async Task<IQueryable<Models.DailySchedule>> GetForNextDays(int days = 7)
        {
            return await this.dailyScheduleUnitOfWork.DailySchedule
                .GetFromRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(days + 1));
        }

        public async Task<Models.DailySchedule> CreateSchedule(Models.DailySchedule newSchedule)
        {
            if (await dailyScheduleUnitOfWork.DailySchedule.DoesExist(newSchedule.Date))
            {
                throw new Exceptions.DailyScheduleExistException(newSchedule.Date, -1,"For selected day schedule exist");
            }

            if (!await dailyScheduleUnitOfWork.DailySchedule.Create(newSchedule) || await dailyScheduleUnitOfWork.Commit() == 0)
            {
                throw new Exceptions.NotCreatedNewDailyScheduleException(newSchedule.Date, "Can\'t create new schedule!!!");
            }

            return newSchedule;
        }

        public async Task<bool> DeleteSchedule(int scheduleId)
        {
            if (!await dailyScheduleUnitOfWork.DailySchedule.DoesExist(scheduleId))
            {
                throw new Exceptions.NotFoundDailyScheduleException(DateTime.MinValue, scheduleId,"For selected day schedule exist");
            }

            await this.dailyScheduleUnitOfWork.DailySchedule.Delete(scheduleId);
            return await this.dailyScheduleUnitOfWork.Commit()!=0;
        }
    }

    public interface IDailySchedule
    {
        Task<IQueryable<Models.DailySchedule>> GetAll();

        Task<Models.DailySchedule> GetToday();
        Task<Models.DailySchedule> GetById(int id);

        Task<IQueryable<Models.DailySchedule>> GetForNextDays(int days = 7);

        Task<Models.DailySchedule> CreateSchedule(Models.DailySchedule newSchedule);

        Task<bool> DeleteSchedule(int scheduleId);
    }
}