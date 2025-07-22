using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using SolarX.SERVICE.Abstractions.IProductServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.ProductServices;

public class ProductServices : IProductServices
{
    private readonly IBaseRepository<Product, Guid> _productRepository;
    private readonly IBaseRepository<ProductImage, Guid> _productImageRepository;
    private readonly IBaseRepository<ProductSpecification, Guid> _productSpecificationRepository;
    private readonly IBaseRepository<Category, Guid> _categoryRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public ProductServices(IBaseRepository<Product, Guid> productRepository, IBaseRepository<Category, Guid> categoryRepository,
        ICloudinaryService cloudinaryService, IBaseRepository<ProductImage, Guid> productImageRepository,
        IBaseRepository<ProductSpecification, Guid> productSpecificationRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cloudinaryService = cloudinaryService;
        _productImageRepository = productImageRepository;
        _productSpecificationRepository = productSpecificationRepository;
    }

    public async Task<Result<ResponseModel.ProductResponse?>> GetProductDetail(Guid productId)
    {
        var product = await _productRepository.GetAllWithQuery(x => x.Id == productId && !x.IsDeleted)
            .Include(x => x.Images.Where(z => !z.IsDeleted))
            .Include(x => x.Specifications.Where(z => !z.IsDeleted))
            .Include(x => x.Category)
            .Include(x => x.Reviews.Where(z => !z.IsDeleted))
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return Result<ResponseModel.ProductResponse?>.CreateResult("Invalid product", 400, null);
        }

