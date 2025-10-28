using System;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
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

    if (tags == null || tags.Any())
      return ApiResponse<IEnumerable<Tag>>.Fail("tags is not found!");

    return ApiResponse<IEnumerable<Tag>>.Success(tags);
  }

  // public async Task<ApiResponse<>>
}
