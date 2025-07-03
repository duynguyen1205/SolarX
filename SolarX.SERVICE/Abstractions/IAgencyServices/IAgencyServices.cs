using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Services.AgencyServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Abstractions.IAgencyServices;

public interface IAgencyServices
{
    Task<Result<PagedResult<ResponseModel.AgencyResponseModel>>> GetAllAgencies(string? searchTerm, int pageIndex, int pageSize);
    Task<Result> CreateAgency(RequestModel.CreateAgencyReq request);
    Task<Result> UpdateAgency(Guid agencyId, RequestModel.UpdateAgencyReq request);
    Task<Result> DeleteAgency(Guid agencyId);
    Task<Agency?> GetBySlugAsync(string slug);
}