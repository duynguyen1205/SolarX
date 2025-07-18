using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IBlogServices;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.BlogServices;

public class BlogServices : IBlogServices
{
    private readonly IBaseRepository<BlogPost, Guid> _blogRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public BlogServices(IBaseRepository<BlogPost, Guid> blogRepository, ICloudinaryService cloudinaryService)
    {
        _blogRepository = blogRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<PagedResult<ResponseModel.BlogResponseModel>>> GetBlog(Guid agencyId, string? searchTerm, int pageIndex,
        int pageSize)
    {
        var query = _blogRepository.GetAllWithQuery(x => x.AgencyId == agencyId && !x.IsDeleted);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Tittle.Contains(searchTerm));
        }

        var resultList = await PagedResult<BlogPost>.CreateAsync(query, pageIndex, pageSize);

        var blogPost = resultList.Items.Select(x => new ResponseModel.BlogResponseModel(x.Id, x.Tittle, x.ThumbnailUrl, x.CreatedAt))
            .ToList();

        var response =
            new PagedResult<ResponseModel.BlogResponseModel>(blogPost, resultList.TotalCount, resultList.PageIndex,
                resultList.PageSize);
        return Result<PagedResult<ResponseModel.BlogResponseModel>>.CreateResult("Get blog success", 200, response);
    }

    public async Task<Result<ResponseModel.BlogResponseDetail>> GetBlogDetail(Guid blogId)
    {
        var blogExisting =
            await _blogRepository.GetAllWithQuery(x => x.Id == blogId && !x.IsDeleted)
                .FirstOrDefaultAsync();
        if (blogExisting == null)
        {
            return Result<ResponseModel.BlogResponseDetail>.CreateResult("Blog not found", 404, null!);
        }

        var response = new ResponseModel.BlogResponseDetail(blogExisting.Id, blogExisting.Tittle, blogExisting.ThumbnailUrl,
            blogExisting.Content, blogExisting.Author, blogExisting.CreatedAt);
        return Result<ResponseModel.BlogResponseDetail>.CreateResult("Get blog detail success", 200, response);
    }

    public async Task<Result> CreateBlog(Guid agencyId, RequestModel.CreateBlogReq request)
    {
        var blogExisting =
            await _blogRepository.GetAllWithQuery(x => x.AgencyId == agencyId && x.Tittle == request.Title && !x.IsDeleted)
                .FirstOrDefaultAsync();
        if (blogExisting != null)
        {
            return Result.CreateResult("Blog already exist", 400);
        }

        string imageUrl = null!;
        if (request.Image != null)
        {
            imageUrl = await _cloudinaryService.UploadFileAsync(request.Image, $"{agencyId}/blog");
        }

        var newBlog = new BlogPost
        {
            Id = Guid.NewGuid(),
            AgencyId = agencyId,
            Tittle = request.Title,
            Content = request.Content,
            Author = request.AuthorName,
            ThumbnailUrl = imageUrl
        };

        _blogRepository.AddEntity(newBlog);
        return Result.CreateResult("Create blog success", 201);
    }

    public async Task<Result> UpdateBlog(Guid agencyId, Guid blogId, RequestModel.UpdateBlogReq request)
    {
        var blog = await _blogRepository.GetById(blogId);
        if (blog == null || blog.IsDeleted)
        {
            return Result.CreateResult("Blog not found", 404);
        }

        if (blog.AgencyId != agencyId)
        {
            return Result.CreateResult("Blog not belong to agency", 400);
        }

        if (request.Title != null && request.Title != blog.Tittle)
        {
            blog.Tittle = request.Title;
        }

        if (request.Content != null && request.Content != blog.Content)
        {
            blog.Content = request.Content;
        }

        if (request.Image != null)
        {
            var imageUrl = await _cloudinaryService.UploadFileAsync(request.Image, $"{agencyId}/blog");
            if (blog.ThumbnailUrl != null)
            {
                await _cloudinaryService.DeleteFile(blog.ThumbnailUrl);
            }

            blog.ThumbnailUrl = imageUrl;
        }

        if (request.AuthorName != null && request.AuthorName != blog.Author)
        {
            blog.Author = request.AuthorName;
        }

        _blogRepository.UpdateEntity(blog);
        return Result.CreateResult("Update blog success", 201);
    }

    public async Task<Result> DeleteBlog(Guid agencyId, Guid blogId)
    {
        var blog = await _blogRepository.GetById(blogId);
        if (blog == null || blog.IsDeleted)
        {
            return Result.CreateResult("Blog not found", 404);
        }

        if (blog.AgencyId != agencyId)
        {
            return Result.CreateResult("Blog not belong to agency", 400);
        }

        _blogRepository.RemoveEntity(blog);
        return Result.CreateResult("Delete blog success", 201);
    }
}