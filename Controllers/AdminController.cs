using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
public class AdminController : ControllerBase
{

  [HttpGet]
  public IActionResult Index()
  {
    return Ok("Welcome to admin panel!!");
  }
}