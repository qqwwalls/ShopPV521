using Microsoft.EntityFrameworkCore;
using Shop.Domain.Data;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Domain.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopDbContext _context;

        public ProductRepository(ShopDbContext context)
        {
            _context = context;
        }

        public void Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.AsNoTracking().ToList();
        }

        public void UpdatePrice(int id, decimal newPrice)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Price = newPrice;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}