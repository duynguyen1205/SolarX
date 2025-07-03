using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
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

        [HttpPost("public")]
        public async Task<IActionResult> CreateOrderPublic([FromBody] RequestModel.PublicOrderRequest request)
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _orderServices.CreateB2COrder(agencyId, request)
            );

            return StatusCode(result.StatusCode, result);
        }

    }
}