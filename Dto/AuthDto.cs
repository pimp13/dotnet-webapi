namespace MyFirstApi.Dto;

using System.ComponentModel.DataAnnotations;


public class RegisterDto
{
  [Required(ErrorMessage = "نام الزامی است")]
  [StringLength(100, MinimumLength = 2, ErrorMessage = "نام باید حداقل 2 کاراکتر باشه")]
  public string FirstName { get; set; } = default!;

  [Required(ErrorMessage = "نام خانوادگی الزامی است")]
  [StringLength(100, MinimumLength = 2, ErrorMessage = "نام خانوادگی باید حداقل 2 کاراکتر باشه")]
  public string LastName { get; set; } = default!;

  [Required(ErrorMessage = "آدرس ایمیل الزامی است")]
  [EmailAddress(ErrorMessage = "آدرس ایمیل صحیح نمیباشد")]
  public string Email { get; set; } = default!;

  [Required(ErrorMessage = "کلمه عبور الزامی است")]
  [StringLength(100, MinimumLength = 6, ErrorMessage = "کلمه عبور باید بالای 6 کاراکتر باشد")]
  public string Password { get; set; } = default!;
}


public class LoginDto
{
  public string Email { get; set; } = default!;

  public string Password { get; set; } = default!;
}