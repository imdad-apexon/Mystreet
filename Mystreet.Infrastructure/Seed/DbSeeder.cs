using BCrypt.Net;
using Mystreet.Domain.Entities;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Infrastructure.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@mystreet.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsAdmin = true
            });
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Description = "Classic sneaker", Price = 119.99m, SizesCsv = "7,8,9,10", StockQty = 50, ImageUrl = "https://picsum.photos/400?1", Category = "Sneakers" },
                new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Description = "Comfort running shoe", Price = 139.99m, SizesCsv = "7,8,9,10,11", StockQty = 35, ImageUrl = "https://picsum.photos/400?2", Category = "Sneakers" }
            );
        }

        await db.SaveChangesAsync();
    }
}