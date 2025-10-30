using System;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class TagService
{
  private readonly AppDbContext _context;

  public TagService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<ApiResponse<IEnumerable<Tag>>> GetAll()
  {
    var tags = await _context.Tags
      .OrderByDescending(t => t.CreatedAt)
      .ToListAsync();

    if (tags == null || !tags.Any())
      return ApiResponse<IEnumerable<Tag>>.Fail("tags is not found!");

    return ApiResponse<IEnumerable<Tag>>.Success(tags);
  }

  public async Task<ApiResponse<object?>> Create(CreateTagDto bodyData)
  {
    var existingTagName = await _context.Tags
      .FirstOrDefaultAsync(t => t.Name.ToLower() == bodyData.Name.ToLower());

    if (existingTagName != null)
      return ApiResponse<object?>.Fail("Tag name already exists.");

    var slug = bodyData.Slug == null ?
      await generateUniqueSlugForTag(bodyData.Name) :
      await generateUniqueSlugForTag(bodyData.Slug);

    var newTag = new Tag
    {
      Name = bodyData.Name,
      Slug = slug,
      IsActive = bodyData.IsActive,
    };

    _context.Tags.Add(newTag);
    await _context.SaveChangesAsync();

    return ApiResponse<object?>.Success(null, "Tag created successfully.");
  }

  public async Task<ApiResponse<object?>> Update(UpdateTagDto bodyData, uint id)
  {
    var tag = await _context.Tags.FindAsync(id);

    if (tag == null)
      return ApiResponse<object?>.Fail("Tag not found.");

    if (!string.IsNullOrWhiteSpace(bodyData.Name) &&
        !bodyData.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
    {
      var existingTagName = await _context.Tags
        .FirstOrDefaultAsync(t => t.Name.ToLower() == bodyData.Name.ToLower() && t.Id != id);

      if (existingTagName != null)
        return ApiResponse<object?>.Fail("Tag name already exists.");

      tag.Name = bodyData.Name;
    }

    if (!string.IsNullOrWhiteSpace(bodyData.Slug) &&
        !bodyData.Slug.Equals(tag.Slug, StringComparison.OrdinalIgnoreCase))
    {
      var slug = await generateUniqueSlugForTag(bodyData.Slug, id);
      tag.Slug = slug;
    }

    if (bodyData.IsActive.HasValue)
    {
      tag.IsActive = bodyData.IsActive.Value;
    }

    _context.Tags.Update(tag);
    await _context.SaveChangesAsync();

    return ApiResponse<object?>.Success(null, "Tag updated successfully.");
  }

  public async Task<ApiResponse<object?>> Delete(uint id)
  {
    var tag = await _context.Tags.FindAsync(id);

    if (tag == null)
      return ApiResponse<object?>.Fail("Tag not found.");

    _context.Tags.Remove(tag);
    await _context.SaveChangesAsync();

    return ApiResponse<object?>.Success(null, "Tag deleted successfully.");
  }

  private async Task<string> generateUniqueSlugForTag(string input, uint? excludePostId = null)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      input = "default";
    }

    string slug = Helper.Slugify(input);
    string baseSlug = slug;
    int suffix = 1;

    while (await _context.Tags.AnyAsync(p => p.Slug == slug && (excludePostId == null || p.Id != excludePostId)))
    {
      slug = $"{baseSlug}-{suffix}";
      suffix++;
    }

    return slug;
  }
}

