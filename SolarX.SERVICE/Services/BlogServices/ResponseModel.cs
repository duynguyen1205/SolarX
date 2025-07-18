namespace SolarX.SERVICE.Services.BlogServices;

public static class ResponseModel
{
    public record BlogResponseModel(Guid Id, string Title, string? ImageUrl, DateTimeOffset CreatedDate, string CategoryName);

    public record BlogResponseDetail(
        Guid Id,
        string Title,
        string? ImageUrl,
        string Content,
        string? AuthorName,
        DateTimeOffset CreatedDate,
        string CategoryName
    );
}