using System;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class PostService
{
  private readonly AppDbContext _context;
  private readonly FileUploderService _fileUploader;

  public PostService(AppDbContext context, FileUploderService fileUploder)
  {
    _context = context;
    _fileUploader = fileUploder;
  }

  public async Task<ApiResponse<List<Post>>> GetAll()
  {
    var posts = await _context.Posts
      .OrderByDescending(p => p.CreatedAt)
      .Include(p => p.Author)
      .Include(p => p.Category)
      .ToListAsync();

    return posts == null ?
      ApiResponse<List<Post>>.Fail("posts not found") :
      ApiResponse<List<Post>>.Success(posts);
  }

  public async Task<ApiResponse<Post>> Create(CreatePostDto bodyData, HttpRequest request)
  {
    try
    {
      var slug = (bodyData.Slug == null) ?
        await generateUniqueSlug(bodyData.Title) :
        await generateUniqueSlug(bodyData.Slug);

      string? imageRelativePath = null;
      string? fullImageUrl = null;

      if (bodyData.Image != null)
      {
        (imageRelativePath, fullImageUrl) = await _fileUploader.UploadAsync(bodyData.Image, "posts", request);
      }

      var post = new Post
      {
        CategoryId = bodyData.CategoryId,
        Content = bodyData.Content,
        Slug = slug,
        Title = bodyData.Title,
        UserId = bodyData.UserId,
        ImageUrl = imageRelativePath,
        Summary = bodyData.Summary,
        FullImageUrl = fullImageUrl,
      };

      await _context.Posts.AddAsync(post);
      await _context.SaveChangesAsync();

      return ApiResponse<Post>.Success(post);

    }
    catch (Exception e)
    {
      return ApiResponse<Post>.Fail($"Error in create post: {e.Message}");
    }
  }

  private async Task<string> generateUniqueSlug(string input, uint? excludePostId = null)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      input = "default";
    }

    string slug = Helper.Slugify(input);
    string baseSlug = slug;
    int suffix = 1;

    while (await _context.Posts.AnyAsync(p => p.Slug == slug && (excludePostId == null || p.Id != excludePostId)))
    {
      slug = $"{baseSlug}-{suffix}";
      suffix++;
    }

    return slug;
  }
}
