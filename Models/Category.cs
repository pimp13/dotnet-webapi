using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFirstApi.Models;

public class Category : BaseModel
{
    [MaxLength(190)]
    public required string Name { get; set; }

    [MaxLength(190)]
    public required string Slug { get; set; }

    public string? Description { get; set; } = null!;

    public uint? ParentId { get; set; }

    [JsonIgnore]
    public Category? Parent { get; set; }

    public ICollection<Category> Children { get; set; } = new List<Category>();
}
