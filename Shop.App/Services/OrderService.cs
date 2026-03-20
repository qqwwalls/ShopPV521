using System;
using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Data;
using Shop.Domain.Entities;

namespace Shop.App.Services
{
    public class OrderService
    {
        private readonly ShopDbContext _context;

        public OrderService(ShopDbContext context)
        {
            _context = context;
        }

        public void CreateOrder(int userId, List<int> productIds)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "New",
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            decimal total = 0;
            foreach (var productId in productIds)
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = productId,
                        Quantity = 1,
                        Price = product.Price
                    };
                    total += product.Price;
                    _context.OrderItems.Add(orderItem);
                }
            }

            order.TotalAmount = total;
            _context.SaveChanges();
        }
    }
}