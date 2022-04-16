using HairdresserScheduleApp.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.V1;

namespace HairdresserScheduleApp.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DailyScheduleController:ControllerBase
    {
        private readonly IDailySchedule dailySchedule;
        private readonly HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest;

        public DailyScheduleController(HairdresserScheduleApp.BusinessLogic.Services.IDailySchedule dailySchedule, HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest)
        {
            this.dailySchedule = dailySchedule;
            this.logRequest = logRequest;
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllDailySchedules()
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetAllDailySchedules)}->\n";
            return Ok(await this.dailySchedule.GetAll());
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayDailySchedules()
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetTodayDailySchedules)}->\n";
            return Ok(await this.dailySchedule.GetToday());
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("{days}")]
        public async Task<IActionResult> GetTodayDailySchedules(int days)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(GetTodayDailySchedules)}->\n";
            return Ok(await this.dailySchedule.GetForNextDays(days));
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateDailySchedules(
            [FromBody]HairdresserScheduleApp.BusinessLogic.Models.DailySchedule newDailySchedule)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(CreateDailySchedules)}->\n";
            return Ok(await this.dailySchedule.CreateSchedule(newDailySchedule));
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDailySchedules(int id)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(DeleteDailySchedules)}->\n";
            return Ok(await this.dailySchedule.DeleteSchedule(id));
        }

    }
}
