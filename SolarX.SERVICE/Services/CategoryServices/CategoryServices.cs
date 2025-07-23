using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.ICategoryServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.CategoryServices;

public class CategoryServices : ICategoryServices
{
    private readonly IBaseRepository<Category, Guid> _categoryRepository;

    public CategoryServices(IBaseRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<PagedResult<ResponseModel.CategoryResponseModel>>> GetCategories(string? searchTerm, int pageIndex,
        int pageSize)
    {
        var query = _categoryRepository.GetAllWithQuery(x => !x.IsDeleted);
        query = query.Include(x => x.Products.Where(z => z.IsActive && !z.IsDeleted));

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) || x.Products.Any(p => p.Name.Contains(searchTerm)));
        }

        var listResult = await PagedResult<Category>.CreateAsync(query, pageIndex, pageSize);

        var response = listResult.Items.Select(x => new ResponseModel.CategoryResponseModel(
            x.Id,
            x.Name,
            x.Products.Select(y => new ResponseModel.ProductViewModel(
                y.Id,
                y.Name,
                y.BasePrice
            )).ToList()
        )).ToList();

        var result = new PagedResult<ResponseModel.CategoryResponseModel>(response, listResult.PageIndex, listResult.PageSize,
            listResult.TotalCount);
        return Result<PagedResult<ResponseModel.CategoryResponseModel>>.CreateResult("Get categories success", 200, result);
    }

    public async Task<Result<PagedResult<ResponseModel.CategoryResponseModel>>> GetCategoriesDetail(Guid categoryId, bool isMarkUp,
        decimal markupPercent,
        string? searchTerm,
        int pageIndex, int pageSize)
    {
        var categoryExist = await _categoryRepository.GetAllWithQuery(x => x.Id == categoryId && !x.IsDeleted)
            .Include(x => x.Products.Where(z => z.IsActive && !z.IsDeleted))
            .FirstOrDefaultAsync();

        if (categoryExist == null)
        {
            return Result<PagedResult<ResponseModel.CategoryResponseModel>>.CreateResult("Category not found", 404, null!);
        }

        var query = _categoryRepository.GetAllWithQuery(x => x.Id == categoryId && !x.IsDeleted);
        query = query.Include(x => x.Products.Where(z => z.IsActive && !z.IsDeleted));

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) ||
                                     x.Products.Any(p => p.Name.Contains(searchTerm)));
        }

        var listResult = await PagedResult<Category>.CreateAsync(query, pageIndex, pageSize);

        var response = listResult.Items.Select(x => new ResponseModel.CategoryResponseModel(
            x.Id,
            x.Name,
            x.Products.Select(y => new ResponseModel.ProductViewModel(
                y.Id,
                y.Name,
                CalculateProductPrice(y.BasePrice, isMarkUp, markupPercent)
            )).ToList()
        )).ToList();

        var result = new PagedResult<ResponseModel.CategoryResponseModel>(response, listResult.PageIndex, listResult.PageSize,
            listResult.TotalCount);
        return Result<PagedResult<ResponseModel.CategoryResponseModel>>.CreateResult("Get categories detail success", 200, result);
    }

    private static decimal CalculateProductPrice(decimal basePrice, bool isMarkUp, decimal markupPercent)
    {
        if (isMarkUp && markupPercent > 0)
        {
            return basePrice + (basePrice * markupPercent / 100);
        }

        return basePrice;
    }

    public async Task<Result> CreateCategory(RequestModel.CreateCategoryReq req)
    {
        var categoryExist = await _categoryRepository.GetAllWithQuery(x => x.Name == req.CategoryName).FirstOrDefaultAsync();

        if (categoryExist != null)
        {
            if (categoryExist.IsDeleted)
            {
                categoryExist.IsDeleted = false;
                _categoryRepository.UpdateEntity(categoryExist);
                return Result.CreateResult("Category restored successfully", 201);

            }
            else
            {
                return Result.CreateResult("Category all ready exist", 400, "Bad Request");
            }
        }

        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = req.CategoryName
        };

        _categoryRepository.AddEntity(newCategory);

        return Result.CreateResult("Create category success", 201);
    }

    public async Task<Result> UpdateCategory(Guid cateogryId, RequestModel.CreateCategoryReq req)
    {
        var categoryExist = await _categoryRepository.GetById(cateogryId);
        if (categoryExist == null || categoryExist.IsDeleted)
        {
            return Result.CreateResult("Category not found", 404, "Not Found");
        }

        if (categoryExist.Name != req.CategoryName)
        {
            categoryExist.Name = req.CategoryName;
        }

        _categoryRepository.UpdateEntity(categoryExist);

        return Result.CreateResult("Update category success", 200, "OK");
    }

    public async Task<Result> DeleteCategory(Guid cateogryId)
    {
        var categoryExist = await _categoryRepository.GetById(cateogryId);
        if (categoryExist == null || categoryExist.IsDeleted)
        {
            return Result.CreateResult("Category not found", 404, "Not Found");
        }

        _categoryRepository.RemoveEntity(categoryExist);

        return Result.CreateResult("Delete category success", 200, "OK");
    }
}