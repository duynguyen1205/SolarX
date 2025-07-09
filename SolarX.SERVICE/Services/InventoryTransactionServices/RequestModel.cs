namespace SolarX.SERVICE.Services.InventoryTransactionServices;

public static class RequestModel
{
    public record UpdateInventoryTransactionReq(
        Guid InventoryTransactionId,
        int QuantityChanged
    );
}