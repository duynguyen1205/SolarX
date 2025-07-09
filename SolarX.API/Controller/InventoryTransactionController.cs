using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IInventoryTransactionServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionServices _inventoryTransactionServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public InventoryTransactionController(IInventoryTransactionServices inventoryTransactionServices,
            IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _inventoryTransactionServices = inventoryTransactionServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet("{inventoryId:guid}/{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetInventoryTransaction(Guid inventoryId, Guid productId, DateTimeOffset? dateCharge,
            string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _inventoryTransactionServices.GetInventoryTransaction(Guid.Parse(agencyId), inventoryId, productId,
                dateCharge,
                searchTerm, (int)pageIndex!, (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{transactionId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateTransaction(Guid transactionId, [FromBody] int quantityChanged)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _inventoryTransactionServices.UpdateInventoryTransaction(transactionId, quantityChanged)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}