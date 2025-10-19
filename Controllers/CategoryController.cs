using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
  private readonly CategoryService _categoryService;
  private readonly ILogger<CategoryService> _logger;

  public CategoryController(CategoryService categoryService, ILogger<CategoryService> logger)
  {
    _categoryService = categoryService;
    _logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var resp = await _categoryService.GetAll();
    return (resp == null) ?
      NotFound(ApiResponse<IEnumerable<Category>>.Fail("category is not found")) :
      Ok(ApiResponse<IEnumerable<Category>>.Success(resp));
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCategoryDto bodyData)
  {
    var result = await _categoryService.Create(bodyData);
    return result.Ok ?
      CreatedAtAction(nameof(FindById), result) :
      Problem(
        detail: result.Message,
        statusCode: StatusCodes.Status500InternalServerError,
        title: "Create a new category failed!"
      );
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiResponse<Category>>> FindById(uint id)
  {
    var data = await _categoryService.FindById(id);
    return (data == null) ?
       NotFound(ApiResponse<Category>.Fail($"category by id #{id} not found")) :
       Ok(ApiResponse<Category>.Success(data));
  }

  [HttpPatch("{id}")]
  public async Task<IActionResult> Update(uint id, [FromBody] UpdateCategoryDto bodyData)
  {
    var resp = await _categoryService.Update(bodyData, id);
    return resp.Ok ? Ok(resp) : BadRequest(resp);
  }
}
