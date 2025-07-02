using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.ICategoryServices;
using SolarX.SERVICE.Services.CategoryServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public CategoryController(ICategoryServices categoryServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _categoryServices = categoryServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = await _categoryServices.GetCategories(searchTerm, (int)pageIndex!, (int)pageSize!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> CreateCategory(RequestModel.CreateCategoryReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _categoryServices.CreateCategory(request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{categoryId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, RequestModel.CreateCategoryReq request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _categoryServices.UpdateCategory(categoryId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{categoryId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _categoryServices.DeleteCategory(categoryId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}