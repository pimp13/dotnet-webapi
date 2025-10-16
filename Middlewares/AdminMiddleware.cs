using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Models.Enums;

namespace MyFirstApi.Middlewares;


public class AdminMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IConfiguration _config;

  public AdminMiddleware(RequestDelegate next, IConfiguration config)
  {
    _next = next;
    _config = config;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var path = context.Request.Path.Value?.ToLower();

    if (path != null && path.Contains("/admin"))
    {
      var token = context.Request.Cookies["_token"];

      if (string.IsNullOrEmpty(token))
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
          ok = false,
          message = "Unauthorized - no token not found",
          data = (object?)null,
        });
        return;
      }


      try
      {
        var key = _config["Jwt:Key"];
        var tokenHandler = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(key)) throw new Exception("key not found");
        var validationParams = new TokenValidationParameters
        {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var principal = tokenHandler.ValidateToken(token, validationParams, out _);
        var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        var isSupperAdmin = principal.Claims.FirstOrDefault(c => c.Type == "isSupperAdmin")?.Value;
        var isActive = principal.Claims.FirstOrDefault(c => c.Type == "isActive")?.Value;

        if (isSupperAdmin == "True" && isActive == "True")
        {
          await _next(context);
          return;
        }


        if (roleClaim != UserRole.Admin.ToString() && isActive != "True")
        {
          context.Response.StatusCode = StatusCodes.Status403Forbidden;
          await context.Response.WriteAsJsonAsync(new
          {
            ok = false,
            message = "You can't paermission",
            data = (object?)null,
          });
          return;
        }

        context.Items["User"] = principal;
      }
      catch (Exception e)
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
          ok = false,
          message = $"you cant't perssion {e.Message}",
          data = (object?)null,
        });
      }
    }

    await _next(context);
  }
}