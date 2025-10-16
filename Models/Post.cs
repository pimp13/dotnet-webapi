using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Models;


public class Post : BaseModel
{
    [Required]
    [MaxLength(195)]
    public required string Title { get; set; }

    public string Summary { get; set; } = default!;

    public required string Slug { get; set; }

    public required string Content { get; set; }

    // public 

}
