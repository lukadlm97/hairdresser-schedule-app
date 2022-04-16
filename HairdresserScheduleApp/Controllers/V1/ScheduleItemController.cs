using HairdresserScheduleApp.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairdresserScheduleApp.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ScheduleItemController : ControllerBase
    {
        private readonly IScheduleItem scheduleItem;
        private readonly HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest;

        public ScheduleItemController(IScheduleItem scheduleItem, HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest)
        {
            this.scheduleItem = scheduleItem;
            this.logRequest = logRequest;
        }
        
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            return Ok("Pong!!!");
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("{scheduleId}")]
        public async Task<IActionResult> GetAllDailySchedules(int scheduleId)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            return Ok(await this.scheduleItem.GetScheduleItems(scheduleId));
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("today")]
        public async Task<IActionResult> GetAllDailySchedules()
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            return Ok(await this.scheduleItem.GetScheduleItems(DateTime.Now.Date));
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("forDay")]
        public async Task<IActionResult> GetAllDailySchedules([FromBody]BusinessLogic.Models.DTOs.Input.ScheduleItemDate scheduleItemDate)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            return Ok(await this.scheduleItem.GetScheduleItems(scheduleItemDate.Date));
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BusinessLogic.Models.DTOs.Input.ScheduleItemInput newScheduleItem)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            await this.scheduleItem.CreateScheduleItems(new List<BusinessLogic.Models.DTOs.Input.ScheduleItemInput>() { newScheduleItem });

            return Ok(newScheduleItem);
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpPost("{dailyScheduleId}/createScheduleItems")]
        public async Task<IActionResult> CreateScheduleItems(int dailyScheduleId,[FromBody] IList<BusinessLogic.Models.DTOs.Input.ScheduleItemInput> scheduleItems)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            foreach (var item in scheduleItems)
            {
                item.DailyScheduleId = dailyScheduleId;
            }
            await this.scheduleItem.CreateScheduleItems(scheduleItems);

            return Ok(scheduleItems);
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpDelete("delete/{scheduleItemId}")]
        public async Task<IActionResult> Delete(int scheduleItemId)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            

            return Ok(await this.scheduleItem.DeleteScheduleItem(scheduleItemId));
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpDelete("deleteForSchedule/{scheduleId}")]
        public async Task<IActionResult> DeleteForSchedule(int scheduleId)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";


            return Ok(await this.scheduleItem.DeleteScheduleItems(scheduleId));
        }



    }
}
