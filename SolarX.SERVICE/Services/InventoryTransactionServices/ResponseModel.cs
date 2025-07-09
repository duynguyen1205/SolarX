namespace SolarX.SERVICE.Services.InventoryTransactionServices;

public static class ResponseModel
{
    public record InventoryTransactionResponseModel(
        Guid InventoryTransactionId,
        int QuantityChanged,
        string ProductName,
        string AgencyName,
        string? Note,
        string InventoryTransactionType,
        DateTimeOffset TransactionDate
    );
}