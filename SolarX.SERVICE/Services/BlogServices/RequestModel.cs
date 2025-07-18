using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SolarX.SERVICE.Services.BlogServices;

public static class RequestModel
{
    public record CreateBlogReq(
        [Required] string Title,
        [Required] string Content,
        IFormFile? Image,
        string? AuthorName,
        [Required] string CategoryName
    );

    public record UpdateBlogReq(string? Title, string? Content, IFormFile? Image, string? CategoryName);
}