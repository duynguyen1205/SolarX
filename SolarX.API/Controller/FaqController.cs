using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IFaqServices;
using SolarX.SERVICE.Services.FaqServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqServices _faqServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public FaqController(IFaqServices faqServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _faqServices = faqServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaq(string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = await _faqServices.GetFaq(searchTerm, (int)pageIndex!, (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> CreateFaq(RequestModel.CreateFaqReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _faqServices.CreateFaq(request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{faqId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateFaq(Guid faqId, RequestModel.UpdateFaqReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _faqServices.UpdateFaq(faqId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{faqId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> DeleteFaq(Guid faqId)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _faqServices.DeleteFaq(faqId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}