using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.BlogServices;

public static class RequestModel
{
    public record CreateBlogReq([Required] string Title, [Required] string Content);

    public record UpdateBlogReq(string? Title, string? Content);
}