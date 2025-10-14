using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Tags("[User]")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly UserService _service;

  public UserController(UserService service)
  {
    _service = service;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var users = await _service.GetAll();
    if (users == null) return NotFound(ApiResponse<object>.Fail("Users not found"));
    return Ok(ApiResponse<object>.Success(users));
  }

  [HttpGet("multiply")]
  public double Multiply(double a, double b) => a * b;

  [HttpPost]
  public async Task<IActionResult> Create(CreateUserDto bodyData)
  {
    await _service.Create(bodyData);
    return CreatedAtAction(nameof(Create), new { message = "Created new user successfully" });
  }

  [HttpPatch("{id}")]
  public async Task<IActionResult> Update(UpdateUserDto bodyData, uint id)
  {
    var result = await _service.Update(id, bodyData);
    return result == null ? NotFound() : Ok(result);
  }
}
