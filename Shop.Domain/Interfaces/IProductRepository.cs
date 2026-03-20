using Shop.Domain.Entities;

namespace Shop.Domain.Interfaces
{
    public interface IProductRepository
    {
        void Create(Product product);
        IEnumerable<Product> GetAll();
        void UpdatePrice(int id, decimal newPrice);
        void Delete(int id);
    }
}