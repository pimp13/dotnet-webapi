using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
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

    if (result.Ok) return Ok(result);
    return NotFound(result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateTagDto bodyData)
  {
    var result = await _tagService.Create(bodyData);

    if (result.Ok) return Ok(result);
    return BadRequest(result);
  }

  [HttpPatch("{id}")]
  public async Task<IActionResult> Update(uint id, [FromBody] UpdateTagDto bodyData)
  {
    var result = await _tagService.Update(bodyData, id);

    if (result.Ok) return Ok(result);
    return BadRequest(result);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(uint id)
  {
    var result = await _tagService.Delete(id);

    if (result.Ok) return Ok(result);
    return BadRequest(result);
  }
}
