namespace SolarX.SERVICE.Services.DashboardServices;

public static class ResponseModel
{
    public record AdminDashboardResponseModel(
        int TotalBranches,
        int TotalB2BOrders,
        int TotalProducts,
        int TotalInventoryQuantity,
        decimal MonthlyRevenue,
        MonthlyOrdersResponseModel MonthlyOrders,
        List<BranchPerformanceResponseModel> BranchPerformances
    );

    public record MonthlyOrdersResponseModel(
        int OrderCount,
        List<BranchOrderSummaryResponseModel> BranchSummaries
    );

    public record BranchOrderSummaryResponseModel(
        Guid BranchId,
        string BranchName,
        decimal TotalPurchaseAmount,
        int OrderCount
    );

    public record BranchPerformanceResponseModel(
        Guid BranchId,
        string BranchName,
        decimal Revenue,
        int OrderCount,
        decimal GrowthRate,
        decimal PreviousMonthRevenue
    );
    
    //branch dashboard response
    public record BranchDashboardResponseModel(
        RevenueSummaryResponseModel RevenueSummary,
        OrderSummaryResponseModel OrderSummary,
        CustomerSummaryResponseModel CustomerSummary,
        InventorySummaryResponseModel InventorySummary
    );

    public record RevenueSummaryResponseModel(
        decimal TotalRevenue,
        decimal PreviousMonthRevenue,
        decimal GrowthRate,
        string GrowthStatus // "increase", "decrease", "same"
    );

    public record OrderSummaryResponseModel(
        int TotalOrders,
        int TodayOrders
    );

    public record CustomerSummaryResponseModel(
        int TotalCustomers,
        int NewCustomers
    );

    public record InventorySummaryResponseModel(
        int TotalProducts
    );


}