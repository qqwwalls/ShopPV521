using Microsoft.EntityFrameworkCore.Storage;
using Shop.Domain.Data;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.App.Services
{
    public class OrderService
    {
        private readonly ShopDbContext _context;

        public OrderService(ShopDbContext context)
        {
            _context = context;
        }

        public void CreateOrder(int userId, List<(int ProductId, int Quantity)> items)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Processing",
                    TotalAmount = 0
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                decimal total = 0;
                foreach (var item in items)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product == null) throw new Exception("Product not found");
                    if (product.StockQuantity < item.Quantity) throw new Exception("Low stock");

                    product.StockQuantity -= item.Quantity;
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };
                    total += product.Price * item.Quantity;
                    _context.OrderItems.Add(orderItem);
                }
                order.TotalAmount = total;
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}