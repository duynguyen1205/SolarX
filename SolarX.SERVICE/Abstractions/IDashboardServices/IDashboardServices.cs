using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.DashboardServices;

namespace SolarX.SERVICE.Abstractions.IDashboardServices;

public interface IDashboardServices
{
    Task<Result<ResponseModel.AdminDashboardResponseModel>> GetAdminDashboard();
    Task<Result<ResponseModel.MonthlyRevenueResponseModel>> GetMonthlyRevenue(int month, int year);

    Task<Result<ResponseModel.BranchDashboardResponseModel>> GetBranchDashboard(Guid agencyId);

}