        var response = new ResponseModel.ProductResponse(
            productId,
            product.Name,
            product.Description,
            product.BasePrice,
            product.Category.Name,
            product.Sku,
            product.Images.OrderBy(x => x.ImageIndex).Select(x => new ResponseModel.ProductImageResponse(
                x.Id,
                x.ImageIndex,
                x.ImageUrl
            )).ToList(),
            product.Specifications.Select(x => new ResponseModel.ProductSpecificationResponse(
                x.Id,
                x.Key,
                x.Value
            )).ToList(),
            product.Reviews.Select(x => new ResponseModel.ProductReviewResponse(
                x.Id,
                x.Comment,
                x.Rating,
                x.Customer.FullName
            )).ToList()
        );
        return Result<ResponseModel.ProductResponse?>.CreateResult("Get product detail success", 200, response);
    }

    public async Task<Result> CreateProduct(RequestModel.ProductRequest request)
    {
        var isCategoryExist = await _categoryRepository.GetAllWithQuery(x => x.Id == request.CategoryId && !x.IsDeleted)
            .FirstOrDefaultAsync();
        if (isCategoryExist == null)
        {
            return Result.CreateResult("Category not found", 400);
        }

        var isProductExist = await _productRepository.GetAllWithQuery(x => x.Name == request.ProductName && !x.IsDeleted)
            .FirstOrDefaultAsync();
        if (isProductExist != null)
        {
            return Result.CreateResult("Product all ready exist", 400);
        }

        var taskUrl = request.Images.Select(x =>
        {
            var url = _cloudinaryService.UploadFileAsync(x, "solar-images");
            return url;
        }).ToList();

        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.ProductName,
            Description = request.ProductDescription,
            BasePrice = request.BasePrice,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive,
            Sku = request.Sku
        };
        _productRepository.AddEntity(newProduct);
        var listDeserializeObject = JsonConvert.DeserializeObject<List<RequestModel.ProductSpecification>>(request.Specifications)!;


        foreach (var item in listDeserializeObject.Select(x => new ProductSpecification
                 {
                     Id = Guid.NewGuid(),
                     ProductId = newProduct.Id,
                     Key = x.Key,
                     Value = x.Value
                 }))
        {
            _productSpecificationRepository.AddEntity(item);
        }

        var url = await Task.WhenAll(taskUrl);

        var newMedia = url.Select((x, index) => new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = newProduct.Id,
                ImageUrl = x,
                ImageIndex = index
            }
        ).ToList();
        _productImageRepository.AddBulkAsync(newMedia);
        return Result.CreateResult("Create product successfully", 201);
    }

    public async Task<Result> UpdateProduct(Guid productId, RequestModel.UpdateProductRequest request)
    {
        var product = await _productRepository.GetById(productId);
        if (product == null || product.IsDeleted)
        {
            return Result.CreateResult("Product not found", 400);
        }

        if (request.CategoryId != null && product.CategoryId != request.CategoryId)
        {
            var isCategoryExist = await _categoryRepository.GetAllWithQuery(x => x.Id == request.CategoryId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (isCategoryExist == null)
            {
                return Result.CreateResult("Category not found", 400);
            }

            product.CategoryId = isCategoryExist.Id;
            product.Category = isCategoryExist;
        }

        if (request.ProductName != null && product.Name != request.ProductName)
        {
            product.Name = request.ProductName;
        }

        if (request.ProductDescription != null && product.Description != request.ProductDescription)
        {
            product.Description = request.ProductDescription;
        }

        if (request.BasePrice != null && product.BasePrice != request.BasePrice)
        {
            product.BasePrice = (decimal)request.BasePrice;
        }

        if (request.IsActive != null && product.IsActive != request.IsActive)
        {
            product.IsActive = (bool)request.IsActive;
        }

        switch (request.IndexFile)
        {
            case { Count: > 0 }:
            {
                var oldImg = await _productImageRepository.GetAllWithQuery(x => x.ProductId == product.Id && x.IsDeleted == false)
                    .ToListAsync();

                var updatedImages = new List<ProductImage>();
                var deletedImages = new List<ProductImage>();

                if (request.Img is { Count: > 0 })
                {
                    var indexCount = request.IndexFile.Count;
                    var imgCount = request.Img.Count;

                    for (var i = 0; i < indexCount; i++)
                    {
                        var index = request.IndexFile[i];
                        var file = request.Img.ElementAtOrDefault(i);
                        var targetImage = oldImg.FirstOrDefault(x => x.ImageIndex == index);
                        if (targetImage == null)
                        {
                            return Result.CreateResult("Invalid index", 400);
                        }

                        if (file == null) continue;
                        var url = await _cloudinaryService.UploadFileAsync(file, "solar-images");

                        await _cloudinaryService.DeleteFile(targetImage.ImageUrl);
                        targetImage.ImageUrl = url;
                        updatedImages.Add(targetImage);

                    }

                    if (imgCount > indexCount)
                    {
                        var newIndex = oldImg.Count > 0 ? oldImg.Max(x => x.ImageIndex) + 1 : 0;
                        var newImages = request.Img.Skip(indexCount).ToList();

                        var taskUrl = newImages.Select(x => _cloudinaryService.UploadFileAsync(x, "solar-images")).ToList();
                        var url = await Task.WhenAll(taskUrl);

                        var newMedia = url.Select((x, index) => new ProductImage
                            {
                                Id = Guid.NewGuid(),
                                ProductId = productId,
                                ImageUrl = x,
                                ImageIndex = newIndex + index
                            }
                        ).ToList();
                        _productImageRepository.AddBulkAsync(newMedia);
                    }
                }
                else
                {
                    foreach (var index in request.IndexFile)
                    {
                        oldImg[index].IsDeleted = true;
                        deletedImages.Add(oldImg[index]);
                        await _cloudinaryService.DeleteFile(oldImg[index].ImageUrl);
                    }
                }


                if (updatedImages.Count != 0)
                {
                    _productImageRepository.UpdateBulk(updatedImages);
                }

                if (deletedImages.Count != 0)
                {
                    _productImageRepository.UpdateBulk(deletedImages);
                }

                break;
            }
            case null when request.Img is { Count: > 0 }:
            {
                var oldImg = await _productImageRepository.GetAllWithQuery(x => x.ProductId == productId && x.IsDeleted == false)
                    .ToListAsync();
                var newIndex = oldImg.Count > 0 ? oldImg.Max(x => x.ImageIndex) + 1 : 0;
                var taskUrl = request.Img.Select(x => _cloudinaryService.UploadFileAsync(x, "solar-images")).ToList();
                var url = await Task.WhenAll(taskUrl);

                var newMedia = url.Select((x, _) => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ImageUrl = x,
                    ImageIndex = newIndex
                }).ToList();
                _productImageRepository.AddBulkAsync(newMedia);
            }
                break;
        }

        return Result.CreateResult("Update product successfully", 200);
    }


    public async Task<Result> DeleteProduct(Guid productId)
    {
        var product = await _productRepository.GetById(productId);
        if (product == null || product.IsDeleted)
        {
            return Result.CreateResult("Product not found", 400);
        }

        _productRepository.RemoveEntity(product);
        return Result.CreateResult("Delete product successfully", 202);
    }

    public async Task<Result> UpdateProductSpecification(Guid productSpecificationId,
        RequestModel.UpdateProductSpecificationRequest request)
    {
        var productSpecification = await _productSpecificationRepository.GetById(productSpecificationId);
        if (productSpecification == null || productSpecification.IsDeleted)
        {
            return Result.CreateResult("Product not found", 400);
        }

        if (request.Key != null && productSpecification.Key != request.Key)
        {
            productSpecification.Key = request.Key;
        }

        if (request.Value != null && productSpecification.Value != request.Value)
        {
            productSpecification.Value = request.Value;
        }

        _productSpecificationRepository.UpdateEntity(productSpecification);
        return Result.CreateResult("Update product specification successfully", 200);
    }

    public async Task<Result> DeleteProductSpecification(Guid productSpecificationId)
    {
        var productSpecification = await _productSpecificationRepository.GetById(productSpecificationId);
        if (productSpecification == null || productSpecification.IsDeleted)
        {
            return Result.CreateResult("Product specification not found", 400);
        }

        _productSpecificationRepository.RemoveEntity(productSpecification);
        return Result.CreateResult("Delete product specification successfully", 202);
    }

}