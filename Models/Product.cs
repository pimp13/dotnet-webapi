namespace MyFirstApi.Models;

public class Product
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}