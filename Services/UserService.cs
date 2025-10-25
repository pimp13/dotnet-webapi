using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class UserService
{
	private readonly AppDbContext _context;
	private readonly ILogger<UserService> _logger;

	public UserService(AppDbContext context, ILogger<UserService> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task<IEnumerable<UserResponseDto>> GetAll()
	{
		var users = await _context.Users
			.Include(u => u.Posts)
			.ThenInclude(x => x.Category)
			.ThenInclude(x => x.Children)
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
			.ToListAsync();

		return users;
	}

	public async Task<User?> GetById(uint id)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task Create(CreateUserDto bodyData)
	{
		_context.Users.Add(new User
		{
			Email = bodyData.Email,
			FirstName = bodyData.FirstName,
			LastName = bodyData.LastName,
			Password = bodyData.Password,
		});
		await _context.SaveChangesAsync();
	}

	public async Task<UserDto?> Update(uint id, UpdateUserDto dto)
	{
		_logger.LogInformation($"Updating user by #{id}...");

		var user = await _context.Users.FindAsync(id);
		if (user == null) return null;

		UserExtensions.ApplyUpdate(user, dto);

		await _context.SaveChangesAsync();

		return new UserDto
		{
			Id = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Email = user.Email,
		};
	}

	public async Task<bool> Delete(uint id)
	{
		_logger.LogInformation($"Deleting user by id #{id}...");

		var user = await _context.Users.FindAsync(id);
		if (user == null) return false;

		_context.Users.Remove(user);
		await _context.SaveChangesAsync();

		return true;
	}
}