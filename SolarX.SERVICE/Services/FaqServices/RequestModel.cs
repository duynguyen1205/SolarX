using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.FaqServices;

public static class RequestModel
{
    public record CreateFaqReq([Required] string Question, [Required] string Answer);

    public record UpdateFaqReq(string? Question, string? Answer);
}