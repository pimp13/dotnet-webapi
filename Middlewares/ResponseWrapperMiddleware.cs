using System.Text.Json;

namespace MyFirstApi.Middlewares;

public class ResponseWrapperMiddleware
{
  private readonly RequestDelegate _next;

  public ResponseWrapperMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var path = context.Request.Path.Value?.ToLower();

    if (path is not null &&
        (path.StartsWith("/swagger") ||
         path.StartsWith("/docs") ||
         path.StartsWith("/favicon") ||
         path.Contains(".css") ||
         path.Contains(".js") ||
         path.Contains(".png") ||
         path.Contains(".ico")))
    {
      await _next(context);
      return;
    }

    var originalBodyStream = context.Response.Body;

    using var memoryStream = new MemoryStream();
    context.Response.Body = memoryStream;

    try
    {
      await _next(context);

      memoryStream.Seek(0, SeekOrigin.Begin);
      var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

      bool ok = context.Response.StatusCode is >= 200 and < 300;
      string? customMessage = context.Items["message"] as string;

      object? responseData = null;

      // ✅ تلاش برای parse کردن JSON به object واقعی
      if (!string.IsNullOrWhiteSpace(responseBody))
      {
        try
        {
          responseData = JsonSerializer.Deserialize<object>(responseBody, new JsonSerializerOptions
          {
            PropertyNameCaseInsensitive = true
          });
        }
        catch
        {
          // اگر JSON نبود، همون string برگردون
          responseData = responseBody;
        }
      }

      var wrappedResponse = new
      {
        ok,
        message = customMessage ?? (ok ? "Success" : "Error"),
        data = responseData
      };

      var json = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
      });

      context.Response.Body = originalBodyStream;
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(json);
    }
    catch (Exception ex)
    {
      context.Response.Body = originalBodyStream;

      var errorResponse = new
      {
        ok = false,
        message = ex.Message,
        data = (object?)null
      };

      var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
      });

      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(json);
    }
  }
}
