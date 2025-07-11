using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.SERVICE.Abstractions.IDashboardServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardServices _dashboardServices;

        public DashboardController(IDashboardServices dashboardServices)
        {
            _dashboardServices = dashboardServices;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "SystemAdmin")] // Có thể thêm role admin nếu cần
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _dashboardServices.GetAdminDashboard();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("branch")]
        [Authorize]
        public async Task<IActionResult> GetBranchDashboard()
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _dashboardServices.GetBranchDashboard(Guid.Parse(agencyId));
            return StatusCode(result.StatusCode, result);
        }

    }
}