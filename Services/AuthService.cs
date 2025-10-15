using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.Dto;
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

  public async Task<User> Register(RegisterDto bodyData)
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

    return user;
  }

  public async Task<string> Login(LoginDto bodyData)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == bodyData.Email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(bodyData.Password, user.Password))
      throw new Exception("Invalid email or password");

    return GenerateJwtToken(user);
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
      new Claim("FullName", $"{user.FirstName} {user.LastName}")
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