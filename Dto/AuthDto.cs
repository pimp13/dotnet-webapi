namespace MyFirstApi.Dto;


public class RegisterDto
{
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string Password { get; set; } = default!;
}


public class LoginDto
{
  public string Email { get; set; } = default!;

  public string Password { get; set; } = default!;
}