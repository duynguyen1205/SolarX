using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IAgencyServices;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.AgencyServices;

public class AgencyServices : IAgencyServices
{
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;
    private readonly IBaseRepository<AgencyWallet, Guid> _agencyWalletRepository;
    private readonly ICloudinaryService _cloudinaryService;


    public AgencyServices(IBaseRepository<Agency, Guid> agencyRepository, IBaseRepository<AgencyWallet, Guid> agencyWalletRepository,
        ICloudinaryService cloudinaryService)
    {
        _agencyRepository = agencyRepository;
        _agencyWalletRepository = agencyWalletRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<PagedResult<ResponseModel.AgencyResponseModel>>> GetAllAgencies(string? searchTerm, int pageIndex,
        int pageSize)
    {
        var query = _agencyRepository.GetAllWithQuery(x => !x.IsDeleted && x.Slug != "admin");
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) || x.Hotline.Contains(searchTerm) || x.Slug.Contains(searchTerm));
        }

        query = query.Include(x => x.DefaultWallet);
        var listResult = await PagedResult<Agency>.CreateAsync(query, pageIndex, pageSize);

        var result = listResult.Items.Select(x => new ResponseModel.AgencyResponseModel(
            x.Id,
            x.Name,
            x.Slug,
            x.LogoUrl,
            x.BannerUrl,
            x.Address,
            x.Email,
            x.ThemeColor,
            x.Hotline,
            x.DisplayWithMarkup,
            x.DefaultWallet!.CreditLimit
        )).ToList();

        var response = new PagedResult<ResponseModel.AgencyResponseModel>(result, listResult.PageIndex, listResult.PageSize,
            listResult.TotalCount);
        return Result<PagedResult<ResponseModel.AgencyResponseModel>>.CreateResult("Get agencies success", 200, response);
    }

    public async Task<Result<ResponseModel.AgencyDetailResponseModel>> GetAllDetail(Guid agencyId)
    {
        var existingAgency =
            await _agencyRepository.GetAllWithQuery(x => x.Id == agencyId && !x.IsDeleted).FirstOrDefaultAsync();
        if (existingAgency == null)
        {
            return Result<ResponseModel.AgencyDetailResponseModel>.CreateResult("Agency not found", 404, null!);
        }

        var response = new ResponseModel.AgencyDetailResponseModel(
            existingAgency.LogoUrl,
            existingAgency.Hotline,
            existingAgency.Email,
            existingAgency.Address,
            existingAgency.Name
        );
        return Result<ResponseModel.AgencyDetailResponseModel>.CreateResult("Get Agency Detail Success", 200, response);
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
                existingAgency.ThemeColor = request.ThemeColor ?? "#000000";
                existingAgency.Hotline = request.Hotline;
                existingAgency.MarkupPercent = request.MarkupPercent ?? 0;
                existingAgency.DisplayWithMarkup = request.DisplayWithMarkup;
                existingAgency.IsDeleted = false;
                existingAgency.LogoUrl = uploadTasksUpdate[0].Result;
                existingAgency.BannerUrl = uploadTasksUpdate[1].Result;
                existingAgency.Address = request.Address;
                existingAgency.Email = request.Email;

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
            ThemeColor = request.ThemeColor ?? "#000000",
            Hotline = request.Hotline,
            MarkupPercent = request.MarkupPercent ?? 0,
            DisplayWithMarkup = request.DisplayWithMarkup,
            LogoUrl = uploadTasks[0].Result,
            BannerUrl = uploadTasks[1].Result,
            Address = request.Address,
            Email = request.Email
        };

        _agencyRepository.AddEntity(newAgency);

        var agencyWaller = new AgencyWallet
        {
            Id = Guid.NewGuid(),
            AgencyId = newAgency.Id,
            CreditLimit = request.CreditLimit ?? 0,
            CurrentDebt = 0,
            Balance = 0
        };
        _agencyWalletRepository.AddEntity(agencyWaller);

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

        if (request.Address != null && existingAgency.Address != request.Address)
        {
            existingAgency.Address = request.Address;
        }

        if (request.Email != null && existingAgency.Email != request.Email)
        {
            existingAgency.Email = request.Email;
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

        _agencyRepository.UpdateEntity(existingAgency);
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

    public async Task<Result> UpdateAgencyCreditLimit(Guid agencyId, decimal creditLimit)
    {
        var agency = await _agencyRepository.GetById(agencyId, x => x.DefaultWallet!);
        if (agency == null || agency.IsDeleted)
        {
            return Result.CreateResult("Agency not found", 404);
        }

        if (creditLimit < 0)
        {
            return Result.CreateResult("Credit limit must be greater than 0", 400);
        }

        var agencyWallet = agency.DefaultWallet;
        if (agencyWallet == null)
        {
            return Result.CreateResult("Agency wallet not found", 404);
        }

        agencyWallet.CreditLimit = creditLimit;
        _agencyWalletRepository.UpdateEntity(agencyWallet);

        return Result.CreateResult("Update agency credit limit successfully", 200);
    }
}