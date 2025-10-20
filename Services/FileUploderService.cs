namespace MyFirstApi.Services;

public class FileUploderService
{
  private readonly IWebHostEnvironment _env;
  private readonly IConfiguration _config;

  public FileUploderService(IWebHostEnvironment env, IConfiguration config)
  {
    _env = env;
    _config = config;
  }

  public async Task<(string RelativePath, string FullUrl)> UploadAsync(IFormFile file, string subFolder, HttpRequest request)
  {
    if (file == null || file.Length == 0)
      throw new ArgumentException("File is empty");

    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
    if (!Directory.Exists(uploadsFolder))
      Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(stream);
    }

    var relativePath = $"/uploads/{subFolder}/{fileName}";

    var domain = _config["AppSettings:BaseUrl"] ?? $"{request.Scheme}://{request.Host.Value}";
    var fullUrl = $"{domain}{relativePath}";

    return (relativePath, fullUrl);
  }
}
