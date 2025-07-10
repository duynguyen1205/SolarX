using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.FaqServices;

public static class RequestModel
{
    public record CreateFaqReq(
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Question must be between 1 and 1000 characters.")]
        string Question,
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Answer must be between 1 and 1000 characters.")]
        string Answer
    );

    public record UpdateFaqReq(string? Question, string? Answer);
}