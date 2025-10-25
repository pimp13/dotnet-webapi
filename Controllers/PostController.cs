using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resp = await _postService.GetAll(Request);
        return resp.Ok ?
            Ok(resp) :
            NotFound(resp);
    }

    [Authorize(Policy = "ActiveUserOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreatePostDto bodyData)
    {
        var result = await _postService.Create(bodyData, Request);

        return result.Ok ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "ActiveUserOnly")]
    [HttpPatch]
    public async Task<IActionResult> Update(uint id, [FromForm] UpdatePostDto bodyData)
    {
        var result = await _postService.Update(id, bodyData, Request);
        return result.Ok ? Ok(result) : BadRequest(result);
    }
}

