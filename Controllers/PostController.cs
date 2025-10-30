using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostService _postService;
    private readonly ILogger<PostService> _logger;

    public PostController(PostService postService, ILogger<PostService> logger)
    {
        _postService = postService;
        _logger = logger;
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
        var strUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation($"user id is loggin {strUserId}");
        if (strUserId == null) return Unauthorized(ApiResponse<object>.Fail("You can't login!"));

        uint userId = Convert.ToUInt32(strUserId);

        var result = await _postService.Create(bodyData, Request, userId);

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

