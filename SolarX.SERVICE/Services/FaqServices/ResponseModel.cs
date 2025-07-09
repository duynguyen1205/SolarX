namespace SolarX.SERVICE.Services.FaqServices;

public static class ResponseModel
{
    public record FaqResponseModel(Guid Id, string Question, string Answer);
}