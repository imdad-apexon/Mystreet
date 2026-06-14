namespace Mystreet.Application.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public DashboardKpisDto Kpis { get; set; } = new();
    public List<BestSellingProductDto> BestSellingProducts { get; set; } = [];
    public List<LowStockProductDto> LowStockProducts { get; set; } = [];
    public List<RecentOrderDto> RecentOrders { get; set; } = [];
    public List<SalesTrendPointDto> SalesTrend { get; set; } = [];
}

public class DashboardKpisDto
{
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenueToday { get; set; }
    public decimal RevenueWeek { get; set; }
    public decimal RevenueMonth { get; set; }
    public int NumberOfOrders { get; set; }
    public int NewCustomers { get; set; }
}

public class BestSellingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class LowStockProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int StockQty { get; set; }
}

public class RecentOrderDto
{
    public Guid OrderId { get; set; }
    public string? CustomerEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SalesTrendPointDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}