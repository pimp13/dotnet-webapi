using System;
using System.ComponentModel.DataAnnotations;
using MyFirstApi.Models;

namespace MyFirstApi.Dto;

public class CreatePostDto
{
  [MaxLength(195)]
  public required string Title { get; set; }

  public string Summary { get; set; } = default!;

  public string? Slug { get; set; } = default!;

  public required string Content { get; set; }

  public IFormFile? Image { get; set; }

  public required uint CategoryId { get; set; }

  public List<string> Tags { get; set; } = new();
}


public class UpdatePostDto
{
  [MaxLength(195)]
  public string? Title { get; set; }

  public string? Summary { get; set; } = default!;

  public string? Slug { get; set; } = default!;

  public string? Content { get; set; }

  public IFormFile? Image { get; set; }

  public uint? CategoryId { get; set; }

  public uint? UserId { get; set; }
}


public class PostResponseDto
{
  public uint Id { get; set; }

  public required string Title { get; set; }

  public string Summary { get; set; } = default!;

  public required string Slug { get; set; }

  public required string Content { get; set; }

  public string? ImageUrl { get; set; } = default!;

  public Category Category { get; set; } = default!;

  public User Author { get; set; } = default!;

  public ICollection<Tag> Tags { get; set; } = new List<Tag>();

  public string? FullImageUrl { get; set; }

  public DateTime CreatedAt { get; set; }
}