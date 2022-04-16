using HairdresserScheduleApp.BusinessLogic.Models.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HairdresserScheduleApp.BusinessLogic.Services;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser userService;
        private readonly LogRequest logRequest;

        public UserController(IUser userService,HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest logRequest)
        {
            this.userService = userService;
            this.logRequest = logRequest;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] HairdresserScheduleApp.BusinessLogic.Models.DTOs.Input.User user)
        {
            this.logRequest.Message += $"Controllers.{nameof(UserController)}.{nameof(Register)}->\n";
            var token = await this.userService.LogIn(user.Username, user.Password);
            if (token == null || string.IsNullOrWhiteSpace(token))
                return NotFound();

            
            return Ok(new
            {
                token = token,
                username = user.Username
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] HairdresserScheduleApp.BusinessLogic.Models.DTOs.Input.User user)
        {
            this.logRequest.Message += $"Controllers.{nameof(UserController)}.{nameof(Register)}->\n";
            var identity = await this.userService.Register(user.Email,user.Username, user.Password);
            if (!identity)
                return UnprocessableEntity(user);

            return Ok();
        }

        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_USER)]
        [HttpGet("verify")]
        public async Task<IActionResult> Verify()
        {
            this.logRequest.Message += $"Controllers.{nameof(UserController)}.{nameof(Verify)}->\n";
            return Ok("Hello world!!!");
        }
        [Authorize(Roles = HairdresserScheduleApp.BusinessLogic.Configurations.Constants.ROLE_ADMIN)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            this.logRequest.Message += $"Controllers.{nameof(UserController)}.{nameof(GetAllUsers)}->\n";
            return Ok(await  this.userService.GetAll());
        }
    }
}
