using MyFirstApi.Models.Enums;

namespace MyFirstApi.Dto;

public class CreateUserDto
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

}

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class UserDto
{
    public uint Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;
}

public class UserResponseDto
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public UserRole Role { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public bool IsSupperAdmin { get; set; } = false;

    public IEnumerable<PostResponseDto> Posts { get; set; } = default!;
}