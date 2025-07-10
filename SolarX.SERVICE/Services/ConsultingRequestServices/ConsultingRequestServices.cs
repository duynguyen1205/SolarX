using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IConsultingRequestServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.ConsultingRequestServices;

public class ConsultingRequestServices : IConsultingRequestServices
{
    private readonly IBaseRepository<ConsultingRequest, Guid> _consultingRequestRepository;

    public ConsultingRequestServices(IBaseRepository<ConsultingRequest, Guid> consultingRequestRepository)
    {
        _consultingRequestRepository = consultingRequestRepository;
    }

    public Task<Result> CreateConsultingRequest(Guid agencyId, RequestModel.CreateConsultingRequest request)
    {
        var consulting = new ConsultingRequest
        {
            Id = Guid.NewGuid(),
            AgencyId = agencyId,
            Area = request.Area,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Message = request.Note,
            RequestType = request.Type
        };
        _consultingRequestRepository.AddEntity(consulting);
        return Task.FromResult(Result.CreateResult("Create consulting request success", 201));
    }

    public async Task<Result> UpdateConsultingRequestStatus(Guid agencyId, Guid consultingId, ConsultingRequestStatus status)
    {
        var consulting = await _consultingRequestRepository.GetById(consultingId);
        if (consulting == null || consulting.IsDeleted)
        {
            return Result.CreateResult("Consulting request not found", 404);
        }

        if (consulting.AgencyId != agencyId)
        {
            return Result.CreateResult("Consulting request not belong to agency", 400);
        }

        consulting.Status = status;
        _consultingRequestRepository.UpdateEntity(consulting);
        return Result.CreateResult("Update consulting request status success", 201);
    }

    public async Task<Result> DeleteConsultingRequest(Guid agencyId, Guid consultingRequestId)
    {
        var consulting = await _consultingRequestRepository.GetById(consultingRequestId);
        if (consulting == null || consulting.IsDeleted)
        {
            return Result.CreateResult("Consulting request not found", 404);
        }

        if (consulting.AgencyId != agencyId)
        {
            return Result.CreateResult("Consulting request not belong to agency", 400);
        }

        consulting.IsDeleted = true;
        _consultingRequestRepository.UpdateEntity(consulting);
        return Result.CreateResult("Delete consulting request success", 201);
    }

    public async Task<Result<PagedResult<ResponseModel.ConsultingRequestResponseModel>>> GetAllConsultingRequest(Guid agencyId,
        ConsultingRequestStatus? consultingRequestStatus, ConsultingRequestType? requestType,
        string? searchTerm, int pageIndex, int pageSize)
    {
        var query = _consultingRequestRepository.GetAllWithQuery(x => x.AgencyId == agencyId && !x.IsDeleted);
        if (consultingRequestStatus != null)
        {
            query = query.Where(x => x.Status == consultingRequestStatus);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Email.Contains(searchTerm) || x.PhoneNumber.Contains(searchTerm));
        }

        if (requestType != null)
        {
            query = query.Where(x => x.RequestType == requestType);
        }

        var resultList = await PagedResult<ConsultingRequest>.CreateAsync(query, pageIndex, pageSize);
        var result = resultList.Items.Select(x => new ResponseModel.ConsultingRequestResponseModel(
            x.Id,
            x.FullName,
            x.Area,
            x.Message,
            x.RequestType.ToString(),
            x.PhoneNumber,
            x.Email,
            x.Status.ToString()
        )).ToList();

        var response = new PagedResult<ResponseModel.ConsultingRequestResponseModel>(result, resultList.PageIndex,
            resultList.PageSize,
            resultList.TotalCount);
        return Result<PagedResult<ResponseModel.ConsultingRequestResponseModel>>.CreateResult("Get consulting request success",
            200, response);
    }
}