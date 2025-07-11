using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IOrderServices;
using SolarX.SERVICE.Services.OrderServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public OrderController(IOrderServices orderServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _orderServices = orderServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders(string? searchTerm, DateTimeOffset? dateOrder, OrderStatus? status,
            DeliveryStatus? deliveryStatus,
            bool seller = false, int? pageIndex = 1, int? pageSize = 10)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _orderServices.GetAllOrder(Guid.Parse(agencyId), searchTerm, dateOrder, status, deliveryStatus, seller,
                (int)pageIndex!,
                (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{orderId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var result = await _orderServices.GetOrderDetail(orderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("statistics")]
        [Authorize]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _orderServices.GetOrderStatistics(Guid.Parse(agencyId));
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("{orderId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, RequestModel.UpdateOrderStatusReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _orderServices.UpdateOrderStatus(orderId, request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("public")]
        public async Task<IActionResult> CreateOrderPublic([FromBody] RequestModel.PublicOrderRequest request)
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _orderServices.CreateB2COrder(agencyId, request)
            );

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("B2B/{sellerId:guid}")]
        [Authorize]
        public async Task<IActionResult> CreateOrderB2B(Guid sellerId, [FromBody] RequestModel.CreateOrderReq request)
        {
            var buyerAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _orderServices.CreateB2BOrder(sellerId, buyerAgencyId, request)
            );

            return StatusCode(result.StatusCode, result);
        }
    }
}