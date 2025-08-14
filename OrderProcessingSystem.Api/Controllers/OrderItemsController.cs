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
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllOrderItemsAsync()
        {
            var result = await _orderItemService.GetAllByFilterAsync();
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpGet("{id}", Name = "GetOrderItemByIdAsync")]
        public async Task<IActionResult> GetOrderItemByIdAsync(int id)
        {
            var result = await _orderItemService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderItemAsync([FromBody] OrderItemRequestDto orderItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _orderItemService.AddAsync(orderItemRequest);
            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetOrderItemByIdAsync), new { id = result.Data?.Id }, result.Data);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItemAsync(int id, [FromBody] OrderItemRequestDto orderItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _orderItemService.UpdateAsync(id, orderItemRequest);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure($"Unexpected error occured {result.Message}"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItemAsync(int id)
        {
            var result = await _orderItemService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Order item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
    }
}
