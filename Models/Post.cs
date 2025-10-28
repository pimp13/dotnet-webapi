using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyFirstApi.Models;


public class Post : BaseModel
{
    [MaxLength(195)]
    public required string Title { get; set; }

    public string Summary { get; set; } = default!;

    [MaxLength(195)]
    public required string Slug { get; set; }

    public required string Content { get; set; }

    public string? ImageUrl { get; set; } = default!;

    public required uint CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = default!;

    public required uint UserId { get; set; }

    [ForeignKey(nameof(UserId)), JsonIgnore]
    public User Author { get; set; } = default!;


    [NotMapped]
    public string? FullImageUrl { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}