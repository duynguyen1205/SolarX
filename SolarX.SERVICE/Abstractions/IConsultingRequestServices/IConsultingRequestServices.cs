using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.ConsultingRequestServices;

namespace SolarX.SERVICE.Abstractions.IConsultingRequestServices;

public interface IConsultingRequestServices
{
    Task<Result> CreateConsultingRequest(Guid agencyId, RequestModel.CreateConsultingRequest request);
    Task<Result> UpdateConsultingRequestStatus(Guid agencyId, Guid consultingId, ConsultingRequestStatus status);
    Task<Result> DeleteConsultingRequest(Guid agencyId, Guid consultingRequestId);

    Task<Result<PagedResult<ResponseModel.ConsultingRequestResponseModel>>> GetAllConsultingRequest(Guid agencyId,
        ConsultingRequestStatus? consultingRequestStatus, //ConsultingRequestType? requestType, 
        string? searchTerm,
        int pageIndex, int pageSize);
}