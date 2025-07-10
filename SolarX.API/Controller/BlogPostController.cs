using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IBlogServices;
using SolarX.SERVICE.Services.BlogServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogServices _blogServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public BlogPostController(IBlogServices blogServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _blogServices = blogServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogPost(string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
        {
            var agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            var result = await _blogServices.GetBlog(agencyId, searchTerm, (int)pageIndex!, (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost(RequestModel.CreateBlogReq request)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _blogServices.CreateBlog(Guid.Parse(agencyId), request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{blogId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogPost(Guid blogId, RequestModel.UpdateBlogReq request)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _blogServices.UpdateBlog(Guid.Parse(agencyId), blogId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{blogId:guid}")]
        public async Task<IActionResult> DeleteBlogPost(Guid blogId)
        {
            var agencyId = User.FindFirst("AgencyId")?.Value!;
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _blogServices.DeleteBlog(Guid.Parse(agencyId), blogId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}