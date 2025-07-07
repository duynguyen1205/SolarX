namespace SolarX.SERVICE.Services.CategoryServices;

public static class ResponseModel
{
    public record CategoryResponseModel(Guid Id, string CategoryName, List<ProductViewModel> Products);

    public record ProductViewModel(Guid ProductId, string ProductName, decimal? ProductPrice);
}