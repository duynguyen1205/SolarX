namespace SolarX.SERVICE.Services.OrderServices;

public static class ResponseModel
{
    public record OrderResponseModel(
        Guid OrderId,
        string OrderCode,
        string OrderStatus,
        string DeliveryStatus,
        DateTimeOffset OrderDate,
        decimal OrderTotal
    );

    public record OrderStatisticsResponseModel(
        int TodayOrders,
        int PendingOrders,
        int DeliveringOrders,
        int CompletedOrders,
        int Compare
    );

    public record OrderItemResponseModel(Guid ProductId, string ProductName, decimal ProductPrice, int Quantity);
}