using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Models;

public class Tag : BaseModel
{
  [MaxLength(190)]
  public required string Name { get; set; }

  [MaxLength(190)]
  public required string Slug { get; set; }

  public bool IsActive { get; set; } = true;

  public ICollection<Post> Posts { get; set; } = new List<Post>();
}
