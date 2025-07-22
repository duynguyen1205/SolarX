using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarX.API.Behaviors;
using SolarX.SERVICE.Abstractions.IProductServices;
using SolarX.SERVICE.Services.ProductServices;

namespace SolarX.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly IGlobalTransactionsBehaviors _globalTransactionsBehaviors;

        public ProductController(IProductServices productServices, IGlobalTransactionsBehaviors globalTransactionsBehaviors)
        {
            _productServices = productServices;
            _globalTransactionsBehaviors = globalTransactionsBehaviors;
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetProductDetail(Guid productId)
        {
            var result = await _productServices.GetProductDetail(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> CreateProduct([FromForm] RequestModel.ProductRequest request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _productServices.CreateProduct(request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] RequestModel.UpdateProductRequest request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _productServices.UpdateProduct(productId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("properties/{productSpecificationId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> UpdateProductProperties(Guid productSpecificationId,
            RequestModel.UpdateProductSpecificationRequest request)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _productServices.UpdateProductSpecification(productSpecificationId, request)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("properties/{productSpecificationId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> DeleteProductProperties(Guid productSpecificationId)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _productServices.DeleteProductSpecification(productSpecificationId)
            );
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var result = await _globalTransactionsBehaviors.ExecuteInTransactionAsync(async () =>
                await _productServices.DeleteProduct(productId)
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}