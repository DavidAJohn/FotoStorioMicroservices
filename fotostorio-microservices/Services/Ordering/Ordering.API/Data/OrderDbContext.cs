using Microsoft.EntityFrameworkCore;
using Ordering.API.Models;
using System;

namespace Ordering.API.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            // defines the Address as being owned by an Order
            modelBuilder.Entity<Order>()
                .OwnsOne(o => o.SendToAddress, a => {
                    a.WithOwner();
                });

            // establishes that OrderItems are deleted if an Order is deleted
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // defines the ItemOrdered as being owned by an OrderItem
            modelBuilder.Entity<OrderItem>()
                .OwnsOne(oi => oi.ItemOrdered, i => {
                    i.WithOwner();
                });

            // converts the OrderStatus enum into a string
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(
                    os => os.ToString(),
                    os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os)
                );
        }
    }
}
