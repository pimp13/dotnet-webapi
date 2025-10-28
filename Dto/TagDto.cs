using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Dto;

public class CreateTagDto
{
  [MaxLength(190)]
  public required string Name { get; set; }

  [MaxLength(190)]
  public required string Slug { get; set; }

  public bool IsActive { get; set; } = true;


}
