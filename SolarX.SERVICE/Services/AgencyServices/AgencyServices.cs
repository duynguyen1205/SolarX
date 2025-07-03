using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IAgencyServices;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.AgencyServices;

public class AgencyServices : IAgencyServices
{
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public AgencyServices(IBaseRepository<Agency, Guid> agencyRepository, ICloudinaryService cloudinaryService)
    {
        _agencyRepository = agencyRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<PagedResult<ResponseModel.AgencyResponseModel>>> GetAllAgencies(string? searchTerm, int pageIndex,
        int pageSize)
    {
        var query = _agencyRepository.GetAllWithQuery(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) || x.Hotline.Contains(searchTerm));
        }

        var listResult = await PagedResult<Agency>.CreateAsync(query, pageIndex, pageSize);

        var result = listResult.Items.Select(x => new ResponseModel.AgencyResponseModel(
            x.Id,
            x.Name,
            x.Slug,
            x.LogoUrl,
            x.BannerUrl,
            x.ThemeColor,
            x.Hotline,
            x.DisplayWithMarkup
        )).ToList();

        var response = new PagedResult<ResponseModel.AgencyResponseModel>(result, listResult.PageIndex, listResult.PageSize,
            listResult.TotalCount);
        return Result<PagedResult<ResponseModel.AgencyResponseModel>>.CreateResult("Get agencies success", 200, response);
    }

    public async Task<Result> CreateAgency(RequestModel.CreateAgencyReq request)
    {
        var existingAgency =
            await _agencyRepository.GetAllWithQuery(x => x.Slug == request.Slug)
                .FirstOrDefaultAsync();
        var folderName = $"agency-{request.Name}";
        switch (existingAgency)
        {
            case { IsDeleted: true }:
            {
                await Task.WhenAll(
                    _cloudinaryService.DeleteFile(existingAgency.LogoUrl),
                    _cloudinaryService.DeleteFile(existingAgency.BannerUrl)
                );

                var uploadTasksUpdate = new[]
                {
                    _cloudinaryService.UploadFileAsync(request.LogoUrl, folderName),
                    _cloudinaryService.UploadFileAsync(request.BannerUrl, folderName)
                };

                await Task.WhenAll(uploadTasksUpdate);
                existingAgency.Name = request.Name;
                existingAgency.Slug = request.Slug;
                existingAgency.ThemeColor = request.ThemeColor;
                existingAgency.Hotline = request.Hotline;
                existingAgency.MarkupPercent = request.MarkupPercent;
                existingAgency.DisplayWithMarkup = request.DisplayWithMarkup;
                existingAgency.IsDeleted = false;
                existingAgency.LogoUrl = uploadTasksUpdate[0].Result;
                existingAgency.BannerUrl = uploadTasksUpdate[1].Result;

                _agencyRepository.UpdateEntity(existingAgency);
                return Result.CreateResult("Create Agency successfully", 201);
            }
            case { IsDeleted: false }:
                return Result.CreateResult("Agency Slug all ready exist", 400);
        }


        var uploadTasks = new[]
        {
            _cloudinaryService.UploadFileAsync(request.LogoUrl, folderName),
            _cloudinaryService.UploadFileAsync(request.BannerUrl, folderName)
        };

        await Task.WhenAll(uploadTasks);

        var newAgency = new Agency
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            ThemeColor = request.ThemeColor,
            Hotline = request.Hotline,
            MarkupPercent = request.MarkupPercent,
            DisplayWithMarkup = request.DisplayWithMarkup,
            LogoUrl = uploadTasks[0].Result,
            BannerUrl = uploadTasks[1].Result
        };

        _agencyRepository.AddEntity(newAgency);
        return Result.CreateResult("Create Agency successfully", 201);
    }

    public async Task<Result> UpdateAgency(Guid agencyId, RequestModel.UpdateAgencyReq request)
    {
        var existingAgency =
            await _agencyRepository.GetAllWithQuery(x => x.Id == agencyId && !x.IsDeleted).FirstOrDefaultAsync();
        if (existingAgency == null)
        {
            return Result.CreateResult("Agency not found", 404);
        }

        if (request.Name != null && existingAgency.Name != request.Name)
        {
            existingAgency.Name = request.Name;
        }

        if (request.ThemeColor != null && existingAgency.ThemeColor != request.ThemeColor)
        {
            existingAgency.ThemeColor = request.ThemeColor;
        }

        if (request.Hotline != null && existingAgency.Hotline != request.Hotline)
        {
            existingAgency.Hotline = request.Hotline;
        }

        if (request.MarkupPercent.HasValue &&
            Math.Abs(existingAgency.MarkupPercent - request.MarkupPercent.Value) > 0.0001f)
        {
            existingAgency.MarkupPercent = request.MarkupPercent.Value;
        }

        var folderName = $"agency-{existingAgency.Name}";


        if (request.LogoUrl != null)
        {
            var newLogoUrl = await _cloudinaryService.UploadFileAsync(request.LogoUrl, folderName);


            if (!string.IsNullOrWhiteSpace(newLogoUrl))
            {
                await _cloudinaryService.DeleteFile(existingAgency.LogoUrl);
                existingAgency.LogoUrl = newLogoUrl;
            }

            existingAgency.LogoUrl = newLogoUrl;
        }

        if (request.BannerUrl != null)
        {
            var newBannerUrl = await _cloudinaryService.UploadFileAsync(request.BannerUrl, folderName);
            if (!string.IsNullOrWhiteSpace(newBannerUrl))
            {
                await _cloudinaryService.DeleteFile(existingAgency.LogoUrl);
                existingAgency.BannerUrl = newBannerUrl;
            }

        }

        if (request.DisplayWithMarkup.HasValue && existingAgency.DisplayWithMarkup != request.DisplayWithMarkup)
        {
            existingAgency.DisplayWithMarkup = request.DisplayWithMarkup.Value;
        }

        return Result.CreateResult("Update agency successfully", 200);
    }

    public async Task<Result> DeleteAgency(Guid agencyId)
    {
        var agencyExisting = await _agencyRepository.GetById(agencyId);
        if (agencyExisting == null || agencyExisting.IsDeleted)
        {
            return Result.CreateResult("Agency not found", 400);
        }

        _agencyRepository.RemoveEntity(agencyExisting);
        return Result.CreateResult("Delete agency successfully", 200);
    }

    public async Task<Agency?> GetBySlugAsync(string slug)
    {
        var agency = await _agencyRepository.GetAllWithQuery(x => x.Slug == slug && !x.IsDeleted).FirstOrDefaultAsync();
        if (agency == null)
            throw new Exception($"Not found Agency \"{slug}\".");
        return agency;

    }
}