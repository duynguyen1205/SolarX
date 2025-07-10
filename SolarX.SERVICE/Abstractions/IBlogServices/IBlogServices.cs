using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.BlogServices;

namespace SolarX.SERVICE.Abstractions.IBlogServices;

public interface IBlogServices
{
    Task<Result<PagedResult<ResponseModel.BlogResponseModel>>> GetBlog(Guid agencyId, string? searchTerm, int pageIndex, int pageSize);
    Task<Result> CreateBlog(Guid agencyId, RequestModel.CreateBlogReq request);
    Task<Result> UpdateBlog(Guid agencyId, Guid blogId, RequestModel.UpdateBlogReq request);
    Task<Result> DeleteBlog(Guid agencyId, Guid blogId);
}