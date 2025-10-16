using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;


[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  private readonly ILogger<AuthController> _logger;

  public AuthController(AuthService service, ILogger<AuthController> logger)
  {
    _authService = service;
    _logger = logger;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterDto bodyData)
  {
    var user = await _authService.Register(bodyData);
    return Ok(user);
  }


  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto bodyData)
  {
    var token = await _authService.Login(bodyData);

    Response.Cookies.Append("_token", token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.UtcNow.AddMinutes(1440)
    });

    return Ok(token);
  }

  [Authorize(Policy = "ActiveUserOnly")]
  [HttpGet("info")]
  public async Task<IActionResult> Info()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var isSupperAdmin = User.FindFirst("isSuperAdmin")?.Value;
    var user = await _authService.FindUserById(Convert.ToUInt32(userId));

    return Ok(new
    {
      user,
      isSupperAdmin
    });
  }
}