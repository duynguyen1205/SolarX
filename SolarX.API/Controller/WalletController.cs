using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.SERVICE.Abstractions.IWalletService;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        [Authorize(Roles = "AgencyStaff, AgencyAdmin")]
        public async Task<IActionResult> GetWallet()
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            var result = await _walletService.GetBalanceAsync(agencyId);
            return StatusCode(result.StatusCode, result);
        }
    }
}