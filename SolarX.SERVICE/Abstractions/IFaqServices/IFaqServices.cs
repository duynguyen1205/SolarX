using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.FaqServices;

namespace SolarX.SERVICE.Abstractions.IFaqServices;

public interface IFaqServices
{
    Task<Result<PagedResult<ResponseModel.FaqResponseModel>>> GetFaq(string? searchTerm, int pageIndex, int pageSize);
    Task<Result> CreateFaq(RequestModel.CreateFaqReq request);
    Task<Result> UpdateFaq(Guid faqId, RequestModel.UpdateFaqReq request);
    Task<Result> DeleteFaq(Guid faqId);
}