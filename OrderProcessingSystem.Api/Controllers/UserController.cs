using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Wrappers;
using System.Security.Claims;

namespace OrderProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductService _productService;
        private readonly ICartItemService _cartItemService;
        private readonly IMessageBrokerService _messageBroker;

        public UserController(IUserService userService,IOrderService orderService,IOrderItemService orderItemService,
            IProductService productService,ICartItemService cartItemService,IMessageBrokerService messageBroker)
        {
            _userService = userService;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productService = productService;
            _cartItemService = cartItemService;
            _messageBroker = messageBroker;
        }
        [HttpGet("cart",Name = "GetUserCartItemsAsync")]
        public async Task<IActionResult> GetUserCartItemsAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _cartItemService.GetAllByFilterAsync(ci => ci.UserId.ToString() == userId,ci=>ci.Product!);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occurred"));
        }
        [HttpPost("cart")]
        public async Task<IActionResult> AddToUserCartAsync([FromBody] UserCartItemRequestDto request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var productResult = await _productService.GetOneByFilterAsync(p => p.Id == request.ProductId);
            if(!productResult.IsSuccess)
            {
                return NotFound(ApiResponse.Failure("Product not found"));
            }
            var cartItemRequest = new CartItemRequestDto
            {
                UserId = int.Parse(userId),
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalPrice = request.Quantity * productResult.Data!.Price
            };
            var result = await _cartItemService.AddAsync(cartItemRequest);
            var addedCartItem = await _cartItemService.GetOneByFilterAsync(ci => ci.UserId.ToString() == userId && ci.ProductId == request.ProductId,ci=>ci.Product!);
            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetUserCartItemsAsync),null, addedCartItem);
            }
            if(result.Status == ServiceResultStatus.Conflict)
            {
                return Conflict(ApiResponse.Failure("Item already exists in cart", new List<string> {"Item already exists in cart" }));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occurred"));
        }
        [HttpPost("order")]
        public async Task<IActionResult> PlaceUserOrderAsync([FromBody] PlaceOrderRequestDto request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failure("Invalid request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var cartItemsResult = await _cartItemService.GetAllByFilterAsync(ci => ci.UserId.ToString() == userId,ci=> ci.Product!);
            if(!cartItemsResult.IsSuccess || cartItemsResult.Data == null || !cartItemsResult.Data.Any())
            {
                return BadRequest(ApiResponse.Failure("Cart is empty or could not be retrieved"));
            }
            var orderRequest = new OrderRequestDto
            {
                UserId = int.Parse(userId),
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItemsResult.Data.Sum(ci => ci.Quantity * ci.Product!.Price),
                PhoneNumber = request.PhoneNumber,
                ShippingAddress = request.ShippingAddress,
                Status = OrderStatus.Pending,
            };
            var result = await _orderService.AddAsync(orderRequest);
            if (result.IsSuccess)
            {
                // i can implement add,remove range but i will do it later (i dont have time)
                // i still did not check if product stock is enough, i will do it later
                // i did not decrement product stock yet, i will do it later
                // and i may use a transaction here, i will do it later or today if i have time
                foreach (var item in cartItemsResult.Data)
                {
                    await _orderItemService.AddAsync(new OrderItemRequestDto
                    {
                        OrderId = result.Data!.Id,
                        ProductId = item.Product!.Id,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    }); 
                    await _cartItemService.DeleteAsync(item.Id);
                }
                var orderPlacedMessage = new OrderPlacedMessage()
                {
                    Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
                    UserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
                    PhoneNumber = request.PhoneNumber,
                    ShippingAddress = request.ShippingAddress,
                    PlacedAt = DateTime.UtcNow,
                    TotalAmount = orderRequest.TotalAmount,
                    Items = cartItemsResult.Data.Select(ci => new OrderPlacedItem
                    {                       
                        ProductName = ci.Product!.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price,
                    }).ToList()
                };
                await _messageBroker.SendMessage<OrderPlacedMessage>(orderPlacedMessage);
                return Created();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occurred"));
        }

        [HttpGet("orders",Name = "GetUserOrdersAsync")]
        public async Task<IActionResult> GetUserOrdersAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _orderService.GetAllByFilterAsync(o => o.UserId.ToString() == userId, o => o.OrderItems!);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Message, result.Data));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure("Unexpected error occurred"));
        }

    }
}
