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
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartItemsAsync()
        {
            var result = await _cartItemService.GetAllByFilterAsync();
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpGet("{id}", Name = "GetCartItemByIdAsync")]
        public async Task<IActionResult> GetCartItemByIdAsync(int id)
        {
            var result = await _cartItemService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Cart item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPost]
        public async Task<IActionResult> AddCartItemAsync([FromBody] CartItemRequestDto cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _cartItemService.AddAsync(cartItemRequest);
            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetCartItemByIdAsync), new { id = result.Data?.Id }, result.Data);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItemAsync(int id, [FromBody] CartItemRequestDto cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _cartItemService.UpdateAsync(id, cartItemRequest);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Cart item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure($"Unexpected error occured {result.Message}"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItemAsync(int id)
        {
            var result = await _cartItemService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Cart item not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
    }
}
