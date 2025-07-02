using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.CategoryServices;

namespace SolarX.SERVICE.Abstractions.ICategoryServices;

public interface ICategoryServices
{
    Task<Result<PagedResult<ResponseModel.CategoryResponseModel>>> GetCategories(string? searchTerm, int pageIndex, int pageSize);
    public Task<Result> CreateCategory(RequestModel.CreateCategoryReq req);
    public Task<Result> UpdateCategory(Guid id, RequestModel.CreateCategoryReq req);
    public Task<Result> DeleteCategory(Guid id);
}