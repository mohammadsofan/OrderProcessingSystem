using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var result = await _orderService.GetAllByFilterAsync();
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
        [HttpGet("{id}", Name = "GetOrderByIdAsync")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var result = await _orderService.GetOneByFilterAsync(o => o.Id == id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderRequestDto orderRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _orderService.AddAsync(orderRequest);
            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetOrderByIdAsync), new { id = result.Data?.Id }, result.Data);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(int id, [FromBody] OrderRequestDto orderRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _orderService.UpdateAsync(id, orderRequest);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure($"Unexpected error occured {result.Message}"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            var result = await _orderService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
    }
}
