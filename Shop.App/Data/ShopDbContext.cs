using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.App.Data;

public class ShopDbContext:DbContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<User> Users { get; set; }
    public ShopDbContext(DbContextOptions<ShopDbContext> options):base(options)
    {
        
    }
}
