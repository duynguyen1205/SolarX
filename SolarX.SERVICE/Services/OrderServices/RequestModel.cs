using System.ComponentModel.DataAnnotations;
using SolarX.REPOSITORY.Enum;

namespace SolarX.SERVICE.Services.OrderServices;

public static class RequestModel
{
    public record CreateOrderReq(
        List<CreateOrderItemDto> OrderItems,
        PaymentMethod PaymentMethod
    );

    public record PublicOrderRequest(
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50 characters.")]
        string FullName,
        [RegularExpression(@"^(0[3|5|7|8|9])\d{8}$", ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,
        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Address must be between 3 and 50 characters.")]
        string Address,
        List<CreateOrderItemDto> Items,
        string? Note
    );

    public record CreateOrderItemDto(
        Guid ProductId,
        int Quantity
    );

    public record UpdateOrderStatusReq(OrderStatus? Status, DeliveryStatus? DeliveryStatus);
}