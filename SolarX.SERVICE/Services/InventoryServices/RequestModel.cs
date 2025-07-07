using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.InventoryServices;

public static class RequestModel
{
    public record InventoryRequestModel(
        [Required(ErrorMessage = "Product ID is required")]
        Guid ProductId,
        [Required]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000")]
        int Quantity
    );
}