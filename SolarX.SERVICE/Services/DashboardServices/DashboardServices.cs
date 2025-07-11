using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IDashboardServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.DashboardServices;

public class DashboardServices : IDashboardServices
{
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;
    private readonly IBaseRepository<Order, Guid> _orderRepository;
    private readonly IBaseRepository<Product, Guid> _productRepository;
    private readonly IBaseRepository<Inventory, Guid> _inventoryRepository;


    public DashboardServices(IBaseRepository<Agency, Guid> agencyRepository, IBaseRepository<Order, Guid> orderRepository,
        IBaseRepository<Product, Guid> productRepository, IBaseRepository<Inventory, Guid> inventoryRepository)
    {
        _agencyRepository = agencyRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<ResponseModel.AdminDashboardResponseModel>> GetAdminDashboard()
    {
        var currentMonth = DateTimeOffset.UtcNow.Date;
        var startOfMonth = new DateTimeOffset(currentMonth.Year, currentMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var endOfMonth = startOfMonth.AddMonths(1);

        var previousMonth = startOfMonth.AddMonths(-1);
        var startOfPreviousMonth = new DateTimeOffset(previousMonth.Year, previousMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);


        var totalBranches = await _agencyRepository
            .GetAllWithQuery(a => !a.IsDeleted)
            .CountAsync();


        var totalB2BOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted && !o.IsB2C)
            .CountAsync();


        var totalProducts = await _productRepository
            .GetAllWithQuery(p => !p.IsDeleted)
            .CountAsync();


        var totalInventoryQuantity = await _inventoryRepository
            .GetAllWithQuery(i => !i.IsDeleted)
            .SumAsync(i => i.Quantity);


        var monthlyRevenue = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.CreatedAt >= startOfMonth &&
                                  o.CreatedAt < endOfMonth &&
                                  o.OrderStatus == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);


        var monthlyOrders = await GetMonthlyOrdersAsync(startOfMonth, endOfMonth);


        var branchPerformances = await GetBranchPerformancesAsync(startOfMonth, endOfMonth, startOfPreviousMonth, previousMonth);

        var dashboard = new ResponseModel.AdminDashboardResponseModel
        (
            totalBranches,
            totalB2BOrders,
            totalProducts,
            totalInventoryQuantity,
            monthlyRevenue,
            monthlyOrders,
            branchPerformances
        );

        return Result<ResponseModel.AdminDashboardResponseModel>.CreateResult("Admin Dashboard", 200, dashboard);
    }


    private async Task<ResponseModel.MonthlyOrdersResponseModel> GetMonthlyOrdersAsync(DateTimeOffset startOfMonth,
        DateTimeOffset endOfMonth)
    {
        // Lấy tổng số đơn hàng trong tháng
        var orderCount = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.CreatedAt >= startOfMonth &&
                                  o.CreatedAt < endOfMonth)
            .CountAsync();

