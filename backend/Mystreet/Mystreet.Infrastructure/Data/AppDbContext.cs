using Microsoft.EntityFrameworkCore;
using Mystreet.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mystreet.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.Property(x => x.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Brand).IsRequired().HasMaxLength(100);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.HasIndex(x => new { x.Name, x.Brand, x.Category, x.Price, x.SizesCsv, x.IsActive }).IsUnique();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User)
             .WithMany(x => x.Orders)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Order)
             .WithMany(x => x.Items)
             .HasForeignKey(x => x.OrderId);
            e.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        });
    }
}