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

  public required uint UserId { get; set; }
}


public class PostResponseDto
{
  public required string Title { get; set; }

  public string Summary { get; set; } = default!;

  public required string Slug { get; set; }

  public required string Content { get; set; }

  public string? ImageUrl { get; set; } = default!;

  public Category Category { get; set; } = default!;

  public User Author { get; set; } = default!;

  public string? FullImageUrl { get; set; }
}