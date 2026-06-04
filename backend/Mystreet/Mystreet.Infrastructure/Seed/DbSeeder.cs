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
            db.Users.AddRange(
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@mystreet.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsAdmin = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "john@mystreet.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("John@123"),
                    IsAdmin = false
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "sarah@mystreet.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Sarah@123"),
                    IsAdmin = false
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "mike@mystreet.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Mike@123"),
                    IsAdmin = false
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "emma@mystreet.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Emma@123"),
                    IsAdmin = false
                }
            );
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Description = "Classic sneaker", Price = 119.99m, SizesCsv = "7,8,9,10", StockQty = 50, ImageUrl = "Resources/ProductImages/1.png", Category = "Sneakers" },
                new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Description = "Comfort running shoe", Price = 139.99m, SizesCsv = "7,8,9,10,11", StockQty = 35, ImageUrl = "Resources/ProductImages/2.png", Category = "Sneakers" }
            );
        }

        await db.SaveChangesAsync();
    }
}