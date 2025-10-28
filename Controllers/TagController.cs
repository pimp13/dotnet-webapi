using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Tags("[Tag]")]
[Route("v1/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly TagService _tagService;

    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tagService.GetAll();

        if (result.Ok)
            return NotFound(result);
        return Ok(result);
    }
}
