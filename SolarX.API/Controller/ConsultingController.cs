using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IConsultingRequestServices;
using SolarX.SERVICE.Services.ConsultingRequestServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultingController : ControllerBase
    {
        private readonly IConsultingRequestServices _consultingRequestServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public ConsultingController(IConsultingRequestServices consultingRequestServices,
            IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _consultingRequestServices = consultingRequestServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllRequest(ConsultingRequestStatus? consultingRequestStatus,
            ConsultingRequestType? requestType,
            string? searchTerm, int pageIndex, int pageSize)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _consultingRequestServices.GetAllConsultingRequest(Guid.Parse(agencyId), consultingRequestStatus,
                requestType, searchTerm, pageIndex, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsultingRequest([FromForm] RequestModel.CreateConsultingRequest request)
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _consultingRequestServices.CreateConsultingRequest(agencyId, request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{consultingRequestId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateConsultingStatus(Guid consultingRequestId, ConsultingRequestStatus status)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _consultingRequestServices.UpdateConsultingRequestStatus(Guid.Parse(agencyId), consultingRequestId, status)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteConsultingRequest(Guid consultingRequestId)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _consultingRequestServices.DeleteConsultingRequest(Guid.Parse(agencyId), consultingRequestId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}