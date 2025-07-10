namespace SolarX.SERVICE.Services.BlogServices;

public static class ResponseModel
{
    public record BlogResponseModel(Guid Id, string Title, string Content);
}