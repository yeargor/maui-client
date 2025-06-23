using System.ComponentModel.DataAnnotations;
using Microsoft.Maui.Storage;

namespace MauiDemo2.Dtos
{
    public enum DefaultPhotoType
    {
        MAN,
        WOMAN
    }

    public class RegisterRequestDto
    {
        [RegularExpression(@"\S+", ErrorMessage = "Username cannot be empty or whitespace.")]
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(2, ErrorMessage = "Username must be at least 2 characters long.")]
        public required string Username { get; init; }
        public string? LastName { get; init; }
        public string? FirstName { get; init; }
        public string? SecondName { get; init; }
        
        [EmailAddress]
        public required string Email { get; init; }
        
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long.")]
        public required string Password { get; init; }
        
        public DefaultPhotoType? DefaultType { get; init; }
        public bool IsUsingDefault { get; init; }
        public FileResult? Image { get; init; }
    }
}
