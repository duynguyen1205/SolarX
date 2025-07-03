using SolarX.SERVICE.Abstractions.IAgencyServices;

namespace SolarX.API.Middlewares;

public class AgencySlugMiddleware
{
    private readonly RequestDelegate _next;

    public AgencySlugMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAgencyServices agencyService)
    {
        string? slug = null;

        if (context.Request.Headers.TryGetValue("X-Agency-Slug", out var headerSlug))
        {
            slug = headerSlug.FirstOrDefault();
        }
        else
        {
            var host = context.Request.Host.Host;
            var hostParts = host.Split('.');

            if (!host.Contains("localhost") && hostParts.Length >= 3)
            {
                slug = hostParts[0];
            }
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            var agency = await agencyService.GetBySlugAsync(slug);
            if (agency != null)
            {
                context.Items["Agency"] = agency;
                context.Items["AgencyId"] = agency.Id;
            }
        }

        await _next(context);
    }
}