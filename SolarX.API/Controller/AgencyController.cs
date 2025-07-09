using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IAgencyServices;
using SolarX.SERVICE.Services.AgencyServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly IAgencyServices _agencyServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public AgencyController(IAgencyServices agencyServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _agencyServices = agencyServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> GetAllAgencies(string? searchTerm, int pageIndex, int pageSize)
        {
            var result = await _agencyServices.GetAllAgencies(searchTerm, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> CreateNewAgency([FromForm] RequestModel.CreateAgencyReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _agencyServices.CreateAgency(request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Roles = "SystemAdmin, AgencyAdmin")]
        public async Task<IActionResult> UpdateAgency(Guid agencyId, [FromForm] RequestModel.UpdateAgencyReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _agencyServices.UpdateAgency(agencyId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("credit/{agencyId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateAgencyCredit(Guid agencyId, [FromBody] decimal creditLimit)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _agencyServices.UpdateAgencyCreditLimit(agencyId, creditLimit)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> DeleteAgency(Guid agencyId)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _agencyServices.DeleteAgency(agencyId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}