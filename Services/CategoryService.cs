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
            .ToListAsync();

        return data;
    }

    public async Task<(Category?, string)> Create(CreateCategoryDto bodyData)
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

            return (category, "category created is ok");
        }
        catch (Exception e)
        {
            return (null, $"Failed to create category {e.Message}");
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
