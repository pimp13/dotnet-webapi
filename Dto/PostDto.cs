using System;
using System.ComponentModel.DataAnnotations;

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
