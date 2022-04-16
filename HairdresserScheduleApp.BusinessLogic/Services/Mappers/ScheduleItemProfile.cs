using AutoMapper;

namespace HairdresserScheduleApp.BusinessLogic.Services.Mappers
{
    public class ScheduleItemProfile : Profile
    {
       
        public ScheduleItemProfile()
        {
            CreateMap<Models.DTOs.Input.ScheduleItemInput, Models.ScheduleItem>();
        }
    }
}