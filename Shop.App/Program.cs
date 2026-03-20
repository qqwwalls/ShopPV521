using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Domain.Data;
using Shop.Domain.Entities;
using Shop.App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = builder.Build();
string? connectionString = configuration.GetConnectionString("MSSQLConnection");

if (string.IsNullOrEmpty(connectionString)) return;

var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var context = new ShopDbContext(optionsBuilder.Options))
{
    var orderService = new OrderService(context);

    var user = context.Users.FirstOrDefault();
    if (user == null)
    {
        user = new User 
        { 
            Name = "HelenTest", 
            Email = "helen_unique@test.com", 
            HashPassword = "StrongPassword1234!" 
        };
        context.Users.Add(user);
        context.SaveChanges();
    }

    var product = context.Products.FirstOrDefault();
    if (product == null)
    {
        product = new Product 
        { 
            Name = "Atomicity Test Product", 
            Price = 100, 
            StockQuantity = 10 
        };
        context.Products.Add(product);
        context.SaveChanges();
    }

    Console.WriteLine($"Initial stock: {product.StockQuantity}");

    var basket = new List<(int ProductId, int Quantity)>
    {
        (product.Id, 100) 
    };

    try
    {
        Console.WriteLine("Trying to create order for 100 items...");
        orderService.CreateOrder(user.Id, basket);
        Console.WriteLine("Order created!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Caught expected error: {ex.Message}");
    }

    context.Entry(product).Reload(); 
    Console.WriteLine($"Stock after rollback: {product.StockQuantity}");
    
    int orderCount = context.Orders.Count(o => o.UserId == user.Id);
    Console.WriteLine($"Total orders for this user: {orderCount}");

    if (product.StockQuantity == 10)
    {
        Console.WriteLine("SUCCESS: Transaction rolled back correctly.");
    }
    else
    {
        Console.WriteLine("FAILURE: Transaction did not roll back.");
    }
}