using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SolarX.REPOSITORY.Enum;

namespace SolarX.SERVICE.Services.ConsultingRequestServices;

public static class RequestModel
{
    public record CreateConsultingRequest(
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        string FullName,
        [Required(ErrorMessage = "Area is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Area must be between 1 and 200 characters")]
        string Area,
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        string? Note,
        // [Required(ErrorMessage = "Request type is required")]
        // ConsultingRequestType Type,
        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])\d{8}$", ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        string Email,
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        decimal Length,
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Width  must be greater than 0")]
        decimal Width,
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Slope  must be greater than 0")]
        decimal Slope,
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Average Usage Last 3 Months  must be greater than 0")]
        double AverageUsageLast3Months,
        [Required]
        [StringLength(500, ErrorMessage = "MainPurpose cannot exceed 500 characters")]
        string MainPurpose,
        [Required]
        [StringLength(500, ErrorMessage = "UsageTime cannot exceed 500 characters")]
        string UsageTime,
        IFormFileCollection Image
    );

}