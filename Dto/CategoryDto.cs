using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Dto;

public class CreateCategoryDto
{
    [MaxLength(190)]
    public required string Name { get; set; }

    public string? Slug { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public uint? ParentId { get; set; } = null!;

}

public class UpdateCategoryDto
{
    public string Name { get; set; } = default!;

    public string? Slug { get; set; } = default!;

    public string? Description { get; set; } = default!;

    public uint? ParentId { get; set; } = default!;
}