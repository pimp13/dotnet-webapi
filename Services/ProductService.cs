using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class ProductService
{
    private readonly List<Product> _products = new();

    public ProductService()
    {
        _products.AddRange(new[]
        {
            new Product{Id = 1, Name = "Keyboard", Price= 500},
            new Product{Id = 2, Name = "Mouse", Price = 200},
            new Product{Id = 3, Name = "Mobile", Price = 1000},
        });
    }

    public IEnumerable<Product> GetAll() => _products;

    public Product? GetById(uint id) => _products.FirstOrDefault(p => p.Id == id);

    public Product Add(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        return product;
    }

    public bool Update(uint id, Product bodyData)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        existing.Name = bodyData.Name;
        existing.Price = bodyData.Price;

        return true;
    }

    public bool Delete(uint id)
    {
        var product = GetById(id);
        if (product == null) return false;

        _products.Remove(product);
        return true;
    }
}