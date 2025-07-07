namespace SolarX.SERVICE.Services.InventoryServices;

public static class ResponseModel
{
    public record InventoryResponseModel(Guid InventoryId, Guid ProductId, string ProductName, decimal ProductPrice, int Quantity);
}