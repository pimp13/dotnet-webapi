using System;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class CategoryService
{
  private readonly ILogger<CategoryService> _logger;
  private readonly AppDbContext _context;

  public CategoryService(ILogger<CategoryService> logger, AppDbContext context)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Category>> GetAll()
  {
    var data = await _context.Categories
        .OrderByDescending(c => c.CreatedAt)
        .Where(c => c.ParentId == null)
        .Include(c => c.Children)
        .ThenInclude(c => c.Children)
        .ToListAsync();

    return data;
  }

  public async Task<ApiResponse<Category>> Create(CreateCategoryDto bodyData)
  {
    try
    {
      var existingCategoryByName = await _context.Categories.AnyAsync(c => c.Name == bodyData.Name);
      if (existingCategoryByName) throw new Exception($"Category by name {bodyData.Name} is exists");

      var slug = (bodyData.Slug == null) ? await generateUniqueSlug(bodyData.Name) : await generateUniqueSlug(bodyData.Slug);

      var category = new Category
      {
        Name = bodyData.Name,
        Slug = slug,
        ParentId = bodyData.ParentId,
        Description = bodyData.Description
      };

      using var transaction = await _context.Database.BeginTransactionAsync();
      await _context.Categories.AddAsync(category);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return ApiResponse<Category>.Success(category, "create a new category is successfully!");
    }
    catch (Exception e)
    {
      return ApiResponse<Category>.Fail($"create category is failed: {e.Message}");
    }
  }

  public async Task<Category?> FindById(uint id)
  {
    var data = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    return data;
  }

  private async Task<string> generateUniqueSlug(string input, uint? excludeCategoryId = null)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      input = "defualt";
    }

    string slug = Helper.Slugify(input);

    string baseSlug = slug;
    int suffix = 1;

    while (await _context.Categories.AnyAsync(c => c.Slug == slug && (excludeCategoryId == null || c.Id != excludeCategoryId)))
    {
      slug = $"{baseSlug}-{suffix}";
      suffix++;
    }

    return slug;
  }
}
