using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;


namespace MyFirstApi.Services;


public class AuthService
{
  private readonly AppDbContext _context;
  private readonly IConfiguration _config;

  public AuthService(AppDbContext context, IConfiguration config)
  {
    _context = context;
    _config = config;
  }

  public async Task<ApiResponse<User>> Register(RegisterDto bodyData)
  {
    try
    {
      if (await _context.Users.AnyAsync(u => u.Email == bodyData.Email))
        throw new Exception("Email already exists!");

      var user = new User
      {
        FirstName = bodyData.FirstName,
        LastName = bodyData.LastName,
        Email = bodyData.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(bodyData.Password)
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return ApiResponse<User>.Success(user, "register a new user successfully!");
    }
    catch (Exception e)
    {
      return ApiResponse<User>.Fail($"failed to register user {e.Message}");
    }
  }

  public async Task<ApiResponse<string>> Login(LoginDto bodyData)
  {
    try
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == bodyData.Email);

      if (user == null || !BCrypt.Net.BCrypt.Verify(bodyData.Password, user.Password))
        throw new Exception("Invalid email or password");

      return ApiResponse<string>.Success(GenerateJwtToken(user), "login user is successfully");
    }
    catch (Exception e)
    {
      return ApiResponse<string>.Fail($"failed to login user: {e.Message}");
    }
  }

  public async Task<UserResponseDto?> FindUserById(uint id)
  {
    var user = await _context.Users
      .Include(u => u.Posts)
      .ThenInclude(u => u.Category)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        IsActive = u.IsActive,
        IsSupperAdmin = u.IsSupperAdmin,
        Posts = u.Posts.Select(p => new PostResponseDto
        {
          Content = p.Content,
          ImageUrl = p.ImageUrl,
          Slug = p.Slug,
          Author = p.Author,
          Title = p.Title,
          Category = p.Category,
          FullImageUrl = p.FullImageUrl,
          Summary = p.Summary
        }),
      })
      .FirstOrDefaultAsync(e => e.Id == id);
    return user;
  }

  private string GenerateJwtToken(User user)
  {
    var jwtKey = _config["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
      throw new InvalidOperationException("JWT key is not configured.");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim("fullName", $"{user.FirstName} {user.LastName}"),
      new Claim("role", user.Role.ToString()),
      new Claim("isActive", user.IsActive ? "True" : "False"),
      new Claim("isSuperAdmin", user.IsSupperAdmin ? "True" : "False")
    };

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"]!)),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

}