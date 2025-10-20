using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Services.Admin;

namespace MyFirstApi.Controllers.Admin;


[Tags("[Admin] Home")]
[Authorize(Policy = "ActiveUserOnly")]
[Route("v1/admin/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{

  private readonly HomeService _homeService;
  public HomeController(HomeService homeService)
  {
    _homeService = homeService;
  }

  [HttpGet]
  public async Task<IActionResult> Index()
  {
    string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    uint userId = Convert.ToUInt32(userIdStr);

    var resp = await _homeService.GetAdminInfo(userId);
    return resp.Ok ? Ok(resp) : Unauthorized(resp);
  }
}
