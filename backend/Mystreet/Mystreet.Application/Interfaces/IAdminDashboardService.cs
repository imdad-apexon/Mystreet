using Mystreet.Application.DTOs.Dashboard;

namespace Mystreet.Application.Interfaces;

public interface IAdminDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(int trendDays = 14, int lowStockThreshold = 10, int recentOrdersLimit = 8);
}