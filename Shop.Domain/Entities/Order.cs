using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "New";
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}