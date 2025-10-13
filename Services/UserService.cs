using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.Extensions;
using MyFirstApi.Models;

namespace MyFirstApi.Services;

public class UserService
{
	private readonly AppDbContext _context;

	public UserService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<User>> GetAll()
	{
		return await _context.Users.ToListAsync();
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
}