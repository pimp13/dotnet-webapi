using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Dto;

public class CreateTagDto
{
  [MaxLength(190)]
  public required string Name { get; set; }

  [MaxLength(190)]
  public string? Slug { get; set; } = null!;

  public bool IsActive { get; set; } = true;
}

public class UpdateTagDto
{
  [MaxLength(190)]
  public string? Name { get; set; } = null!;

  [MaxLength(190)]
  public string? Slug { get; set; } = null!;

  public bool? IsActive { get; set; } = null;
}