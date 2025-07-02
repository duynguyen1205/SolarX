using System.ComponentModel.DataAnnotations;
using SolarX.REPOSITORY.Enum;

namespace SolarX.SERVICE.Services.AuthServices;

public static class RequestModel
{
    public record RegisterRequest(
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        string Password,
        [Required] [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 50 characters.")]
        string FirstName,
        [Required] [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 50 characters.")]
        string LastName,
        [Required] Role Role,
        [Required] [RegularExpression(@"^(0[3|5|7|8|9])\d{8}$")]
        string PhoneNumber
    );

    public record LoginRequest(
        [Required] [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        string Password
    );
}