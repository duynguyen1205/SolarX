using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IBlogServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.BlogServices;

public class BlogServices : IBlogServices
{
    private readonly IBaseRepository<BlogPost, Guid> _blogRepository;

    public BlogServices(IBaseRepository<BlogPost, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
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

        var blogPost = resultList.Items.Select(x => new ResponseModel.BlogResponseModel(x.Id, x.Tittle, x.Content)).ToList();

        var response =
            new PagedResult<ResponseModel.BlogResponseModel>(blogPost, resultList.TotalCount, resultList.PageIndex,
                resultList.PageSize);
        return Result<PagedResult<ResponseModel.BlogResponseModel>>.CreateResult("Get blog success", 200, response);
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

        var newBlog = new BlogPost
        {
            Id = Guid.NewGuid(),
            AgencyId = agencyId,
            Tittle = request.Title,
            Content = request.Content
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