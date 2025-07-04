using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SolarX.SERVICE.Services.AgencyServices;

public static class RequestModel
{
    public record CreateAgencyReq(
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Agency name must be between 3 and 50 characters.")]
        string Name,
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Agency name must be between 3 and 50 characters.")]
        string Slug,
        [Required]
        IFormFile LogoUrl,
        [Required]
        IFormFile BannerUrl,
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Theme Color must be between 3 and 50 characters.")]
        string ThemeColor,
        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])\d{8}$", ErrorMessage = "Invalid phone number format.")]
        string Hotline,
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Markup Percent must be greater than 0")]
        float MarkupPercent,
        [Required]
        bool DisplayWithMarkup,
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Markup Percent must be greater than 0")]
        decimal CreditLimit
    );


    public record UpdateAgencyReq(
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Agency name must be between 3 and 50 characters.")]
        string? Name,
        IFormFile? LogoUrl,
        IFormFile? BannerUrl,
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Theme Color must be between 3 and 50 characters.")]
        string? ThemeColor,
        [RegularExpression(@"^(0[3|5|7|8|9])\d{8}$", ErrorMessage = "Invalid phone number format.")]
        string? Hotline,
        [Range(0.01, double.MaxValue, ErrorMessage = "Markup Percent must be greater than 0")]
        float? MarkupPercent,
        bool? DisplayWithMarkup
    );
}