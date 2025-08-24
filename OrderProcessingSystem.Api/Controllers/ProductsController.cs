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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
            
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await _productService.GetAllByFilterAsync();
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError,ApiResponse.Failure("Unexpected error occured"));
        }
        [HttpGet("{id}",Name = "GetProductByIdAsync")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var result = await _productService.GetOneByFilterAsync(p => p.Id == id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Product not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] ProductRequestDto productRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _productService.AddAsync(productRequest);
            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetProductByIdAsync), new { id = result.Data?.Id }, result.Data);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductRequestDto productRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _productService.UpdateAsync(id, productRequest);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Product not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure($"Unexpected error occured {result.Message}"));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else if (result.Status == ServiceResultStatus.NotFound)
            {
                return NotFound(ApiResponse.Failure("Product not found"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occured"));
        }
    }
}
