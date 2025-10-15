using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MyFirstApi.Dto;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;


[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;

  public AuthController(AuthService service)
  {
    _authService = service;
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

  [Authorize]
  [HttpGet("info")]
  public IActionResult Info()
  {
     var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;



        return Ok(new
        {
            user = new { id = userId, email }
        });
  }
}