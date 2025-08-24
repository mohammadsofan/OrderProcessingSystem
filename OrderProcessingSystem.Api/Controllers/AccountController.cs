using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Wrappers;

namespace OrderProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _userService.RegisterUserAsync(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _userService.LoginUserAsync(request);
            if(result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message,result.Data));
            }
            else if (result.Status == ServiceResultStatus.BadRequest)
            {
                return BadRequest(ApiResponse.Failure("Login failed",new List<string>() {result.Message??"Login failed"}));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
    }
}
