using Microsoft.EntityFrameworkCore;
using Mystreet.Application.DTOs.Orders;
using Mystreet.Application.Interfaces;
using Mystreet.Domain.Entities;
using Mystreet.Domain.Enums;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Application.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    public OrderService(AppDbContext db) => _db = db;

    public async Task<Guid> CreateAsync(Guid userId, CreateOrderDto dto)
    {
        var productIds = dto.Items.Select(x => x.ProductId).ToList();
        var products = await _db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync();

        if (products.Count != dto.Items.Count)
            throw new InvalidOperationException("One or more products not found.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            PaymentMethod = dto.PaymentMethod,
            Status = OrderStatus.Pending
        };

        foreach (var item in dto.Items)
        {
            var product = products.First(x => x.Id == item.ProductId);
            if (product.StockQty < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}.");

            product.StockQty -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                Size = item.Size,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order.Id;
    }

    public async Task<IEnumerable<object>> GetMineAsync(Guid userId)
    {
        return await _db.Orders
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.TotalAmount,
                x.CreatedAt,
                x.ShippingAddress,
                x.PaymentMethod,
                Items = x.Items.Select(i => new
                {
                    i.ProductId,
                    i.ProductName,
                    i.Size,
                    i.Quantity,
                    i.UnitPrice,
                    ImageUrl = i.Product != null ? i.Product.ImageUrl : null
                })
            })
            .ToListAsync();
    }

    public async Task<object?> GetByIdAsync(Guid userId, Guid orderId, bool isAdmin)
    {
        var query = _db.Orders.Include(x => x.Items).ThenInclude(x => x.Product).AsQueryable();

        var order = isAdmin
            ? await query.FirstOrDefaultAsync(x => x.Id == orderId)
            : await query.FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

        if (order is null) return null;

        return new
        {
            order.Id,
            order.Status,
            order.TotalAmount,
            order.ShippingAddress,
            order.PaymentMethod,
            order.CreatedAt,
            Items = order.Items.Select(x => new
            {
                x.ProductId,
                x.ProductName,
                x.Size,
                x.Quantity,
                x.UnitPrice,
                ImageUrl = x.Product != null ? x.Product.ImageUrl : null
            })
        };
    }

    public async Task<bool> CancelAsync(Guid userId, Guid orderId, bool isAdmin)
    {
        var order = isAdmin
            ? await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId)
            : await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

        if (order is null) return false;
        if (order.Status == OrderStatus.Delivered) throw new InvalidOperationException("Delivered orders cannot be cancelled.");

        if (order.Status == OrderStatus.Cancelled) return true;

        await RestoreStockForOrderAsync(order);

        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        return await _db.Orders
            .Include(x => x.User)
            .Include(x => x.Items).ThenInclude(i => i.Product)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.TotalAmount,
                x.CreatedAt,
                x.ShippingAddress,
                x.PaymentMethod,
                CustomerEmail = x.User != null ? x.User.Email : null,
                Items = x.Items.Select(i => new
                {
                    i.ProductId,
                    i.ProductName,
                    i.Size,
                    i.Quantity,
                    i.UnitPrice,
                    ImageUrl = i.Product != null ? i.Product.ImageUrl : null
                })
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId);
        if (order is null) return false;

        if (order.Status != OrderStatus.Cancelled && status == OrderStatus.Cancelled)
            await RestoreStockForOrderAsync(order);

        order.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task RestoreStockForOrderAsync(Order order)
    {
        if (order.Items.Count == 0) return;

        var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(x => productIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var item in order.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
                product.StockQty += item.Quantity;
        }
    }
}