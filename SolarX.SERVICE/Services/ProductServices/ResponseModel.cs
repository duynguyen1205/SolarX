namespace SolarX.SERVICE.Services.ProductServices;

public static class ResponseModel
{
    public record ProductResponse(
        Guid CategoryId,
        Guid ProductId,
        string ProductName,
        string ProductDescription,
        decimal BasePrice,
        string CategoryName,
        string Sku,
        List<ProductImageResponse> Images,
        List<ProductSpecificationResponse> Specifications,
        List<ProductReviewResponse> Reviews,
        string? DocumentUrl
    );

    public record GetAllProductResponse(
        Guid ProductId,
        string CategoryName,
        string ProductName,
        string ProductDescription,
        decimal BasePrice,
        string Sku,
        bool IsActive
    );

    public record ProductImageResponse(
        Guid ImageId,
        int ImageIndex,
        string ImageUrl
    );

    public record ProductReviewResponse(
        Guid ReviewId,
        string ReviewContent,
        int Rating,
        string UserName
    );

    public record ProductSpecificationResponse(
        Guid ProductSpecificationId,
        string Key,
        string Value
    );
}