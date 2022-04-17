using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace HairdresserScheduleApp.BusinessLogic.Services
{
    public class ScheduleItem:IScheduleItem
    {
        private readonly UnitOfWorks.IScheduleItemUnitOfWork scheduleItemUnitOfWork;
        private readonly UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork;
        private readonly IDailySchedule dailyScheduleService;
        private readonly IMapper mapper;

        public ScheduleItem(UnitOfWorks.IScheduleItemUnitOfWork scheduleItemUnitOfWork, UnitOfWorks.IDailyScheduleUnitOfWork dailyScheduleUnitOfWork, IMapper mapper, IDailySchedule dailyScheduleService)
        {
            this.dailyScheduleUnitOfWork = dailyScheduleUnitOfWork;
            this.scheduleItemUnitOfWork = scheduleItemUnitOfWork;
            this.dailyScheduleService = dailyScheduleService;
            this.mapper = mapper;
        }

        public async Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(int scheduleId)
        {
            return await this.scheduleItemUnitOfWork.ScheduleItem.GetScheduleItems(scheduleId);
        }

        public async Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(DateTime date)
        {
            var dailySchedule= await this.dailyScheduleUnitOfWork.DailySchedule.Get(date.Date);
            return await this.scheduleItemUnitOfWork.ScheduleItem.GetScheduleItems(dailySchedule.Id);
        }

        public async Task<IEnumerable<Models.ScheduleItem>> CreateScheduleItems(IEnumerable<Models.DTOs.Input.ScheduleItemInput> scheduleItems)
        {
            List<Models.ScheduleItem> listScheduleItems = new List<Models.ScheduleItem>();
            foreach (var item in scheduleItems)
            {
                var schedule = await this.dailyScheduleService.GetById(item.DailyScheduleId);
                var newItem = mapper.Map<Models.ScheduleItem>(item);
                newItem.StartTime = schedule.Date.AddHours(GetHours(item.Start)).AddMinutes(GetMinutes(item.Start));
                newItem.EndTime = schedule.Date.AddHours(GetHours(item.End)).AddMinutes(GetMinutes(item.End));
                listScheduleItems.Add(newItem);
            }

            await this.scheduleItemUnitOfWork.ScheduleItem.CreateScheduleItems(listScheduleItems);
            if (await this.scheduleItemUnitOfWork.Commit() != 0)
            {
                return listScheduleItems;
            }

            throw new Exception("Problem on insertion");
        }

        private int GetHours(string content)
        {
            var spplited = content.Split(':');
            int result = 0;
            if (int.TryParse(spplited[0], out result))
            {
                return result;
            }

            throw new Exception();
        }
        private int GetMinutes(string content)
        {
            var spplited = content.Split(':');
            int result = 0;
            if (int.TryParse(spplited[1], out result))
            {
                return result;
            }

            throw new Exception();
        }

        public async Task<bool> DeleteScheduleItem(int scheduleItemId)
        {
             await this.scheduleItemUnitOfWork.ScheduleItem.DeleteScheduleItem(scheduleItemId);
             return await this.scheduleItemUnitOfWork.Commit()!=0;
        }

        public async Task<bool> DeleteScheduleItems(int scheduleId)
        {
            await this.scheduleItemUnitOfWork.ScheduleItem.DeleteScheduleItems(scheduleId);
            return await this.scheduleItemUnitOfWork.Commit() != 0;
        }
    }

    public interface IScheduleItem
    {
        Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(int scheduleId);
        Task<IQueryable<Models.ScheduleItem>> GetScheduleItems(DateTime date);
        Task<IEnumerable<Models.ScheduleItem>> CreateScheduleItems(IEnumerable<Models.DTOs.Input.ScheduleItemInput> scheduleItems);
        Task<bool> DeleteScheduleItem(int scheduleItemId);
        Task<bool> DeleteScheduleItems(int scheduleId);
    }
}
