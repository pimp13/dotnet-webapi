using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Models;

public class Category : BaseModel
{
    [MaxLength(190)]
    public required string Name { get; set; }

    [MaxLength(190)]
    public required string Slug { get; set; }

}
