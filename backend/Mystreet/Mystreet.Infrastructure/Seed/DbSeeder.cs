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
                // Sneakers
                new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Description = "Classic sneaker", Price = 119.99m, SizesCsv = "7,8,9,10", StockQty = 50, ImageUrl = "Resources/ProductImages/1.png", Category = "Sneakers" },
                new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Description = "Comfort running shoe", Price = 139.99m, SizesCsv = "7,8,9,10,11", StockQty = 35, ImageUrl = "Resources/ProductImages/2.png", Category = "Sneakers" },
                new Product { Id = Guid.NewGuid(), Name = "Chuck Taylor All Star", Brand = "Converse", Description = "Canvas high-top sneaker", Price = 69.99m, SizesCsv = "6,7,8,9,10", StockQty = 40, ImageUrl = "Resources/ProductImages/3.jpg", Category = "Sneakers" },

                // Clothing
                new Product { Id = Guid.NewGuid(), Name = "511 Slim Jeans", Brand = "Levi's", Description = "Slim-fit denim jeans", Price = 69.99m, SizesCsv = "30,32,34,36", StockQty = 60, ImageUrl = "Resources/ProductImages/4.jpg", Category = "Clothing" },
                new Product { Id = Guid.NewGuid(), Name = "Casual Cotton Shirt", Brand = "Wrangler", Description = "Premium cotton shirt", Price = 44.99m, SizesCsv = "S,M,L,XL", StockQty = 40, ImageUrl = "Resources/ProductImages/5.jpg", Category = "Clothing" },
                new Product { Id = Guid.NewGuid(), Name = "Essentials Hoodie", Brand = "Puma", Description = "Warm fleece hoodie", Price = 54.99m, SizesCsv = "M,L,XL", StockQty = 45, ImageUrl = "Resources/ProductImages/6.jpg", Category = "Clothing" },
                new Product { Id = Guid.NewGuid(), Name = "Bomber Jacket", Brand = "Zara", Description = "Stylish bomber jacket", Price = 79.99m, SizesCsv = "M,L,XL", StockQty = 28, ImageUrl = "Resources/ProductImages/7.jpg", Category = "Clothing" },

                // Electronics
                new Product { Id = Guid.NewGuid(), Name = "AirPods Pro 2", Brand = "Apple", Description = "Wireless earbuds", Price = 249.99m, SizesCsv = "One Size", StockQty = 22, ImageUrl = "Resources/ProductImages/8.jpg", Category = "Electronics" },
                new Product { Id = Guid.NewGuid(), Name = "Galaxy Watch 6", Brand = "Samsung", Description = "Smart fitness watch", Price = 299.99m, SizesCsv = "One Size", StockQty = 18, ImageUrl = "Resources/ProductImages/9.jpg", Category = "Electronics" },
                new Product { Id = Guid.NewGuid(), Name = "WH-1000XM5", Brand = "Sony", Description = "Noise cancelling headphones", Price = 399.99m, SizesCsv = "One Size", StockQty = 15, ImageUrl = "Resources/ProductImages/10.jpg", Category = "Electronics" },
                new Product { Id = Guid.NewGuid(), Name = "PowerCore 20000", Brand = "Anker", Description = "Portable power bank", Price = 59.99m, SizesCsv = "One Size", StockQty = 70, ImageUrl = "Resources/ProductImages/11.jpg", Category = "Electronics" },
                new Product { Id = Guid.NewGuid(), Name = "MX Master 3S", Brand = "Logitech", Description = "Wireless productivity mouse", Price = 99.99m, SizesCsv = "One Size", StockQty = 32, ImageUrl = "Resources/ProductImages/12.jpg", Category = "Electronics" },
                new Product { Id = Guid.NewGuid(), Name = "Deco Mesh WiFi", Brand = "TP-Link", Description = "Whole-home mesh WiFi", Price = 179.99m, SizesCsv = "One Size", StockQty = 24, ImageUrl = "Resources/ProductImages/13.jpg", Category = "Electronics" },

                // Home & Living
                new Product { Id = Guid.NewGuid(), Name = "Lack Coffee Table", Brand = "IKEA", Description = "Minimalist coffee table", Price = 49.99m, SizesCsv = "One Size", StockQty = 26, ImageUrl = "Resources/ProductImages/14.jpg", Category = "Home & Living" },
                new Product { Id = Guid.NewGuid(), Name = "Hue Smart Bulb", Brand = "Philips", Description = "Smart LED bulb", Price = 39.99m, SizesCsv = "One Size", StockQty = 80, ImageUrl = "Resources/ProductImages/15.jpg", Category = "Home & Living" },
                new Product { Id = Guid.NewGuid(), Name = "V8 Vacuum Cleaner", Brand = "Dyson", Description = "Cordless vacuum cleaner", Price = 429.99m, SizesCsv = "One Size", StockQty = 12, ImageUrl = "Resources/ProductImages/16.jpg", Category = "Home & Living" },
                new Product { Id = Guid.NewGuid(), Name = "Artisan Stand Mixer", Brand = "KitchenAid", Description = "Premium kitchen mixer", Price = 349.99m, SizesCsv = "One Size", StockQty = 10, ImageUrl = "Resources/ProductImages/17.jpg", Category = "Home & Living" },

                // Sports
                new Product { Id = Guid.NewGuid(), Name = "NBA Basketball", Brand = "Spalding", Description = "Official size basketball", Price = 29.99m, SizesCsv = "One Size", StockQty = 90, ImageUrl = "Resources/ProductImages/18.jpg", Category = "Sports" },
                new Product { Id = Guid.NewGuid(), Name = "Tennis Racket", Brand = "Wilson", Description = "Professional tennis racket", Price = 119.99m, SizesCsv = "One Size", StockQty = 25, ImageUrl = "Resources/ProductImages/19.jpg", Category = "Sports" },
                new Product { Id = Guid.NewGuid(), Name = "Yoga Mat", Brand = "Decathlon", Description = "Non-slip yoga mat", Price = 24.99m, SizesCsv = "One Size", StockQty = 65, ImageUrl = "Resources/ProductImages/20.jpg", Category = "Sports" },
                new Product { Id = Guid.NewGuid(), Name = "Football Boots", Brand = "Adidas", Description = "Firm ground football boots", Price = 89.99m, SizesCsv = "7,8,9,10", StockQty = 38, ImageUrl = "Resources/ProductImages/21.jpg", Category = "Sports" },

                // Beauty
                new Product { Id = Guid.NewGuid(), Name = "Revitalift Serum", Brand = "L'Oreal", Description = "Anti-aging serum", Price = 29.99m, SizesCsv = "One Size", StockQty = 75, ImageUrl = "Resources/ProductImages/22.jpg", Category = "Beauty" },
                new Product { Id = Guid.NewGuid(), Name = "Fit Me Foundation", Brand = "Maybelline", Description = "Liquid foundation", Price = 12.99m, SizesCsv = "One Size", StockQty = 110, ImageUrl = "Resources/ProductImages/23.jpg", Category = "Beauty" },
                new Product { Id = Guid.NewGuid(), Name = "Shea Body Butter", Brand = "The Body Shop", Description = "Deep moisturizing cream", Price = 21.99m, SizesCsv = "One Size", StockQty = 68, ImageUrl = "Resources/ProductImages/24.jpg", Category = "Beauty" },
                new Product { Id = Guid.NewGuid(), Name = "Men+Care Body Wash", Brand = "Dove", Description = "Refreshing body wash", Price = 8.99m, SizesCsv = "One Size", StockQty = 120, ImageUrl = "Resources/ProductImages/25.jpg", Category = "Beauty" },

                // Toys & Games
                new Product { Id = Guid.NewGuid(), Name = "Classic Creative Bricks", Brand = "LEGO", Description = "Creative building blocks", Price = 34.99m, SizesCsv = "One Size", StockQty = 50, ImageUrl = "Resources/ProductImages/26.jpg", Category = "Toys & Games" },
                new Product { Id = Guid.NewGuid(), Name = "UNO Card Game", Brand = "Mattel", Description = "Family card game", Price = 9.99m, SizesCsv = "One Size", StockQty = 140, ImageUrl = "Resources/ProductImages/27.jpg", Category = "Toys & Games" },
                new Product { Id = Guid.NewGuid(), Name = "Elite Blaster", Brand = "Nerf", Description = "Foam dart blaster", Price = 24.99m, SizesCsv = "One Size", StockQty = 58, ImageUrl = "Resources/ProductImages/28.jpg", Category = "Toys & Games" },
                new Product { Id = Guid.NewGuid(), Name = "Switch OLED", Brand = "Nintendo", Description = "Portable gaming console", Price = 349.99m, SizesCsv = "One Size", StockQty = 14, ImageUrl = "Resources/ProductImages/29.jpg", Category = "Toys & Games" },

                // Books
                new Product { Id = Guid.NewGuid(), Name = "Clean Code", Brand = "Prentice Hall", Description = "A Handbook of Agile Software Craftsmanship", Price = 39.99m, SizesCsv = "Paperback", StockQty = 100, ImageUrl = "Resources/ProductImages/30.png", Category = "Books" }
            );

            db.SaveChanges();
        }

        await db.SaveChangesAsync();
    }
}