using System.Security.Claims;
using BlogModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;


[Tags("[Auth] User")]
[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  private readonly ILogger<AuthController> _logger;
  private readonly IConfiguration _config;

  private const string TokenCookieName = "_token";

  public AuthController(AuthService service, ILogger<AuthController> logger, IConfiguration config)
  {
    _authService = service;
    _logger = logger;
    _config = config;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterDto bodyData)
  {
    var response = await _authService.Register(bodyData);
    return response.Ok ? Ok(response) : BadRequest(response);
  }


  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto bodyData)
  {
    var response = await _authService.Login(bodyData);

    if (response.Ok && !string.IsNullOrEmpty(response.Data))
    {
      Response.Cookies.Append(TokenCookieName, response.Data, new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"]!))
      });
    }

    return response.Ok ? Ok(response) : BadRequest(response);
  }

  [HttpPost("logout")]
  public IActionResult Logout()
  {
    Response.Cookies.Delete(TokenCookieName);

    return Ok(ApiResponse<object>.Success(null, "Your logout is sucessfully!"));
  }


  [Authorize(Policy = "ActiveUserOnly")]
  [HttpGet("info")]
  public async Task<IActionResult> Info()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var isSupperAdmin = User.FindFirst("isSuperAdmin")?.Value;
    var user = await _authService.FindUserById(Convert.ToUInt32(userId));

    var lib = new ClassLibBlog();
    lib.SayHello(user?.FirstName ?? "Unknow");

    return Ok(ApiResponse<object>.Success(new
    {
      user,
      isSupperAdmin
    }));
  }
}