namespace SolarX.SERVICE.Services.ConsultingRequestServices;

public static class ResponseModel
{
    public record ConsultingRequestResponseModel(
        Guid Id,
        string FullName,
        string Area,
        string? Note,
        string Type,
        string PhoneNumber,
        string Email,
        string Status,
        string MainPurpose,
        string UsageTime,
        double LastUsageLast3Months,
        string ImageUrl,
        decimal Length,
        decimal Width,
        decimal Slope
    );

}