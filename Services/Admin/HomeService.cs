using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services.Admin;

public class HomeService
{
  private readonly AppDbContext _context;

  public HomeService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<ApiResponse<User?>> GetAdminInfo(uint userId)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null) return ApiResponse<User?>.Fail("user is not found!");

    return ApiResponse<User?>.Success(user);
  }
}
