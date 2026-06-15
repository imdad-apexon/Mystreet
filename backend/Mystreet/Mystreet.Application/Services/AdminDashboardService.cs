using Microsoft.EntityFrameworkCore;
using Mystreet.Application.DTOs.Dashboard;
using Mystreet.Application.Interfaces;
using Mystreet.Domain.Enums;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _db;

    public AdminDashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync(int trendDays = 14, int lowStockThreshold = 10, int recentOrdersLimit = 8)
    {
        var safeTrendDays = Math.Clamp(trendDays, 7, 60);
        var safeLowStockThreshold = Math.Clamp(lowStockThreshold, 1, 100);
        var safeRecentOrdersLimit = Math.Clamp(recentOrdersLimit, 5, 20);

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var weekStart = todayStart.AddDays(-6);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var trendStart = todayStart.AddDays(-(safeTrendDays - 1));

        var orders = await _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Include(x => x.User)
            .ToListAsync();

        var nonCancelledOrders = orders
            .Where(x => x.Status != OrderStatus.Cancelled)
            .ToList();

        var kpis = new DashboardKpisDto
        {
            TotalSales = nonCancelledOrders.SelectMany(x => x.Items).Sum(x => x.Quantity),
            TotalRevenue = nonCancelledOrders.Sum(x => x.TotalAmount),
            RevenueToday = nonCancelledOrders.Where(x => x.CreatedAt >= todayStart).Sum(x => x.TotalAmount),
            RevenueWeek = nonCancelledOrders.Where(x => x.CreatedAt >= weekStart).Sum(x => x.TotalAmount),
            RevenueMonth = nonCancelledOrders.Where(x => x.CreatedAt >= monthStart).Sum(x => x.TotalAmount),
            NumberOfOrders = orders.Count,
            NewCustomers = await _db.Users.AsNoTracking().CountAsync(x => x.CreatedAt >= monthStart)
        };

        var bestSellingProducts = nonCancelledOrders
            .SelectMany(x => x.Items)
            .GroupBy(x => new { x.ProductId, x.ProductName })
            .Select(g => new BestSellingProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                UnitsSold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderByDescending(x => x.UnitsSold)
            .ThenByDescending(x => x.Revenue)
            .Take(5)
            .ToList();

        var lowStockProducts = await _db.Products
            .AsNoTracking()
            .Where(x => x.IsActive && x.StockQty <= safeLowStockThreshold)
            .OrderBy(x => x.StockQty)
            .ThenBy(x => x.Name)
            .Take(8)
            .Select(x => new LowStockProductDto
            {
                ProductId = x.Id,
                ProductName = x.Name,
                Brand = x.Brand,
                StockQty = x.StockQty
            })
            .ToListAsync();

        var recentOrders = orders
            .OrderByDescending(x => x.CreatedAt)
            .Take(safeRecentOrdersLimit)
            .Select(x => new RecentOrderDto
            {
                OrderId = x.Id,
                CustomerEmail = x.User != null ? x.User.Email : null,
                TotalAmount = x.TotalAmount,
                Status = (int)x.Status,
                ItemCount = x.Items.Sum(i => i.Quantity),
                CreatedAt = x.CreatedAt
            })
            .ToList();

        var trendByDate = nonCancelledOrders
            .Where(x => x.CreatedAt >= trendStart)
            .GroupBy(x => x.CreatedAt.Date)
            .ToDictionary(
                g => g.Key,
                g => new SalesTrendPointDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Orders = g.Count()
                });

        var salesTrend = Enumerable.Range(0, safeTrendDays)
            .Select(offset => trendStart.AddDays(offset))
            .Select(date => trendByDate.TryGetValue(date, out var point)
                ? point
                : new SalesTrendPointDto
                {
                    Date = date,
                    Revenue = 0,
                    Orders = 0
                })
            .ToList();

        return new DashboardOverviewDto
        {
            Kpis = kpis,
            BestSellingProducts = bestSellingProducts,
            LowStockProducts = lowStockProducts,
            RecentOrders = recentOrders,
            SalesTrend = salesTrend
        };
    }
}