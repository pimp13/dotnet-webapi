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
    return Ok(token);
  }
}