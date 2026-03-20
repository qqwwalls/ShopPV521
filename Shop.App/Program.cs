using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.App;
using Shop.App.Services;
using Shop.Domain.Data;
using Shop.Domain.Interfaces;
using Shop.Domain.Repositories;
using System.IO;

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = configBuilder.Build();
string connectionString = configuration.GetConnectionString("MSSQLConnection") ?? throw new Exception("Connection string not found");

ServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddDbContext<ShopDbContext>(opt => 
    opt.UseSqlServer(connectionString));

serviceCollection.AddScoped<IProductRepository, ProductRepository>();
serviceCollection.AddScoped<ProductService>();
serviceCollection.AddScoped<ShopManager>();

ServiceProvider services = serviceCollection.BuildServiceProvider();

ShopManager manager = services.GetRequiredService<ShopManager>();
manager.Run();