namespace SolarX.SERVICE.Services.CategoryServices;

public static class ResponseModel
{
    public record CategoryResponseModel(Guid Id, string CategoryName, List<ProductViewModel> Products);

    public record ProductViewModel(
        Guid ProductId,
        string ProductName,
        string? ProductDocumentUrl,
        List<ProductImage> ProductImages,
        List<ProductSpecification> ProductSpecifications
    );

    public record ProductImage(Guid ImageId, string ImageUrl);

    public record ProductSpecification(Guid SpecificationId, string Key, string Value);
}