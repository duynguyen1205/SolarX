using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IAuthServices;
using SolarX.SERVICE.Services.AuthServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public AuthController(IAuthServices authServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _authServices = authServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpPost]
        public async Task<IActionResult> Login(RequestModel.LoginRequest request)
        {
            var result = await _authServices.Login(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("register")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> RegisterByAdmin( RequestModel.RegisterRequest request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _authServices.AdminCreateAccount(request)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}