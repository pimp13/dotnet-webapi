using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Tags("[User]")]
[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
public class UserController : ControllerBase
{
  private readonly UserService _service;

  public UserController(UserService service)
  {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAll()
  {
    var users = await _service.GetAll();
    return users == null ?
      NotFound(ApiResponse<IEnumerable<UserResponseDto>>.Fail("User not found")) :
      Ok(ApiResponse<IEnumerable<UserResponseDto>>.Success(users));
  }

  [HttpGet("multiply")]
  public double Multiply(double a, double b) => a * b;

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateUserDto bodyData)
  {
    await _service.Create(bodyData);
    HttpContext.Items["message"] = "Created a new user.";
    return CreatedAtAction(nameof(Create), new());
  }

  [HttpPatch("{id}")]
  public async Task<IActionResult> Update([FromBody] UpdateUserDto bodyData, uint id)
  {
    var result = await _service.Update(id, bodyData);
    return result == null ? NotFound() : Ok(result);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(uint id)
  {
    bool result = await _service.Delete(id);
    return result ? Ok() : BadRequest("User Not Found");
  }
}
