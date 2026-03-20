using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.App.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public void AddNewProduct(string name, decimal price, int stock)
        {
            var product = new Product 
            { 
                Name = name, 
                Price = price, 
                StockQuantity = stock,
                CreatedAt = DateTime.UtcNow 
            };
            _repository.Create(product);
        }

        public void ShowAllProducts()
        {
            var products = _repository.GetAll();
            foreach (var p in products)
            {
                Console.WriteLine($"[{p.Id}] {p.Name} - {p.Price}$ (Stock: {p.StockQuantity})");
            }
        }

        public void ChangePrice(int id, decimal newPrice)
        {
            _repository.UpdatePrice(id, newPrice);
        }

        public void RemoveProduct(int id)
        {
            _repository.Delete(id);
        }
    }
}