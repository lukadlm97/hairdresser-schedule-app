using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairdresserScheduleApp.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly BusinessLogic.Services.IReservation reservation;
        private readonly HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest;

        public ReservationController(BusinessLogic.Services.IReservation reservation, HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest)
        {
            this.reservation = reservation;
            this.logRequest = logRequest;
        }


        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("today")]
        public async Task<IActionResult> GetReservationsForToday()
        {
            this.logRequest.Message += $"Controllers.{nameof(ReservationController)}.{nameof(GetReservationsForToday)}->\n";
            return Ok(await this.reservation.GetToday());
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("nextDays")]
        public async Task<IActionResult> GetReservationsForNextDays([FromBody]int noDays)
        {
            this.logRequest.Message += $"Controllers.{nameof(ReservationController)}.{nameof(GetReservationsForToday)}->\n";
            return Ok(await this.reservation.GetForNextDays(noDays));
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            this.logRequest.Message += $"Controllers.{nameof(ReservationController)}.{nameof(GetReservationsForToday)}->\n";
            return Ok(await this.reservation.GetAll());
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BusinessLogic.Models.DTOs.Input.ReservationInput reservationInput)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(Create)}->\n";

            return Ok(await this.reservation.Create(new Reservation()
            {
                ScheduleItemId = reservationInput.ScheduleItemId,
                UserId = reservationInput.UserId
            }));
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpDelete("delete/{scheduleItemId}")]
        public async Task<IActionResult> Delete(int reservationId)
        {
            this.logRequest.Message += $"Controllers.{nameof(DailyScheduleController)}.{nameof(Delete)}->\n";


            return Ok(await this.reservation.Delete(reservationId));
        }

    }
}
