using Microsoft.EntityFrameworkCore;
using Shop.Domain.Data;
using Shop.Domain.Entities;
using System.Diagnostics;

namespace Shop.App.Services;

public class ProductSeeder
{
    private const string ProductNameIndex = "IX_Products_Name";
    private const int BatchSize = 5_000;
    private readonly ShopDbContext _context;

    public ProductSeeder(ShopDbContext context)
    {
        _context = context;
    }

    public async Task SeedAndBenchmarkAsync(int targetProductCount = 200_000, int measurements = 10)
    {
        await _context.Database.MigrateAsync();
        await GenerateAndInsertProductsAsync(targetProductCount);

        string productNameToSearch = $"SeedProduct_{targetProductCount / 2:D7}";

        await DropProductNameIndexIfExistsAsync();
        long elapsedWithoutIndexMs = await MeasureSearchByNameAsync(productNameToSearch, measurements);

        await CreateProductNameIndexIfMissingAsync();
        long elapsedWithIndexMs = await MeasureSearchByNameAsync(productNameToSearch, measurements);

        // Висновок і результати тестів:
        // 1) Результати друкуються нижче в консолі (БЕЗ індекса / З індексом) після кожного запуску бенчмарку.
        // 2) Після створення індекса по Name запит має виконуватись відчутно швидше на великій таблиці.
        // 3) Чим більше записів у Products, тим помітніша різниця на користь індекса.
        Console.WriteLine($"Середній час пошуку БЕЗ індекса: {elapsedWithoutIndexMs} ms");
        Console.WriteLine($"Середній час пошуку З індексом: {elapsedWithIndexMs} ms");
    }

    public async Task GenerateAndInsertProductsAsync(int targetProductCount)
    {
        int existingCount = await _context.Products.CountAsync();
        if (existingCount >= targetProductCount)
        {
            Console.WriteLine($"Пропуск сидування: у таблиці вже {existingCount} продуктів.");
            return;
        }

        int productsToCreate = targetProductCount - existingCount;
        Console.WriteLine($"Створюю {productsToCreate} продуктів для сидування...");

        for (int i = 0; i < productsToCreate; i += BatchSize)
        {
            int take = Math.Min(BatchSize, productsToCreate - i);
            int start = existingCount + i + 1;

            List<Product> batch = new(take);
            for (int j = 0; j < take; j++)
            {
                int seedNumber = start + j;
                batch.Add(new Product
                {
                    Name = $"SeedProduct_{seedNumber:D7}",
                    Price = 10 + seedNumber % 500,
                    StockQuantity = seedNumber % 200,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.Products.AddRange(batch);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }
    }

    private async Task<long> MeasureSearchByNameAsync(string productName, int measurements)
    {
        if (measurements <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(measurements), "Measurements must be greater than zero.");
        }

        long totalMs = 0;

        for (int i = 0; i < measurements; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            _ = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == productName);
            stopwatch.Stop();

            totalMs += stopwatch.ElapsedMilliseconds;
        }

        return totalMs / measurements;
    }

    private async Task DropProductNameIndexIfExistsAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(
            $$"""
            IF EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = '{{ProductNameIndex}}'
                  AND object_id = OBJECT_ID('dbo.Products')
            )
            DROP INDEX [{{ProductNameIndex}}] ON [dbo].[Products];
            """);
    }

    private async Task CreateProductNameIndexIfMissingAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(
            $$"""
            IF NOT EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = '{{ProductNameIndex}}'
                  AND object_id = OBJECT_ID('dbo.Products')
            )
            CREATE INDEX [{{ProductNameIndex}}] ON [dbo].[Products]([Name]);
            """);
    }
}
