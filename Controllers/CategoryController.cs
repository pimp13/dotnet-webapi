using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resp = await _categoryService.GetAll();
        return (resp == null) ? NotFound("Category is not found") : Ok(resp);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto bodyData)
    {
        var (resp, msg) = await _categoryService.Create(bodyData);
        return Ok((resp, msg));
    }
}
