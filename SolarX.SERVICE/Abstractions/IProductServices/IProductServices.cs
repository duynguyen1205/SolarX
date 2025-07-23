using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.ProductServices;

namespace SolarX.SERVICE.Abstractions.IProductServices;

public interface IProductServices
{
    Task<Result<PagedResult<ResponseModel.GetAllProductResponse>>> GetAllProducts(string? searchTerm, int pageIndex, int pageSize);
    Task<Result<ResponseModel.ProductResponse?>> GetProductDetail(Guid productId);
    Task<Result> CreateProduct(RequestModel.ProductRequest request);
    Task<Result> UpdateProduct(Guid productId, RequestModel.UpdateProductRequest request);
    Task<Result> DeleteProduct(Guid productId);
    Task<Result> UpdateProductSpecification(Guid productSpecificationId, RequestModel.UpdateProductSpecificationRequest request);

    Task<Result> DeleteProductSpecification(Guid productSpecificationId);
}