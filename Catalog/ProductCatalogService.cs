using EcommerceOrder.Models;

namespace EcommerceOrder.Catalog;

public interface IProductCatalogService
{
    List<Product> GetAllProducts();
    Product? GetProductById(int id);
    List<Product> SearchByCategory(string category);
    void SeedSampleProducts();
}

public class ProductCatalogService : IProductCatalogService
{
    private readonly List<Product> _products = new();

    public void SeedSampleProducts()
    {
        _products.AddRange(new[]
        {
            new Product { Id = 1, Name = "Wireless Mouse",       Category = "Electronics", Price = 29.99m,  StockQuantity = 100 },
            new Product { Id = 2, Name = "Mechanical Keyboard",  Category = "Electronics", Price = 79.99m,  StockQuantity = 50  },
            new Product { Id = 3, Name = "USB-C Hub",            Category = "Electronics", Price = 39.99m,  StockQuantity = 75  },
            new Product { Id = 4, Name = "Noise-Cancel Headset", Category = "Electronics", Price = 149.99m, StockQuantity = 30  },
            new Product { Id = 5, Name = "Running Shoes",        Category = "Apparel",     Price = 89.99m,  StockQuantity = 60  },
            new Product { Id = 6, Name = "Winter Jacket",        Category = "Apparel",     Price = 119.99m, StockQuantity = 40  },
            new Product { Id = 7, Name = "Python Programming",   Category = "Books",       Price = 34.99m,  StockQuantity = 200 },
            new Product { Id = 8, Name = "Clean Code Book",      Category = "Books",       Price = 29.99m,  StockQuantity = 150 },
            new Product { Id = 9, Name = "Desk Lamp",            Category = "Home",        Price = 24.99m,  StockQuantity = 80  },
            new Product { Id = 10, Name = "Coffee Maker",        Category = "Home",        Price = 59.99m,  StockQuantity = 45  },
        });
    }

    public List<Product> GetAllProducts() => _products;
    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    public List<Product> SearchByCategory(string category) =>
        _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
}
