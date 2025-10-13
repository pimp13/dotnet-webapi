using MyFirstApi.Dto;
using MyFirstApi.Models;

namespace MyFirstApi.Extensions;


public static class UserExtensions
{
  public static void ApplyUpdate(this User user, UpdateUserDto dto)
  {
    if (!string.IsNullOrWhiteSpace(dto.FirstName))
      user.FirstName = dto.FirstName;
    if (!string.IsNullOrWhiteSpace(dto.LastName))
      user.LastName = dto.LastName;
    if (!string.IsNullOrWhiteSpace(dto.Email))
      user.Email = dto.Email;
    if (!string.IsNullOrWhiteSpace(dto.Password))
      user.Password = dto.Password;
  }
}