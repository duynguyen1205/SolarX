using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IInventoryServices;
using RequestModel = SolarX.SERVICE.Services.InventoryServices.RequestModel;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryServices _inventoryServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public InventoryController(IInventoryServices inventoryServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _inventoryServices = inventoryServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        [Authorize(Roles = "SystemAdmin, AgencyAdmin")]
        public async Task<IActionResult> GetInventory(string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            var result = await _inventoryServices.GetInventory(agencyId, searchTerm, (int)pageIndex!, (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ImportInventory(List<RequestModel.InventoryRequestModel> request)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _inventoryServices.CreateInventory(Guid.Parse(agencyId), request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{inventoryId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateInventory(Guid inventoryId, RequestModel.InventoryRequestModel request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _inventoryServices.UpdateAdminInventory(inventoryId, request)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}