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
  private readonly ILogger<PostService> _logger;
  private readonly IConfiguration _config;

  public PostService(AppDbContext context, FileUploderService fileUploder, ILogger<PostService> logger, IConfiguration config)
  {
    _context = context;
    _fileUploader = fileUploder;
    _logger = logger;
    _config = config;
  }

  public async Task<ApiResponse<IEnumerable<PostResponseDto>>> GetAll(HttpRequest request)
  {
    string domain = _config["AppSettings:BaseUrl"] ?? $"{request.Scheme}://{request.Host.Value}";

    var posts = await _context.Posts
      .AsNoTracking() // چون فقط خواندنی است، باعث بهبود عملکرد میشود
      .Include(p => p.Author)
      .Include(p => p.Tags)
      .Include(p => p.Category)
        .ThenInclude(c => c.Children)
      .OrderByDescending(p => p.CreatedAt)
      .Select(p => new PostResponseDto
      {
        Id = p.Id,
        Title = p.Title,
        Slug = p.Slug,
        ImageUrl = p.ImageUrl,
        FullImageUrl = !string.IsNullOrEmpty(p.ImageUrl)
            ? $"{domain}/{p.ImageUrl.TrimStart('/')}"
            : null,
        Content = p.Content,
        Author = p.Author,
        Tags = p.Tags,
        Category = p.Category,
        Summary = p.Summary,
        CreatedAt = p.CreatedAt,
      })
      .ToListAsync();

    if (posts == null || !posts.Any())
      return ApiResponse<IEnumerable<PostResponseDto>>.Fail("posts not found");

    return ApiResponse<IEnumerable<PostResponseDto>>.Success(posts);

  }

  public async Task<ApiResponse<PostResponseDto>> Create(CreatePostDto bodyData, HttpRequest request, uint userId)
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
        (imageRelativePath, fullImageUrl) =
          await _fileUploader.UploadAsync(bodyData.Image, "posts", request);
      }

      var post = new Post
      {
        CategoryId = bodyData.CategoryId,
        Content = bodyData.Content,
        Slug = slug,
        Title = bodyData.Title,
        UserId = userId,
        ImageUrl = imageRelativePath,
        Summary = bodyData.Summary,
        FullImageUrl = fullImageUrl,
      };

      foreach (var tagName in bodyData.Tags)
      {
        var exitingTag = await _context.Tags
          .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

        if (exitingTag != null)
        {
          post.Tags.Add(exitingTag);
        }
        else
        {
          var newTag = new Tag
          {
            Name = tagName,
            Slug = await generateUniqueSlugForTag(tagName),
            IsActive = true,
          };

          post.Tags.Add(newTag);
        }
      }

      await _context.Posts.AddAsync(post);
      await _context.SaveChangesAsync();

      return ApiResponse<PostResponseDto>.Success(new PostResponseDto
      {
        Content = post.Content,
        Slug = post.Slug,
        Title = post.Title,
        Id = post.Id,
        ImageUrl = post.ImageUrl,
        FullImageUrl = post.FullImageUrl,
        Summary = post.Summary,
        CreatedAt = post.CreatedAt,
      }, "پست جدید با موفقیت ثبت شد");

    }
    catch (Exception e)
    {
      return ApiResponse<PostResponseDto>.Fail($"Error in create post: {e.Message}");
    }
  }

  public async Task<ApiResponse<Post>> Update(uint id, UpdatePostDto bodyData, HttpRequest request)
  {
    try
    {
      var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
      if (post == null)
        return ApiResponse<Post>.Fail("Post Not Found");

      if (!string.IsNullOrWhiteSpace(bodyData.Title))
      {
        post.Title = bodyData.Title;
        post.Slug = await generateUniqueSlug(bodyData.Slug ?? bodyData.Title);
      }

      if (!string.IsNullOrWhiteSpace(bodyData.Content))
        post.Content = bodyData.Content;

      if (!string.IsNullOrWhiteSpace(bodyData.Summary))
        post.Summary = bodyData.Summary;

      if (bodyData.CategoryId.HasValue)
        post.CategoryId = bodyData.CategoryId.Value;

      if (bodyData.UserId.HasValue)
        post.UserId = bodyData.UserId.Value;


      if (bodyData.Image != null)
      {
        _fileUploader.DeleteFileIfExists(post.ImageUrl);

        (string imageRelativePath, string fullImageUrl) =
           await _fileUploader.UploadAsync(bodyData.Image, "posts", request);
        post.ImageUrl = imageRelativePath;
        post.FullImageUrl = fullImageUrl;
      }

      _context.Posts.Update(post);
      await _context.SaveChangesAsync();

      return ApiResponse<Post>.Success(post);

    }
    catch (Exception e)
    {
      return ApiResponse<Post>.Fail($"Error in update pos: {e.Message}");
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

  // private void ApplyUpdate(this Post post, UpdatePostDto bodyData)
  // {
  //   if (!string.IsNullOrWhiteSpace(bodyData.Title))
  //     post.Title = bodyData.Title;
  //   if (!string.IsNullOrWhiteSpace(bodyData.Summary))
  //     post.Title = bodyData.Summary;
  //   if (!string.IsNullOrWhiteSpace(bodyData.Slug))
  //     post.Title = bodyData.Slug;
  //   if (!string.IsNullOrWhiteSpace(bodyData.Content))
  //     post.Title = bodyData.Content;
  //   if (bodyData.CategoryId.HasValue)
  //     post.CategoryId = bodyData.CategoryId.Value;
  //   if (bodyData.UserId.HasValue)
  //     post.CategoryId = bodyData.UserId.Value;
  // }
}
