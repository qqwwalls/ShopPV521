using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Domain.Data;
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

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Error: Connection string 'MSSQLConnection' not found in appsettings.json");
    return;
}

var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var context = new ShopDbContext(optionsBuilder.Options))
{
    var orderService = new OrderService(context);

    var testUser = context.Users.FirstOrDefault();
    var testProducts = context.Products.Take(2).Select(p => p.Id).ToList();

    if (testUser != null && testProducts.Any())
    {
        orderService.CreateOrder(testUser.Id, testProducts);
        Console.WriteLine("Order created successfully!");
    }
    else
    {
        Console.WriteLine("Database is empty. Please add at least one User and one Product first.");
    }
}