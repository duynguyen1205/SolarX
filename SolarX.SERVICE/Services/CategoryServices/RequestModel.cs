using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.CategoryServices;

public static class RequestModel
{
    public record CreateCategoryReq(
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        string CategoryName
    );
}