        // Lấy danh sách đơn hàng B2B với thông tin agency
        var monthlyOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.CreatedAt >= startOfMonth &&
                                  o.CreatedAt < endOfMonth &&
                                  o.BuyerAgencyId != null) // Chỉ lấy đơn B2B
            .Include(o => o.BuyerAgency)
            .ToListAsync();

        // Group và tính toán ở client side
        var branchSummaries = monthlyOrders
            .GroupBy(o => new
            {
                o.BuyerAgencyId,
                BranchName = o.BuyerAgency.Name
            })
            .Select(g => new ResponseModel.BranchOrderSummaryResponseModel(
                g.Key.BuyerAgencyId!.Value,
                g.Key.BranchName,
                g.Sum(o => o.TotalAmount),
                g.Count()
            ))
            .OrderByDescending(x => x.TotalPurchaseAmount)
            .ToList();

        return new ResponseModel.MonthlyOrdersResponseModel(
            orderCount,
            branchSummaries
        );
    }

    private async Task<List<ResponseModel.BranchPerformanceResponseModel>> GetBranchPerformancesAsync(
        DateTimeOffset startOfMonth, DateTimeOffset endOfMonth,
        DateTimeOffset startOfPreviousMonth, DateTimeOffset previousMonth)
    {
        // Lấy doanh thu tháng hiện tại
        var currentMonthOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.CreatedAt >= startOfMonth &&
                                  o.CreatedAt < endOfMonth &&
                                  o.OrderStatus == OrderStatus.Completed)
            .Include(o => o.SellerAgency)
            .ToListAsync();

        // Group ở client side
        var currentMonthRevenue = currentMonthOrders
            .GroupBy(o => new
            {
                o.SellerAgencyId,
                BranchName = o.SellerAgency.Name
            })
            .Select(g => new
            {
                BranchId = g.Key.SellerAgencyId,
                g.Key.BranchName,
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .ToList();

        var previousMonthOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.CreatedAt >= startOfPreviousMonth &&
                                  o.CreatedAt < previousMonth &&
                                  o.OrderStatus == OrderStatus.Completed)
            .ToListAsync();

        // Group ở client side
        var previousMonthRevenue = previousMonthOrders
            .GroupBy(o => o.SellerAgencyId)
            .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));

        var branchPerformances = currentMonthRevenue.Select(current =>
            {
                var previousRevenue = previousMonthRevenue.GetValueOrDefault(current.BranchId, 0);
                var growthRate = previousRevenue > 0
                    ? (current.Revenue - previousRevenue) / previousRevenue * 100
                    : current.Revenue > 0
                        ? 100
                        : 0;

                return new ResponseModel.BranchPerformanceResponseModel(
                    current.BranchId,
                    current.BranchName,
                    current.Revenue,
                    current.OrderCount,
                    Math.Round(growthRate, 2),
                    previousRevenue
                );
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        return branchPerformances;
    }

    public async Task<Result<ResponseModel.BranchDashboardResponseModel>> GetBranchDashboard(Guid agencyId)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var startOfToday = new DateTimeOffset(today, TimeSpan.Zero);
        var endOfToday = startOfToday.AddDays(1);

        var currentMonth = DateTimeOffset.UtcNow.Date;
        var startOfMonth = new DateTimeOffset(currentMonth.Year, currentMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var endOfMonth = startOfMonth.AddMonths(1);

        var previousMonth = startOfMonth.AddMonths(-1);
        var startOfPreviousMonth = new DateTimeOffset(previousMonth.Year, previousMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);

        // 1. Doanh thu tháng (tổng doanh thu, so với tháng trước)
        var totalRevenue = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.SellerAgencyId == agencyId &&
                                  o.CreatedAt >= startOfMonth &&
                                  o.CreatedAt < endOfMonth &&
                                  o.OrderStatus == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        var previousMonthRevenue = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.SellerAgencyId == agencyId &&
                                  o.CreatedAt >= startOfPreviousMonth &&
                                  o.CreatedAt < startOfMonth &&
                                  o.OrderStatus == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        var growthRate = previousMonthRevenue > 0
            ? ((totalRevenue - previousMonthRevenue) / previousMonthRevenue) * 100
            : totalRevenue > 0
                ? 100
                : 0;

        var growthStatus = growthRate > 0 ? "increase" : growthRate < 0 ? "decrease" : "same";

        var revenueSummary = new ResponseModel.RevenueSummaryResponseModel(
            totalRevenue,
            previousMonthRevenue,
            Math.Round(growthRate, 2),
            growthStatus
        );

        // 2. Đơn hàng (tổng đơn hàng, số đơn trong hôm nay)
        var totalOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted && o.SellerAgencyId == agencyId)
            .CountAsync();

        var todayOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.SellerAgencyId == agencyId &&
                                  o.CreatedAt >= startOfToday &&
                                  o.CreatedAt < endOfToday)
            .CountAsync();

        var orderSummary = new ResponseModel.OrderSummaryResponseModel(
            totalOrders,
            todayOrders
        );

        // 3. Khách hàng (tổng khách hàng, số khách hàng mới)
        // Lấy danh sách khách hàng đã mua từ chi nhánh này
        var customerOrders = await _orderRepository
            .GetAllWithQuery(o => !o.IsDeleted &&
                                  o.SellerAgencyId == agencyId &&
                                  o.IsB2C &&
                                  o.CustomerId != null)
            .ToListAsync();

        var totalCustomers = customerOrders.Select(o => o.CustomerId).Distinct().Count();

        var newCustomers = customerOrders
            .Where(o => o.CreatedAt >= startOfMonth && o.CreatedAt < endOfMonth)
            .Select(o => o.CustomerId)
            .Distinct()
            .Count();

        var customerSummary = new ResponseModel.CustomerSummaryResponseModel(
            totalCustomers,
            newCustomers
        );

        // 4. Tồn kho (sản phẩm trong kho)
        var totalProducts = await _inventoryRepository
            .GetAllWithQuery(i => !i.IsDeleted && i.AgencyId == agencyId)
            .CountAsync();

        var inventorySummary = new ResponseModel.InventorySummaryResponseModel(
            totalProducts
        );

        var dashboard = new ResponseModel.BranchDashboardResponseModel(
            revenueSummary,
            orderSummary,
            customerSummary,
            inventorySummary
        );

        return Result<ResponseModel.BranchDashboardResponseModel>.CreateResult("Branch Dashboard", 200, dashboard);

    }
}