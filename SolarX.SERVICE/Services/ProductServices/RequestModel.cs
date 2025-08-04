using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SolarX.SERVICE.Services.ProductServices;

public static class RequestModel
{
    public record ProductRequest(
        [Required]
        Guid CategoryId,
        [Required]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters.")]
        string ProductName,
        [Required]
        string ProductDescription,
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than 0")]
        decimal BasePrice,
        [Required] IFormFileCollection Images,
        [Required]
        bool IsActive,
        string Specifications,
        [Required]
        string Sku,
        IFormFile? Document
    );

    public record ProductSpecification(
        string Key,
        string Value
    );

    public record UpdateProductRequest(
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 20 characters.")]
        string? ProductName,
        string? ProductDescription,
        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than 0")]
        decimal? BasePrice,
        IFormFileCollection? Img,
        List<int>? IndexFile,
        Guid? CategoryId,
        bool? IsActive,
        IFormFile? Document
    );

    public record UpdateProductSpecificationRequest(
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product key must be between 3 and 20 characters.")]
        string? Key,
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product value must be between 3 and 20 characters.")]
        string? Value
    );
}