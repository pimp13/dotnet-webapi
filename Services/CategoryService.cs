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

      var slug = (bodyData.Slug == null) ?
        await generateUniqueSlug(bodyData.Name) :
        await generateUniqueSlug(bodyData.Slug);

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

  public async Task<ApiResponse<Category>> Update(UpdateCategoryDto bodyData, uint id)
  {
    try
    {
      var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
      if (category == null)
        return ApiResponse<Category>.Fail($"Category with id {id} not found");

      // فقط فیلدهایی که مقدار دارند رو تغییر بده
      if (!string.IsNullOrWhiteSpace(bodyData.Name))
      {
        // اگر اسم جدید تکراری است => خطا بده
        var nameExists = await _context.Categories.AnyAsync(c => c.Name == bodyData.Name && c.Id != id);
        if (nameExists)
          throw new Exception($"Category name '{bodyData.Name}' already exists");

        category.Name = bodyData.Name;

        // اگر Slug هم null بود از Name تولید کن
        if (string.IsNullOrWhiteSpace(bodyData.Slug))
          category.Slug = await generateUniqueSlug(bodyData.Name);
      }

      if (!string.IsNullOrWhiteSpace(bodyData.Slug))
      {
        category.Slug = await generateUniqueSlug(bodyData.Slug);
      }

      if (!string.IsNullOrWhiteSpace(bodyData.Description))
      {
        category.Description = bodyData.Description;
      }

      if (bodyData.ParentId.HasValue)
      {
        category.ParentId = bodyData.ParentId;
      }

      using var transaction = await _context.Database.BeginTransactionAsync();
      _context.Categories.Update(category);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return ApiResponse<Category>.Success(category, "Category updated successfully!");
    }
    catch (Exception e)
    {
      return ApiResponse<Category>.Fail($"update category failed: {e.Message}");
    }
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
