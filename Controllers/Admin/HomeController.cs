using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Extensions;

namespace MyFirstApi.Controllers.Admin;


[Authorize(Policy = "ActiveUserOnly")]
[Route("v1/admin/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(ApiResponse<string>.Success("Hello welcome to admin panel page!"));
    }
}
