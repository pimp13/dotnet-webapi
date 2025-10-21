using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyFirstApi.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MyFirstApi.Models;

[Index(nameof(Email), IsUnique = true)]
public class User : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.User;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsSupperAdmin { get; set; } = false;

    [InverseProperty(nameof(Post.Author)), JsonIgnore]
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public string FullName()
    {
        return $"{FirstName} {LastName}";
    }
}