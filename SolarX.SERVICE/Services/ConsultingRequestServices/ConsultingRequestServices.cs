using System.Text.Json;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using SolarX.SERVICE.Abstractions.IConsultingRequestServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.ConsultingRequestServices;

public class ConsultingRequestServices : IConsultingRequestServices
{
    private readonly IBaseRepository<ConsultingRequest, Guid> _consultingRequestRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public ConsultingRequestServices(IBaseRepository<ConsultingRequest, Guid> consultingRequestRepository,
        ICloudinaryService cloudinaryService)
    {
        _consultingRequestRepository = consultingRequestRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result> CreateConsultingRequest(Guid agencyId, RequestModel.CreateConsultingRequest request)
    {

        var taskUrl = request.Images.Select(x =>
        {
            var url = _cloudinaryService.UploadFileAsync(x, $"{agencyId}/consulting");
            return url;
        }).ToList();
        var result = await Task.WhenAll(taskUrl);
        var imageUrls = result.ToList();
        var consulting = new ConsultingRequest
        {
            Id = Guid.NewGuid(),
            AgencyId = agencyId,
            Area = request.Area,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Message = request.Note,
            // RequestType = request.Type,
            Length = request.Length,
            Width = request.Width,
            Slope = request.Slope,
            AverageUsageLast3Months = request.AverageUsageLast3Months,
            UsageTime = request.UsageTime,
            MainPurpose = request.MainPurpose,
            ImgUrl = JsonSerializer.Serialize(imageUrls)
        };
        _consultingRequestRepository.AddEntity(consulting);
        return Result.CreateResult("Create consulting request success", 201);
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
        ConsultingRequestStatus? consultingRequestStatus, //ConsultingRequestType? requestType,
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

        // if (requestType != null)
        // {
        //     query = query.Where(x => x.RequestType == requestType);
        // }

        var resultList = await PagedResult<ConsultingRequest>.CreateAsync(query, pageIndex, pageSize);
        var result = resultList.Items.Select(x => new ResponseModel.ConsultingRequestResponseModel(
            x.Id,
            x.FullName,
            x.Area,
            x.Message,
            // x.RequestType.ToString(),
            x.PhoneNumber,
            x.Email,
            x.Status.ToString(),
            x.MainPurpose,
            x.UsageTime,
            x.AverageUsageLast3Months,
            string.IsNullOrEmpty(x.ImgUrl)
                ? []
                : JsonSerializer.Deserialize<List<string>>(x.ImgUrl) ?? [],
            x.Length,
            x.Width,
            x.Slope
        )).ToList();

        var response = new PagedResult<ResponseModel.ConsultingRequestResponseModel>(result, resultList.PageIndex,
            resultList.PageSize,
            resultList.TotalCount);
        return Result<PagedResult<ResponseModel.ConsultingRequestResponseModel>>.CreateResult("Get consulting request success",
            200, response);
    }
}